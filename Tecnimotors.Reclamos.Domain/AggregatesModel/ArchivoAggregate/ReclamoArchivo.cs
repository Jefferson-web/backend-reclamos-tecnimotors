using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate
{
    public class ReclamoArchivo
    {
        public string TicketId { get; private set; }
        public int ArchivoId { get; private set; }

        protected ReclamoArchivo() { }

        public ReclamoArchivo(string ticketId, int archivoId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket del reclamo no puede estar vacío", nameof(ticketId));

            TicketId = ticketId;
            ArchivoId = archivoId;
        }
    }
}
