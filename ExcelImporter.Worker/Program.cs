using ExcelImporter.Business.Interfaces;
using ExcelImporter.Business.Services;
using ExcelImporter.Repository.Context;
using ExcelImporter.Repository.Interfaces;
using ExcelImporter.Repository.Repositories;
using ExcelImporter.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddDbContext<ExcelImporterDbContext>(options =>
//{
//    options.UseOracle(
//        builder.Configuration.GetConnectionString("OracleConnection"));
//});
//builder.Services.AddScoped<ICardImportRepository, CardImportRepository>();
//builder.Services.AddScoped<IExcelImportService, ExcelImportService>();
builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<IRecordRepository>(sp =>
    new RecordRepository(
        builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddScoped<IExcelImportService, ExcelImportService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
