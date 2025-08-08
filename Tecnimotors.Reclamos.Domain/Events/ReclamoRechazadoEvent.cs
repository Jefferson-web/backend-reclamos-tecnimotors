using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class ReclamoRechazadoEvent: ReclamoEventBase
    {
        public int UsuarioId { get; }
        public ReclamoRechazadoEvent(string ticketId, int usuarioId): base(ticketId)
        {
            UsuarioId = usuarioId;
        }
    }
}
