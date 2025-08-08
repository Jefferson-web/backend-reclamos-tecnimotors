using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado
{
    public class GetReclamosListadoQueryHandler : IRequestHandler<GetReclamosListadoQuery, PaginatedList<ReclamoListadoDto>>
    {
        private readonly IReclamosQueries _reclamosQueries;
        private readonly IUsuarioService _usuarioService;

        public GetReclamosListadoQueryHandler(IReclamosQueries reclamosQueries, IUsuarioService usuarioService)
        {
            _reclamosQueries = reclamosQueries ?? throw new ArgumentNullException(nameof(reclamosQueries));
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        }

        public async Task<PaginatedList<ReclamoListadoDto>> Handle(GetReclamosListadoQuery request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            return await _reclamosQueries.GetReclamosPaginadosAsync(
                request.PageNumber,
                request.PageSize,
                request.TicketId,
                request.FechaDesde,
                request.FechaHasta,
                request.Estado,
                request.Prioridad,
                usuarioActual);
        }
    }
}
