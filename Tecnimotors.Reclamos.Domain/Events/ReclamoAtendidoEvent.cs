using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class ReclamoAtendidoEvent: ReclamoEventBase
    {
        public ReclamoAtendidoEvent(string ticketId): base(ticketId)
        {
            
        }
    }
}
