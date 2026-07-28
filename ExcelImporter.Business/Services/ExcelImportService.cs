using ClosedXML.Excel;
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

        using var workbook = new XLWorkbook(filePath); //new workbook = holds the excel sheet
        var worksheet = workbook.Worksheet(1); //takes awel sheet
        var lastRow = worksheet.LastRowUsed().RowNumber(); //a5er row used
        var lastDataRow = lastRow - 2; //a5er needed row (last two rows are footer)

        var headerRow = worksheet.Row(15); //row beta3 el column names
        var sampleRow = worksheet.Row(17); //sample row to get each column's data type
        int colCount = headerRow.CellsUsed().Count(); //number of columns needed

        for (int col = 1; col <= colCount; col++)
        {
            string colName = headerRow.Cell(col).GetString().Trim(); //gets esm el columns kolaha 
            if (string.IsNullOrWhiteSpace(colName))
                continue;

            string sampleValue = sampleRow.Cell(col).GetString().Trim(); //sets types el columns kolaha
            Type colType = InferType(sampleValue);

            table.Columns.Add(colName, colType); // adds that column (name and data type) to the data table
        }

        foreach (var row in worksheet.RowsUsed()) //now adds the records themselves 
        {
            int rowNum = row.RowNumber();

            if (rowNum <= 16) //skips the first 16 rows
                continue;

            if (rowNum > lastDataRow) 
                break;
            //otherwise, it's a valid row with a record that must be added
            string firstCell = row.Cell(1).GetString().Trim(); 
            if (string.IsNullOrWhiteSpace(firstCell))
                continue; //firstCell column is empty

            var dataRow = table.NewRow();
            for (int col = 1; col <= table.Columns.Count; col++)
            {
                string cellValue = row.Cell(col).GetString().Trim();
                var columnType = table.Columns[col - 1].DataType;
                dataRow[col - 1] = ConvertValue(cellValue, columnType);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to import {filePath}: {ex.Message}");
            }
        }
    }

    private Type InferType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return typeof(string);

        if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return typeof(DateTime);

        if (int.TryParse(value, out _))
            return typeof(int);

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            return typeof(decimal);

        return typeof(string);
    }

    private object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DBNull.Value;

        if (targetType == typeof(int) && int.TryParse(value, out int i))
            return i;

        if (targetType == typeof(decimal) && decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal d))
            return d;

        if (targetType == typeof(DateTime) && DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            return dt;

        return value;
    }
}