using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Common.Interfaces
{
    public interface IExcelService
    {
        byte[] ExportToExcel<T>(IEnumerable<T> data, Dictionary<string, string> columnMappings, string worksheetName = "Datos");
    }
}
