using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public class Asignacion
    {
        public string TicketId { get; private set; }
        public int UsuarioId { get; private set; }

        protected Asignacion() { }

        public Asignacion(string ticketId, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket del reclamo no puede estar vacío", nameof(ticketId));

            if (usuarioId <= 0)
                throw new ArgumentException("El ID del usuario no puede ser cero o negativo", nameof(usuarioId));

            TicketId = ticketId;
            UsuarioId = usuarioId;
        }
    }
}
