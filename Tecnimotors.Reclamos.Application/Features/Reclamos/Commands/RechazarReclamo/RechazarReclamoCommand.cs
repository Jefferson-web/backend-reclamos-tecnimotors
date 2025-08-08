using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.RechazarReclamo
{
    public record RechazarReclamoCommand(string TicketId, string MotivoRechazo): IRequest<bool>
    {
    }
}
