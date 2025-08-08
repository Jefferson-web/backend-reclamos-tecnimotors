using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetDetalleReclamoCompleto
{
    public class GetReclamoDetalleCompletoQuery : IRequest<ReclamoDetalleCompletoDto>
    {
        public string TicketId { get; set; }

        public GetReclamoDetalleCompletoQuery(string ticketId)
        {
            TicketId = ticketId;
        }
    }
}
