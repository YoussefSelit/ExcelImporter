using ExcelImporter.Repository.Entities;
using ExcelImporter.Repository.Interfaces;

namespace ExcelImporter.Business.Interfaces
{
    public interface IExcelReader
    {
        Task<List<CardImport>> ReadAsync(string filePath);
    }
}
