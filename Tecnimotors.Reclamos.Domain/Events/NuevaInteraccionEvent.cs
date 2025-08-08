using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class NuevaInteraccionEvent: ReclamoEventBase
    {
        public int UsuarioId { get; }
        public NuevaInteraccionEvent(string ticketId, int usuarioId): base(ticketId)
        {
            UsuarioId = usuarioId;
        }
    }
}
