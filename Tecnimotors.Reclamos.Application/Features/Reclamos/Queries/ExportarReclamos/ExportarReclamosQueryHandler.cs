using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.ExportarReclamos
{
    public class ExportarReclamosQueryHandler : IRequestHandler<ExportarReclamosQuery, ExportarReclamosVm>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IExcelService _excelService;

        public ExportarReclamosQueryHandler(
            IReclamoRepository reclamoRepository,
            IExcelService excelService)
        {
            _reclamoRepository = reclamoRepository ?? throw new ArgumentNullException(nameof(reclamoRepository));
            _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        }

        public async Task<ExportarReclamosVm> Handle(ExportarReclamosQuery request, CancellationToken cancellationToken)
        {
            // Obtener los datos con los filtros aplicados (sin paginación)
            var reclamos = await _reclamoRepository.GetReclamosParaExportarAsync(
                request.TicketId,
                request.FechaDesde,
                request.FechaHasta,
                request.Estado,
                request.Prioridad);

            // Definir el mapeo de las columnas (Propiedad -> Nombre en Excel)
            var columnMappings = new Dictionary<string, string>
            {
                { "TicketId", "Ticket" },
                { "Cliente", "Cliente" },
                { "Nombres", "Nombres" },
                { "Apellidos", "Apellidos" },
                { "Telefono", "Teléfono" },
                { "Correo", "Correo" },
                { "Estado", "Estado" },
                { "Prioridad", "Prioridad" },
                { "FechaCreacion", "Fecha de Creación" },
                { "FechaCierre", "Fecha de Cierre" },
                { "UltimaModificacion", "Última Modificación" }
            };

            // Generar el excel
            var fileContent = _excelService.ExportToExcel(reclamos, columnMappings, "Reclamos");

            // Generar nombre de archivo con timestamp
            var fileName = $"Reclamos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return new ExportarReclamosVm
            {
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Content = fileContent
            };
        }
    }
}
