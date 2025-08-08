using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;

namespace Tecnimotors.Reclamos.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public byte[] ExportToExcel<T>(IEnumerable<T> data, Dictionary<string, string> columnMappings, string worksheetName = "Datos")
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(worksheetName);

                var headerRow = worksheet.Row(1);
                int columnIndex = 1;

                foreach (var mapping in columnMappings)
                {
                    headerRow.Cell(columnIndex).Value = mapping.Value;
                    headerRow.Cell(columnIndex).Style.Font.Bold = true;
                    headerRow.Cell(columnIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
                    columnIndex++;
                }

                int rowIndex = 2;
                foreach (var item in data)
                {
                    columnIndex = 1;
                    foreach (var property in columnMappings.Keys)
                    {
                        PropertyInfo propertyInfo = typeof(T).GetProperty(property);
                        var value = propertyInfo?.GetValue(item);

                        // Manejar valores especiales
                        if (value is DateTime dateTime)
                        {
                            worksheet.Cell(rowIndex, columnIndex).Value = dateTime.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                        {
                            worksheet.Cell(rowIndex, columnIndex).Value = value?.ToString() ?? "";
                        }

                        columnIndex++;
                    }
                    rowIndex++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
