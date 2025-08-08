using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public class Reclamo
    {
        public string TicketId { get; private set; }
        public int UsuarioId { get; set; }
        public string Cliente { get; private set; }
        public string Nombres { get; private set; }
        public string Apellidos { get; private set; }
        public string Telefono { get; private set; }
        public string Correo { get; private set; }
        public string Detalle { get; private set; }
        public int MotivoId { get; private set; }
        public string Estado { get; private set; }
        public string DepartamentoId { get; private set; }
        public string ProvinciaId { get; private set; }
        public string DistritoId { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaCierre { get; private set; }
        public DateTime? UltimaModificacion { get; private set; }
        public string Prioridad { get; private set; }
        public string MotivoRechazo { get; private set; }
        public bool CierreAutomatico { get; private set; }


        private readonly List<Asignacion> _asignaciones;
        public IReadOnlyCollection<Asignacion> Asignaciones => _asignaciones;

        private readonly List<ReclamoArchivo> _archivos;
        public IReadOnlyCollection<ReclamoArchivo> Archivos => _archivos;

        private readonly List<Interaccion> _interacciones;
        public IReadOnlyCollection<Interaccion> Interacciones => _interacciones;

        // Constructor protegido para EF Core
        protected Reclamo()
        {
            _asignaciones = new List<Asignacion>();
            _archivos = new List<ReclamoArchivo>();
            _interacciones = new List<Interaccion>();
        }

        public Reclamo(string ticketId, int usuarioId, string cliente, string nombres, string apellidos, string telefono,
                       string correo, string detalle, int motivoId, string departamentoId,
                       string provinciaId, string distritoId, string prioridad)
            : this()
        {
            if (string.IsNullOrWhiteSpace(ticketId))
                throw new ArgumentException("El ticket no puede estar vacío", nameof(ticketId));

            TicketId = ticketId;
            UsuarioId = usuarioId;
            Cliente = cliente ?? throw new ArgumentException("El cliente no puede ser nulo", nameof(cliente));
            Nombres = nombres ?? throw new ArgumentException("Los nombres no pueden ser nulos", nameof(nombres));
            Apellidos = apellidos ?? throw new ArgumentException("Los apellidos no pueden ser nulos", nameof(apellidos));
            Telefono = telefono ?? throw new ArgumentException("El teléfono no puede ser nulo", nameof(telefono));
            Correo = correo;
            Detalle = detalle ?? throw new ArgumentException("El detalle no puede ser nulo", nameof(detalle));
            MotivoId = motivoId;
            Estado = ReclamoEstado.Registrado;
            DepartamentoId = departamentoId ?? throw new ArgumentException("El departamento no puede ser nulo", nameof(departamentoId));
            ProvinciaId = provinciaId ?? throw new ArgumentException("La provincia no puede ser nula", nameof(provinciaId));
            DistritoId = distritoId ?? throw new ArgumentException("El distrito no puede ser nulo", nameof(distritoId));
            Prioridad = prioridad;
            FechaCreacion = DateTime.UtcNow;
        }

        public void ActualizarEstado(string nuevoEstado)
        {
            if (string.IsNullOrWhiteSpace(nuevoEstado))
                throw new ArgumentException("El estado no puede estar vacío", nameof(nuevoEstado));

            Estado = nuevoEstado;
            UltimaModificacion = DateTime.UtcNow;

            // Si el estado es "CERRADO" o similar, actualizar la fecha de cierre
            if (nuevoEstado == "CERRADO" || nuevoEstado == "RESUELTO" || nuevoEstado == "FINALIZADO")
            {
                FechaCierre = DateTime.UtcNow;
            }
            else if (FechaCierre.HasValue) // Si se reabre un reclamo previamente cerrado
            {
                FechaCierre = null;
            }
        }

        public Asignacion Asignar(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var asignacionExistente = _asignaciones.FirstOrDefault(a => a.UsuarioId == usuario.UsuarioId);
            if (asignacionExistente != null)
                return asignacionExistente;

            var asignacion = new Asignacion(TicketId, usuario.UsuarioId);
            _asignaciones.Add(asignacion);

            return asignacion;
        }

        public Interaccion AgregarInteraccion(Usuario usuario, string mensaje)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (string.IsNullOrWhiteSpace(mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío", nameof(mensaje));

            var interaccion = new Interaccion(TicketId, usuario.UsuarioId, mensaje);
            _interacciones.Add(interaccion);
            UltimaModificacion = DateTime.UtcNow;

            return interaccion;
        }

        public ReclamoArchivo AgregarArchivo(Archivo archivo, Usuario usuario)
        {
            if (archivo == null)
                throw new ArgumentNullException(nameof(archivo));

            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var archivoExistente = _archivos.FirstOrDefault(a => a.ArchivoId == archivo.ArchivoId);
            if (archivoExistente != null)
                return archivoExistente;

            var reclamoArchivo = new ReclamoArchivo(TicketId, archivo.ArchivoId);
            _archivos.Add(reclamoArchivo);
            UltimaModificacion = DateTime.UtcNow;

            return reclamoArchivo;
        }

        public void QuitarArchivo(int archivoId)
        {
            var archivo = _archivos.FirstOrDefault(a => a.ArchivoId == archivoId);
            if (archivo != null)
            {
                _archivos.Remove(archivo);
                UltimaModificacion = DateTime.UtcNow;
            }
        }

        public void ActualizarInformacion(string telefono, string correo, string detalle)
        {
            if (!string.IsNullOrWhiteSpace(telefono))
                Telefono = telefono;

            Correo = correo; // Puede ser null

            if (!string.IsNullOrWhiteSpace(detalle))
                Detalle = detalle;

            UltimaModificacion = DateTime.UtcNow;
        }

        public void EnProceso()
        {
            Estado = ReclamoEstado.EnProceso;
            UltimaModificacion = DateTime.Now;
        }

        public void Cerrar()
        {
            Estado = ReclamoEstado.Cerrado;
            FechaCierre = DateTime.Now;
            UltimaModificacion = DateTime.Now;
        }

        public void Rechazar(string motivoRechazo)
        {
            Estado = ReclamoEstado.Rechazado;
            MotivoRechazo = motivoRechazo;
            UltimaModificacion = DateTime.Now;
        }

        public void MarcarComoAtendido()
        {
            Estado = ReclamoEstado.Atendido;
            UltimaModificacion = DateTime.Now;
        }

        public void CerrarAutomaticamente()
        {
            var fechaActual = DateTime.Now;
            UltimaModificacion = fechaActual;
            FechaCierre = fechaActual;
            CierreAutomatico = true;
            MotivoRechazo = "Cerrado automáticamente por inactividad de 3 meses";
        }
    }
}
