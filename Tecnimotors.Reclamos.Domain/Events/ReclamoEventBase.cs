using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Events
{
    public class ReclamoEventBase : INotification
    {
        public string TicketId { get; }
        public DateTime OcurridoEn { get; }
        public ReclamoEventBase(string ticketId)
        {
            TicketId = ticketId;
            OcurridoEn = DateTime.Now;
        }
    }
}
