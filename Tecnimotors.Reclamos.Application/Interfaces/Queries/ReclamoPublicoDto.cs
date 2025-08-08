using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Interfaces.Queries
{
    public class ReclamoPublicoDto
    {
        // Información básica del reclamo
        public string TicketId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimaModificacion { get; set; }

        // Información del tipo de reclamo
        public string TipoReclamo { get; set; }

        // Detalle del reclamo
        public string Descripcion { get; set; }

        // Correo para notificaciones
        public string Correo { get; set; }

        // Historial de estados
        public List<HistorialEstadoDto> Historial { get; set; } = new List<HistorialEstadoDto>();
    }

    public class HistorialEstadoDto
    {
        public string Estado { get; set; }
        public string EstadoTexto { get; set; }
        public string EstadoAnterior { get; set; }
        public string EstadoAnteriorTexto { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
    }
}
