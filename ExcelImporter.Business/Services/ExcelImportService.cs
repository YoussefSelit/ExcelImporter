using ClosedXML.Excel;
using ExcelImporter.Business.Interfaces;
using ExcelImporter.Repository.Entities;
using ExcelImporter.Repository.Interfaces;
using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace ExcelImporter.Business.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly IRecordRepository _repository;
    private readonly IMetadataRepository _metadataRepository;

    private static readonly string[] DateFormats = { "dd/MM/yyyy", "dd-MM-yyyy" };

    public ExcelImportService(IRecordRepository repository, IMetadataRepository metadataRepository)
    {
        _repository = repository;
        _metadataRepository = metadataRepository;
    }

    public DataTable ReadExcelToDataTable(string filePath, FileConfig config)
    {
        var table = new DataTable();

        using var workbook = new XLWorkbook(filePath); //new workbook = holds the excel sheet
        var worksheet = workbook.Worksheet(1); //takes awel sheet

        int headerRowNum = config.HeaderRow ?? 1; //given directly now, no more -1 derivation
        int ignoreFirstRows = config.IgnoreFirstNoOfRows ?? headerRowNum;
        int ignoreLastRows = config.IgnoreLastNoOfRows ?? 0;
        int ignoreFirstCols = config.IgnoreFirstNoOfColumns ?? 0;
        int ignoreLastCols = config.IgnoreLastNoOfColumns ?? 0;

        var lastRow = worksheet.LastRowUsed().RowNumber(); //a5er row used
        var lastDataRow = lastRow - ignoreLastRows; //a5er needed row (footer rows ignored)

        var headerRow = worksheet.Row(headerRowNum); //row beta3 el column names - straight from config
        var sampleRow = worksheet.Row(ignoreFirstRows + 1); //sample row to get each column's data type

        int firstCol = ignoreFirstCols + 1;
        int lastCol = headerRow.LastCellUsed().Address.ColumnNumber - ignoreLastCols; //number of columns needed

        for (int col = firstCol; col <= lastCol; col++)
        {
            string colName = headerRow.Cell(col).GetString().Trim(); //gets esm el columns kolaha
            //if (string.IsNullOrWhiteSpace(colName))
            //    continue;

            string sampleValue = sampleRow.Cell(col).GetString().Trim(); //sets types el columns kolaha
            Type colType = InferType(sampleValue);

            table.Columns.Add(colName, colType); // adds that column (name and data type) to the data table
        }

        foreach (var row in worksheet.RowsUsed()) //now adds the records themselves
        {
            int rowNum = row.RowNumber();

            if (rowNum <= ignoreFirstRows) //skips the first rows (header + separator)
                continue;

            if (rowNum > lastDataRow)
                break;
            //otherwise, it's a valid row with a record that must be added
            string firstCell = row.Cell(firstCol).GetString().Trim();
            //if (string.IsNullOrWhiteSpace(firstCell))
            //    continue; //firstCell column is empty

            var dataRow = table.NewRow();
            int tableColIndex = 0;
            for (int col = firstCol; col <= lastCol; col++)
            {
                string headerName = headerRow.Cell(col).GetString().Trim();
                //if (string.IsNullOrWhiteSpace(headerName))
                //    continue; //this column was skipped when building the table, skip it here too

                string cellValue = row.Cell(col).GetString().Trim();
                var columnType = table.Columns[tableColIndex].DataType;
                dataRow[tableColIndex] = ConvertValue(cellValue, columnType);
                tableColIndex++;
            }

            table.Rows.Add(dataRow);
        }

        return table;
    }

    public void ImportFile(string filePath, FileConfig config, string archiveFolder)
    {
        DateTime importStart = DateTime.Now;
        string fileName = Path.GetFileName(filePath);//gets file name from the path

        DataTable table = ReadExcelToDataTable(filePath, config); //reads from excel sheet
        string tableName = config.Prefix; //prefix comes straight from FileConfig

        if (!_repository.TableExists(tableName)) //if no table with prefix name, create one
        {
            //DateTime startTime = DateTime.Now;
            _repository.CreateTableFromDataTable(tableName, table);
            //DateTime endTime = DateTime.Now;

            //_metadataRepository.LogTableCreation(tableName, startTime, endTime); //log the creation
        }
        else
        {
            _repository.TruncateTable(tableName);
        }


        _repository.InsertDataTable(tableName, table); //add data to database
        DateTime importEnd = DateTime.Now;

        var log = new FileImportLog
        {
            FileName = Path.GetFileName(filePath),
            StartTime = importStart,
            EndTime = importEnd
        };
        _metadataRepository.LogFileImport(log);
        string archivePath = Path.Combine(archiveFolder, fileName); //creates new path (destination)
        File.Move(filePath, archivePath); //moves
    }

    public void ImportAllPendingFiles(string sourceFolder, string archiveFolder)
    {
        var excelFiles = Directory.GetFiles(sourceFolder, "*.xlsx"); //scan the input folder directly now

        foreach (var filePath in excelFiles)
        {
            try
            {
                string prefix = GetPrefixFromFileName(filePath);
                var config = _metadataRepository.GetFileConfigByPrefix(prefix); //look up settings by prefix

                if (config == null)
                {
                    Console.WriteLine($"No FileConfig found for prefix '{prefix}', skipping {filePath}.");
                    continue; //leave the file alone, don't move or import it
                }

                ImportFile(filePath, config, archiveFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to import {filePath}: {ex.Message}");
            }
        }
    }

    private string GetPrefixFromFileName(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        return fileName.Split('_')[0]; //e.g. "TEst_NI.xlsx" -> "TEst"
    }

    private Type InferType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return typeof(string);

        if (DateTime.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return typeof(DateTime); //covers dd/MM/yyyy (e.g. Creation Date) and dd-MM-yyyy (e.g. Birthday)

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

        if (targetType == typeof(DateTime) && DateTime.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            return dt;

        if (targetType != typeof(string))
            return DBNull.Value; //can't convert to the inferred type -> null it out, never leak a raw string into a typed column

        return value;
    }
}