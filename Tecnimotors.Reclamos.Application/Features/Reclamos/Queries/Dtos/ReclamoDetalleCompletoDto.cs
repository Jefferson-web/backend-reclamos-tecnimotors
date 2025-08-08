using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos
{
    public class ReclamoDetalleCompletoDto
    {
        // Información básica del reclamo
        public string TicketId { get; set; }
        public string Cliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => $"{Nombres} {Apellidos}";
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Detalle { get; set; }
        public string Estado { get; set; }
        public string Prioridad { get; set; }
        public string MotivoRechazo { get; set; }

        // Motivo
        public int MotivoId { get; set; }
        public string MotivoNombre { get; set; }
        public string MotivoDescripcion { get; set; }

        // Usuario creador
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioApellidos { get; set; }
        public string UsuarioEmail { get; set; }
        public int UsuarioRolId { get; set; }
        public string UsuarioRolNombre { get; set; }

        // Ubicación
        public string DepartamentoId { get; set; }
        public string DepartamentoNombre { get; set; }
        public string ProvinciaId { get; set; }
        public string ProvinciaNombre { get; set; }
        public string DistritoId { get; set; }
        public string DistritoNombre { get; set; }

        // Fechas
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public DateTime? UltimaModificacion { get; set; }

        // Campos calculados
        public int DiasAbierto { get; set; }
        public string EstadoActual => FechaCierre.HasValue ? "Cerrado" : "Abierto";

        // Colecciones relacionadas
        public List<AsignacionDetalleDto> Asignaciones { get; set; } = new List<AsignacionDetalleDto>();
        public List<ArchivoDetalleDto> Archivos { get; set; } = new List<ArchivoDetalleDto>();
        public List<InteraccionDetalleDto> Interacciones { get; set; } = new List<InteraccionDetalleDto>();

        public List<EstadoHistorialDto> HistorialEstados { get; set; } = new List<EstadoHistorialDto>();
    }

    public class AsignacionDetalleDto
    {
        public string TicketId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioApellidos { get; set; }
        public string UsuarioNombreCompleto => $"{UsuarioNombre} {UsuarioApellidos}";
        public string UsuarioEmail { get; set; }
        public int UsuarioRolId { get; set; }
        public string UsuarioRolNombre { get; set; }
    }

    public class EstadoHistorialDto
    {
        public int HistorialId { get; set; }
        public string TicketId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioApellidos { get; set; }
        public string UsuarioNombreCompleto => $"{UsuarioNombre} {UsuarioApellidos}";
        public string EstadoAnterior { get; set; }
        public string EstadoNuevo { get; set; }
        public string Comentario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class ArchivoDetalleDto
    {
        public int ArchivoId { get; set; }
        public string NombreOriginal { get; set; }
        public string NombreSistema { get; set; }
        public string Extension { get; set; }
        public string TipoMime { get; set; }
        public string RutaAlmacenamiento { get; set; }
        public int TamanoByte { get; set; }
        public string TamanoFormateado => FormatearTamano(TamanoByte);
        public DateTime FechaSubida { get; set; }

        private string FormatearTamano(int bytes)
        {
            string[] sufijos = { "B", "KB", "MB", "GB", "TB" };
            int contador = 0;
            decimal tamano = bytes;

            while (tamano >= 1024 && contador < sufijos.Length - 1)
            {
                tamano /= 1024;
                contador++;
            }

            return $"{Math.Round(tamano, 2)} {sufijos[contador]}";
        }
    }

    public class InteraccionDetalleDto
    {
        public int InteraccionId { get; set; }
        public string TicketId { get; set; }
        public string UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioApellidos { get; set; }
        public string UsuarioNombreCompleto => $"{UsuarioNombre} {UsuarioApellidos}";
        public string UsuarioRolNombre { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public List<ArchivoDetalleDto> Archivos { get; set; } = new List<ArchivoDetalleDto>();
    }
}
