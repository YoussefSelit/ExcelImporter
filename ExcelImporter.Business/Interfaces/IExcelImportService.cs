using System;
using System.Collections.Generic;
using System.Text;

namespace ExcelImporter.Business.Interfaces
{
    public interface IExcelImportService
    {
        Task ImportExcelAsync();
    }
}
