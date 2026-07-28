using ExcelImporter.Business.Interfaces;
using ExcelImporter.Repository.Interfaces;
using ExcelImporter.Repository.Entities;  
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ExcelImporter.Business.Services
{


    public class ExcelImportService
    {
        private readonly ICardImportRepository _repository;

        public ExcelImportService(ICardImportRepository repository)
        {
            _repository = repository;
        }

        public List<CardImport> ReadCardRecordsFromExcel(string filePath)
        {
            var records = new List<CardImport>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var lastRow = worksheet.LastRowUsed().RowNumber();
                var lastDataRow = lastRow - 2; // exclude the last 2 rows entirely

                foreach (var row in worksheet.RowsUsed())
                {
                    int rowNum = row.RowNumber();

                    if (rowNum <= 16)
                        continue;

                    if (rowNum > lastDataRow)
                        break;

                    string firstCell = row.Cell(1).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(firstCell))
                        continue; // skip separator/blank rows

                    var record = new CardImport
                    {
                        ClientBranch = row.Cell(1).GetString().Trim(),
                        CardBranch = row.Cell(2).GetString().Trim(),
                        Pan = row.Cell(3).GetString().Trim(),
                        Mbr = ParseIntOrDefault(row.Cell(4).GetString()),
                        CustomerName = row.Cell(5).GetString().Trim(),
                        ClientId = row.Cell(6).GetString().Trim(),
                        EmbossingName = row.Cell(7).GetString().Trim(),
                        CurrentCmsStatus = row.Cell(8).GetString().Trim(),
                        CurrentOnlineStatus = row.Cell(9).GetString().Trim(),
                        CreationDate = ParseDateOrNull(row.Cell(10).GetString()),
                        ExpiryDate = ParseDateOrNull(row.Cell(11).GetString()),
                        ActivationDate = ParseDateOrNull(row.Cell(12).GetString()),
                        ClosingDate = ParseDateOrNull(row.Cell(13).GetString()),
                        InternalAcc = row.Cell(14).GetString().Trim(),
                        ExternalAcc = row.Cell(15).GetString().Trim(),
                        AccountCurrency = row.Cell(16).GetString().Trim(),
                        MobileNumber = row.Cell(17).GetString().Trim(),
                        PassportNumber = row.Cell(18).GetString().Trim(),
                        CurrentBalance = ParseDecimalOrDefault(row.Cell(19).GetString()),
                        CreditLimit = ParseDecimalOrDefault(row.Cell(20).GetString()),
                        LimitCurrency = row.Cell(21).GetString().Trim(),
                        OnHold = ParseDecimalOrDefault(row.Cell(22).GetString()),
                        ArrestedAmount = ParseDecimalOrDefault(row.Cell(23).GetString()),
                        CardType = row.Cell(24).GetString().Trim(),
                        LimitGroup = row.Cell(25).GetString().Trim(),
                        FinancialProfile = row.Cell(26).GetString().Trim(),
                        ClerkCode = row.Cell(27).GetString().Trim(),
                        IssuanceReason = row.Cell(28).GetString().Trim(),
                        CardProductName = row.Cell(29).GetString().Trim(),
                        ExternalCode = row.Cell(30).GetString().Trim(),
                        IssuancePriority = ParseIntOrDefault(row.Cell(31).GetString()),
                        PersonalCode = row.Cell(32).GetString().Trim(),
                        ContractNumber = row.Cell(33).GetString().Trim(),
                        Gender = row.Cell(34).GetString().Trim(),
                        Birthday = ParseDateOrNull(row.Cell(35).GetString()),
                        ContactAddress = row.Cell(36).GetString().Trim(),
                        Contactless = row.Cell(37).GetString().Trim(),
                    };

                    records.Add(record);
                }
            }

            return records;
        }

        public void ImportCardRecords(string filePath, string archiveFolder)
        {
            List<CardImport> records = ReadCardRecordsFromExcel(filePath);

            _repository.BulkInsertAsync(records); // Service calls Repository

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
                    ImportCardRecords(filePath, archiveFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to import {filePath}: {ex.Message}");
                }
            }
        }

        // --- Helper methods for safe parsing ---

        private int ParseIntOrDefault(string value)
        {
            return int.TryParse(value?.Trim(), out int result) ? result : 0;
        }

        private decimal ParseDecimalOrDefault(string value)
        {
            return decimal.TryParse(value?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result)
                ? result
                : 0;
        }

        private DateTime? ParseDateOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result)
                ? result
                : (DateTime?)null;
        }
    }
}