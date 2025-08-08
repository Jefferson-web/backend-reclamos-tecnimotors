using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivos;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivosFiltros
{
    public class ListarMotivosFiltrosQueryHandler : IRequestHandler<ListarMotivosFiltrosQuery, PaginatedList<GetMotivosQueryResponse>>
    {
        private readonly IMotivoRepository _motivoRepository;

        public ListarMotivosFiltrosQueryHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
        }

        public async Task<PaginatedList<GetMotivosQueryResponse>> Handle(ListarMotivosFiltrosQuery request, CancellationToken cancellationToken)
        {
            // Validar parámetros de paginación
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageIndex < 1) request.PageIndex = 1;

            // Obtener datos paginados y el total de registros
            var (motivos, totalCount) = await _motivoRepository.GetPaginatedAsync(
                request.PageIndex,
                request.PageSize,
                request.Nombre);

            // Mapear a objetos de respuesta
            var items = motivos.Select(m => new GetMotivosQueryResponse
            {
                MotivoId = m.MotivoId,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activo = m.Activo,
                FechaRegistro = m.FechaRegistro,
                FechaActualizacion = m.FechaActualizacion
            }).ToList();

            // Crear lista paginada con los resultados
            return new PaginatedList<GetMotivosQueryResponse>(items, totalCount, request.PageIndex, request.PageSize);
        }
    }
}
