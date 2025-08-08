using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class ReclamoRegistradoEvent : ReclamoEventBase
    {
        public ReclamoRegistradoEvent(string ticketId): base(ticketId)
        {
            
        }
    }
}
