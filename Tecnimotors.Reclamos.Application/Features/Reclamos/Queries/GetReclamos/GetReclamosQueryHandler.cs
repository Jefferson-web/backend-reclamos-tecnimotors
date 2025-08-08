using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamos
{
    public class GetReclamosQueryHandler : IRequestHandler<GetReclamosQuery, PaginatedList<ReclamoDto>>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IUsuarioService _usuarioService;

        public GetReclamosQueryHandler(IReclamoRepository reclamoRepository, IUsuarioService usuarioService)
        {
            _reclamoRepository = reclamoRepository ?? throw new ArgumentNullException(nameof(reclamoRepository));
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(reclamoRepository));
        }

        public async Task<PaginatedList<ReclamoDto>> Handle(GetReclamosQuery request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            var (reclamos, totalCount) = await _reclamoRepository.GetPaginatedAsync(
                request.PageNumber,
                request.PageSize,
                request.TicketId,
                request.FechaDesde,
                request.FechaHasta,
                request.Estado,
                request.Prioridad);

            var reclamosDtos = reclamos.Select(r =>
            {
                return new ReclamoDto
                {
                    TicketId = r.TicketId,
                    UsuarioId = r.UsuarioId,
                    Detalle = r.Detalle,
                    Estado = r.Estado,
                    Nombres = r.Nombres,
                    Apellidos = r.Apellidos,
                    Prioridad = r.Prioridad,
                    FechaCreacion = r.FechaCreacion,
                    UltimaModificacion = r.UltimaModificacion,
                };
            }).ToList();

            return new PaginatedList<ReclamoDto>(
                reclamosDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }

}
