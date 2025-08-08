using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public class HistorialEstado
    {
        public int HistorialId { get; private set; }
        public string TicketId { get; private set; }
        public int UsuarioId { get; private set; }
        public string EstadoAnterior { get; private set; }
        public string EstadoNuevo { get; private set; }
        public string Comentario { get; private set; }
        public DateTime FechaRegistro { get; private set; }

        protected HistorialEstado()
        {
        }

        public HistorialEstado(string ticketId, int usuarioId, string estadoAnterior, string estadoNuevo, string comentario)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket no puede estar vacío", nameof(ticketId));

            if (string.IsNullOrWhiteSpace(estadoNuevo))
                throw new ArgumentException("El nuevo estado no puede estar vacío", nameof(estadoNuevo));

            TicketId = ticketId;
            UsuarioId = usuarioId;
            EstadoAnterior = estadoAnterior;
            EstadoNuevo = estadoNuevo;
            Comentario = comentario;
            FechaRegistro = DateTime.UtcNow;
        }
    }
}
