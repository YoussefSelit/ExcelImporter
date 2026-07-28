using ExcelImporter.Business.Interfaces;

namespace ExcelImporter.Worker
{
    public class Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
    {


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int intervalMinutes = configuration.GetValue<int>("ExcelSettings:IntervalMinutes");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var importer = scope.ServiceProvider.GetRequiredService<IExcelImportService>();

                string sourceFolder = configuration["ExcelSettings:SourceFolder"]!;
                string processedFolder = configuration["ExcelSettings:ProcessedFolder"]!;

                try
                {
                    logger.LogInformation("Checking for Excel files...");

                    importer.ImportAllPendingFiles(sourceFolder, processedFolder);

                    logger.LogInformation("Import completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Import failed.");
                }

                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        if (logger.IsEnabled(LogLevel.Information))
        //        {
        //            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //        }
        //        await Task.Delay(1000, stoppingToken);
        //    }
        //}
    }
}
