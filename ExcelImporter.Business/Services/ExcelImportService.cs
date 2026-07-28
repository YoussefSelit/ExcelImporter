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

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(1);
        var lastRow = worksheet.LastRowUsed().RowNumber();
        var lastDataRow = lastRow - 2;

        var headerRow = worksheet.Row(15);
        var sampleRow = worksheet.Row(17);
        int colCount = headerRow.CellsUsed().Count();

        for (int col = 1; col <= colCount; col++)
        {
            string colName = headerRow.Cell(col).GetString().Trim();
            if (string.IsNullOrWhiteSpace(colName))
                continue;

            string sampleValue = sampleRow.Cell(col).GetString().Trim();
            Type colType = InferType(sampleValue);

            table.Columns.Add(colName, colType);
        }

        foreach (var row in worksheet.RowsUsed())
        {
            int rowNum = row.RowNumber();

            if (rowNum <= 16)
                continue;

            if (rowNum > lastDataRow)
                break;

            string firstCell = row.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(firstCell))
                continue;

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
        DataTable table = ReadExcelToDataTable(filePath);
        string tableName = _repository.GetTablePrefix(filePath);

        if (!_repository.TableExists(tableName))
        {
            _repository.CreateTableFromDataTable(tableName, table);
        }

        _repository.InsertDataTable(tableName, table);

        string fileName = Path.GetFileName(filePath);
        string archivePath = Path.Combine(archiveFolder, fileName);
        File.Move(filePath, archivePath);
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