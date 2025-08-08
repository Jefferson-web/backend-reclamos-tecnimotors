using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Archivos.Queries.DownloadArchivo
{
    public class ArchivoDownloadDto
    {
        public int ArchivoId { get; set; }
        public string NombreOriginal { get; set; }
        public string NombreSistema { get; set; }
        public string Extension { get; set; }
        public string TipoMime { get; set; }
        public string RutaAlmacenamiento { get; set; }
        public int TamanoBytes { get; set; }
        public DateTime FechaSubida { get; set; }
    }

    public class ArchivoStreamResultDto
    {
        public byte[] Contenido { get; set; }
        public string NombreArchivo { get; set; }
        public string TipoContenido { get; set; }
    }
}
