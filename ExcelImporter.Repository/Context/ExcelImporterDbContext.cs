using Microsoft.EntityFrameworkCore;

namespace ExcelImporter.Repository.Context
{
    public class ExcelImporterDbContext : DbContext
    {
        public ExcelImporterDbContext(
            DbContextOptions<ExcelImporterDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ExcelImporterDbContext).Assembly);
        }
    }
}