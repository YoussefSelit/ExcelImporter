using ExcelImporter.Repository.Entities;
using System.Data;

namespace ExcelImporter.Business.Interfaces;

public interface IExcelImportService
{
    DataTable ReadExcelToDataTable(string filePath, FileConfig config);
    void ImportFile(string filePath, FileConfig config, string archiveFolder);
    void ImportAllPendingFiles(string sourceFolder, string archiveFolder);
}