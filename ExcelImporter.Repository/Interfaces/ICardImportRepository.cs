using ExcelImporter.Repository.Entities;

namespace ExcelImporter.Repository.Interfaces
{
    public interface ICardImportRepository
    {

        Task BulkInsertAsync(IEnumerable<CardImport> cardImports);

    }
}
