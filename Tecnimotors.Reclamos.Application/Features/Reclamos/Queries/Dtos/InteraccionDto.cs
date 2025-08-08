using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos
{
    public class InteraccionDto
    {
        public int InteraccionId { get; set; }
        public string TicketId { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<ArchivoDto> Archivos { get; set; }
    }
}
