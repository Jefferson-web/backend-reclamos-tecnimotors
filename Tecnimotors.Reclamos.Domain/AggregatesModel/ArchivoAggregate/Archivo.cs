using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate
{
    public class Archivo
    {
        public int ArchivoId { get; private set; }
        public string NombreOriginal { get; private set; }
        public string NombreSistema { get; private set; }
        public string Extension { get; private set; }
        public string TipoMime { get; private set; }
        public string RutaAlmacenamiento { get; private set; }
        public int TamanoBytes { get; private set; }
        public DateTime FechaSubida { get; private set; }

        protected Archivo() { }


        public Archivo(string nombreOriginal, string nombreSistema, string extension,
                     string tipoMime, string rutaAlmacenamiento, int tamanoBytes)
        {
            if (string.IsNullOrWhiteSpace(nombreOriginal))
                throw new ArgumentException("El nombre original no puede estar vacío", nameof(nombreOriginal));

            if (string.IsNullOrWhiteSpace(nombreSistema))
                throw new ArgumentException("El nombre del sistema no puede estar vacío", nameof(nombreSistema));

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("La extensión no puede estar vacía", nameof(extension));

            if (string.IsNullOrWhiteSpace(tipoMime))
                throw new ArgumentException("El tipo MIME no puede estar vacío", nameof(tipoMime));

            if (string.IsNullOrWhiteSpace(rutaAlmacenamiento))
                throw new ArgumentException("La ruta de almacenamiento no puede estar vacía", nameof(rutaAlmacenamiento));

            if (tamanoBytes <= 0)
                throw new ArgumentException("El tamaño debe ser mayor que cero", nameof(tamanoBytes));

            NombreOriginal = nombreOriginal;
            NombreSistema = nombreSistema;
            Extension = extension.ToLowerInvariant();
            TipoMime = tipoMime;
            RutaAlmacenamiento = rutaAlmacenamiento;
            TamanoBytes = tamanoBytes;
            FechaSubida = DateTime.UtcNow;
        }

        public Archivo(int archivoId, string nombreOriginal, string nombreSistema, string extension,
                     string tipoMime, string rutaAlmacenamiento, int tamanoBytes)
        {
            if (string.IsNullOrWhiteSpace(nombreOriginal))
                throw new ArgumentException("El nombre original no puede estar vacío", nameof(nombreOriginal));

            if (string.IsNullOrWhiteSpace(nombreSistema))
                throw new ArgumentException("El nombre del sistema no puede estar vacío", nameof(nombreSistema));

            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("La extensión no puede estar vacía", nameof(extension));

            if (string.IsNullOrWhiteSpace(tipoMime))
                throw new ArgumentException("El tipo MIME no puede estar vacío", nameof(tipoMime));

            if (string.IsNullOrWhiteSpace(rutaAlmacenamiento))
                throw new ArgumentException("La ruta de almacenamiento no puede estar vacía", nameof(rutaAlmacenamiento));

            if (tamanoBytes <= 0)
                throw new ArgumentException("El tamaño debe ser mayor que cero", nameof(tamanoBytes));

            ArchivoId = archivoId;
            NombreOriginal = nombreOriginal;
            NombreSistema = nombreSistema;
            Extension = extension.ToLowerInvariant();
            TipoMime = tipoMime;
            RutaAlmacenamiento = rutaAlmacenamiento;
            TamanoBytes = tamanoBytes;
            FechaSubida = DateTime.UtcNow;
        }
        private static string ObtenerTipoMime(string extension)
        {
            // Implementación simplificada, en un caso real podría consultar un mapa más completo
            return extension.ToLowerInvariant() switch
            {
                "pdf" => "application/pdf",
                "jpg" => "image/jpeg",
                "jpeg" => "image/jpeg",
                "png" => "image/png",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }

        public string ObtenerRutaCompleta(string rutaBase)
        {
            return Path.Combine(rutaBase, RutaAlmacenamiento);
        }
    }
}
