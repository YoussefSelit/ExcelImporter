using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelImporter.Repository.Entities;

public class FileImportLog
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}