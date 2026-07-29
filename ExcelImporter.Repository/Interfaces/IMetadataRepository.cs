using ExcelImporter.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelImporter.Repository.Interfaces
{
    public interface IMetadataRepository
    {
        //List<FileConfig> GetAllFileConfigs();
        FileConfig GetFileConfigByPrefix(string prefix);
        void LogFileImport(FileImportLog log);
    }
}
