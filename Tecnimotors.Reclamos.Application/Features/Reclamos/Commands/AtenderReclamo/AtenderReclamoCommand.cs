using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.AtenderReclamo
{
    public record AtenderReclamoCommand(string TicketId): IRequest
    {
    }
}
