using ExcelImporter.Repository.Context;
using ExcelImporter.Repository.Entities;
using ExcelImporter.Repository.Interfaces;

namespace ExcelImporter.Repository.Repositories
{
    public class CardImportRepository : ICardImportRepository
    {
        private readonly ExcelImporterDbContext _context;

        public CardImportRepository(ExcelImporterDbContext context)
        {
            _context = context;
        }

        public Task BulkInsertAsync(IEnumerable<CardImport> cardImports)
        {
            throw new NotImplementedException();
        }
    }
}
