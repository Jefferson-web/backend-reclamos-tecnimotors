using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public class Interaccion
    {
        public int InteraccionId { get; private set; }
        public string TicketId { get; private set; }
        public int UsuarioId { get; private set; }
        public string Mensaje { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        private readonly List<InteraccionArchivo> _archivos;
        public IReadOnlyCollection<InteraccionArchivo> Archivos => _archivos;

        protected Interaccion()
        {
            _archivos = new List<InteraccionArchivo>();
        }

        public Interaccion(string ticketId, int usuarioId, string mensaje)
            : this()
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket del reclamo no puede estar vacío", nameof(ticketId));

            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío", nameof(mensaje));

            TicketId = ticketId;
            UsuarioId = usuarioId;
            Mensaje = mensaje;
            FechaRegistro = DateTime.UtcNow;
        }

        public Interaccion(int interaccionId, string ticketId, int usuarioId, string mensaje, DateTime fechaRegistro, DateTime fechaModificacion)
            : this()
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket del reclamo no puede estar vacío", nameof(ticketId));

            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío", nameof(mensaje));

            InteraccionId = interaccionId;
            TicketId = ticketId;
            UsuarioId = usuarioId;
            Mensaje = mensaje;
            FechaRegistro = fechaRegistro;
            FechaModificacion = fechaModificacion;
        }

        public void EditarMensaje(string nuevoMensaje, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(nuevoMensaje))
                throw new ArgumentException("El mensaje no puede estar vacío", nameof(nuevoMensaje));

            // Validar que solo el usuario que creó la interacción pueda editarla
            if (usuarioId != UsuarioId)
                throw new InvalidOperationException("Solo el autor puede editar el mensaje");

            if (Mensaje == nuevoMensaje)
                return; // No hay cambios, evitamos actualizar fechas

            Mensaje = nuevoMensaje;
            FechaModificacion = DateTime.UtcNow;
        }

        public InteraccionArchivo AgregarArchivo(int archivoId)
        {
            // Verificar si el archivo ya está asociado
            var archivoExistente = _archivos.FirstOrDefault(a => a.ArchivoId == archivoId);
            if (archivoExistente != null)
                return archivoExistente;

            var interaccionArchivo = new InteraccionArchivo(InteraccionId, archivoId);
            _archivos.Add(interaccionArchivo);

            return interaccionArchivo;
        }

        public void QuitarArchivo(int archivoId)
        {
            var archivo = _archivos.FirstOrDefault(a => a.ArchivoId == archivoId);
            if (archivo != null)
            {
                _archivos.Remove(archivo);
            }
        }
    }
}
