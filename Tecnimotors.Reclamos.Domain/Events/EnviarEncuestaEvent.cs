using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class EnviarEncuestaEvent: ReclamoEventBase
    {
        public EnviarEncuestaEvent(string ticketId): base(ticketId)
        {
            
        }
    }
}
