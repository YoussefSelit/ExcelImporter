using System.Data;

namespace ExcelImporter.Business.Interfaces;

public interface IExcelImportService
{
    DataTable ReadExcelToDataTable(string filePath);

    void ImportFile(string filePath, string archiveFolder);

    void ImportAllPendingFiles(string sourceFolder, string archiveFolder);
}