using ExcelImporter.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExcelImporter.Repository.Context
{
    public class ExcelImporterDbContext : DbContext
    {
        public ExcelImporterDbContext(DbContextOptions<ExcelImporterDbContext> options) : base(options)
        {
        }

        public DbSet<CardImport> CardImports { get; set; }

        // Choose the table name for the CardImport entity 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExcelImporterDbContext).Assembly);
        }

    }
}
