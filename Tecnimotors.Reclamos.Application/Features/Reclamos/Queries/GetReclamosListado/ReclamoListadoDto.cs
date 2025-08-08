using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado
{
    public class AsignacionDto
    {
        public string TicketId { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string RolNombre { get; set; }
    }

    public class AsignacionResumenDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellidos}";
        public string RolNombre { get; set; }
    }

    public class ReclamoListadoDto
    {
        public string TicketId { get; set; }
        public string Cliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public string MotivoNombre { get; set; }
        public int DiasAbierto { get; set; }
        public DateTime? UltimaModificacion { get; set; }
        public List<AsignacionResumenDto> Asignaciones { get; set; } = new List<AsignacionResumenDto>();
    }
}
