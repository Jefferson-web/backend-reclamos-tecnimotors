using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetDetalleReclamoCompleto
{
    public class GetReclamoDetalleCompletoQueryHandler : IRequestHandler<GetReclamoDetalleCompletoQuery, ReclamoDetalleCompletoDto>
    {
        private readonly IReclamosQueries _reclamosQueries;

        public GetReclamoDetalleCompletoQueryHandler(IReclamosQueries reclamosQueries)
        {
            _reclamosQueries = reclamosQueries ?? throw new ArgumentNullException(nameof(reclamosQueries));
        }

        public async Task<ReclamoDetalleCompletoDto> Handle(GetReclamoDetalleCompletoQuery request, CancellationToken cancellationToken)
        {
            return await _reclamosQueries.GetReclamoDetalleCompletoAsync(request.TicketId);
        }
    }
}
