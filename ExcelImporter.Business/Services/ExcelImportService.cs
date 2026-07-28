using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelImporter.Business.Interfaces;
using ExcelImporter.Repository.Interfaces;
using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace ExcelImporter.Business.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly IRecordRepository _repository;

    public ExcelImportService(IRecordRepository repository)
    {
        _repository = repository;
    }

    public DataTable ReadExcelToDataTable(string filePath)
    {
        var table = new DataTable();
        var excelColumns = new List<int>();

        using var workbook = new XLWorkbook(filePath); //new workbook = holds the excel sheet
        var worksheet = workbook.Worksheet(1); //takes awel sheet
        var lastRow = worksheet.LastRowUsed().RowNumber(); //a5er row used
        var lastDataRow = lastRow - 2; //a5er needed row (last two rows are footer)

        var headerRow = worksheet.Row(15); //row beta3 el column names
        var sampleRow = worksheet.Row(17); //sample row to get each column's data type
        int colCount = 0;

        for (int col = 1; col <= headerRow.CellCount(); col++)
        {
            if (!string.IsNullOrWhiteSpace(headerRow.Cell(col).GetString()))
                colCount = col;
        } //number of columns needed

        for (int col = 1; col <= colCount; col++)
        {
            string colName = headerRow.Cell(col).GetString().Trim(); //gets esm el columns kolaha 
            if (string.IsNullOrWhiteSpace(colName))
                continue;

            string sampleValue = sampleRow.Cell(col).GetString().Trim(); //sets types el columns kolaha
            Type colType = InferColumnType(
                    worksheet,
                    col,
                    17,
                    lastDataRow);

            table.Columns.Add(colName, colType); // adds that column (name and data type) to the data table
            excelColumns.Add(col);
        }

        foreach (var row in worksheet.RowsUsed()) //now adds the records themselves 
        {
            int rowNum = row.RowNumber();

            if (rowNum <= 16) //skips the first 16 rows
                continue;

            if (rowNum > lastDataRow) 
                break;
            //otherwise, it's a valid row with a record that must be added
            bool isEmptyRow = true;

            for (int col = 1; col <= table.Columns.Count; col++)
            {
                if (!string.IsNullOrWhiteSpace(row.Cell(col).GetString()))
                {
                    isEmptyRow = false;
                    break;
                }
            }

            if (isEmptyRow)
                continue;

            var dataRow = table.NewRow();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                int excelCol = excelColumns[i];

                string cellValue = row.Cell(excelCol).GetString().Trim();
                var columnType = table.Columns[i].DataType;

                dataRow[i] = ConvertValue(cellValue, columnType);
            }

            table.Rows.Add(dataRow);
        }

        return table;
    }

    public void ImportFile(string filePath, string archiveFolder)
    {
        DataTable table = ReadExcelToDataTable(filePath); //reads from excel sheet
        string tableName = _repository.GetTablePrefix(filePath); //gets prefix

        if (!_repository.TableExists(tableName)) //if no table with prefix name, create one
        {
            _repository.CreateTableFromDataTable(tableName, table);
        }

        Console.WriteLine($"Rows: {table.Rows.Count}");
        Console.WriteLine($"Columns: {table.Columns.Count}");

        foreach (DataColumn column in table.Columns)
        {
            Console.Write(column.ColumnName + " | ");
        }

        Console.WriteLine();

        foreach (DataRow row in table.Rows.Cast<DataRow>().Take(3))
        {
            foreach (var item in row.ItemArray)
            {
                Console.Write(item + " | ");
            }

            Console.WriteLine();
        }
        _repository.InsertDataTable(tableName, table); //add data to database

        string fileName = Path.GetFileName(filePath); //gets file name from the path
        string archivePath = Path.Combine(archiveFolder, fileName); //creates new path (destination)
        File.Move(filePath, archivePath); //moves
    }

    public void ImportAllPendingFiles(string sourceFolder, string archiveFolder)
    {
        var excelFiles = Directory.GetFiles(sourceFolder, "*.xlsx");

        foreach (var filePath in excelFiles)
        {
            try
            {
                ImportFile(filePath, archiveFolder);
            }
            catch(Exception ex)
{
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }

    //private Type InferType(string value)
    //{
    //    if (DateTime.TryParseExact(
    //            value,
    //            "dd/MM/yyyy",
    //            CultureInfo.InvariantCulture,
    //            DateTimeStyles.None,
    //            out _))
    //    {
    //        return typeof(DateTime);
    //    }

    //    return typeof(string);
    //}

    private Type InferColumnType(IXLWorksheet worksheet, int column, int startRow, int endRow)
    {
        bool isDate = true;
        bool isLong = true;
        bool isDecimal = true;

        for (int row = startRow; row <= endRow; row++)
        {
            string value = worksheet.Cell(row, column).GetString().Trim();

            if (string.IsNullOrWhiteSpace(value))
                continue;

            if (!DateTime.TryParseExact(
                    value,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _))
            {
                isDate = false;
            }

            if (!long.TryParse(value, out _))
            {
                isLong = false;
            }

            if (!decimal.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out _))
            {
                isDecimal = false;
            }
        }

        if (isDate)
            return typeof(DateTime);

        if (isLong)
            return typeof(long);

        if (isDecimal)
            return typeof(decimal);

        return typeof(string);
    }

    //private object ConvertValue(string value, Type targetType)
    //{
    //    if (string.IsNullOrWhiteSpace(value))
    //        return DBNull.Value;

    //    if (targetType == typeof(int) && int.TryParse(value, out int i))
    //        return i;

    //    if (targetType == typeof(decimal) && decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d))
    //        return d;

    //    if (targetType == typeof(DateTime) && DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
    //        return dt;

    //    return value;
    //}

    private object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DBNull.Value;

        try
        {
            if (targetType == typeof(long))
                return long.Parse(value);

            if (targetType == typeof(decimal))
                return decimal.Parse(value, CultureInfo.InvariantCulture);

            if (targetType == typeof(DateTime))
                return DateTime.ParseExact(
                    value,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);

            return value;
        }
        catch
        {
            return DBNull.Value;
        }
    }
}