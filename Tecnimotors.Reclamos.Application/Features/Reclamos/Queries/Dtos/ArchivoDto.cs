using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos
{
    public class ArchivoDto
    {
        public int ArchivoId { get; set; }
        public string NombreOriginal { get; set; }
        public string TipoMime { get; set; }
        public long TamanoByte { get; set; }
        public DateTime FechaSubida { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
    }
}
