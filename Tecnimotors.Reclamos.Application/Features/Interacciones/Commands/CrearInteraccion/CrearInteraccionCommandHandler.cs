using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.Events;

namespace Tecnimotors.Reclamos.Application.Features.Interacciones.Commands.CrearInteraccion
{
    public class CrearInteraccionCommandHandler : IRequestHandler<CrearInteraccionCommand>
    {
        private readonly IInteraccionRepository _interaccionRepository;
        private readonly IArchivoRepository _archivoRepository;
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IFileService _fileService;
        private readonly IUsuarioService _usuarioService;
        private readonly IHistorialEstadoRepository _historialEstadoRepository;
        private readonly IMediator _mediator;
        public CrearInteraccionCommandHandler(
            IInteraccionRepository interaccionRepository,
            IArchivoRepository archivoRepository,
            IReclamoRepository reclamoRepository,
            IFileService fileService,
            IUsuarioService usuarioService,
            IHistorialEstadoRepository historialEstadoRepository,
            IMediator mediator)
        {
            _interaccionRepository = interaccionRepository;
            _archivoRepository = archivoRepository;
            _reclamoRepository = reclamoRepository;
            _fileService = fileService;
            _usuarioService = usuarioService;
            _historialEstadoRepository = historialEstadoRepository;
            _mediator = mediator;
        }

        public async Task Handle(CrearInteraccionCommand request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            var reclamo = await _reclamoRepository.GetByTicketAsync(request.TicketId);
            if (reclamo == null)
                throw new ApplicationException($"No existe un reclamo con el código {request.TicketId}");

            var interaccion = new Interaccion(
                request.TicketId,
                usuarioActual.UsuarioId,
                request.Mensaje
            );

            if (usuarioActual.NombreRol == "JefeArea")
            {
                bool usuarioAsignado = await _reclamoRepository.VerificarUsuarioAsignadoAsync(reclamo.TicketId, usuarioActual.UsuarioId);
                if (!usuarioAsignado)
                    throw new ApplicationException("No estás asignado a este reclamo");
            }
            else if (usuarioActual.NombreRol != "AtencionCliente")
            {
                throw new ApplicationException("No tienes permisos para agregar interacciones a este reclamo");
            }

            if (reclamo.Estado == ReclamoEstado.Cerrado || reclamo.Estado == ReclamoEstado.Rechazado)
            {
                throw new ApplicationException($"No se pueden agregar interacciones a un reclamo en estado {reclamo.Estado}");
            }

            var interaccionId = await _interaccionRepository.CreateAsync(interaccion);

            string estadoAnterior = reclamo.Estado;
            var interacciones = await _interaccionRepository.ObtenerInteracciones(reclamo.TicketId);

            if (estadoAnterior == ReclamoEstado.Registrado && interacciones.Count() == 1)
            {
                reclamo.EnProceso();

                var historial = new HistorialEstado(
                    request.TicketId,
                    usuarioActual.UsuarioId,
                    estadoAnterior,
                    ReclamoEstado.EnProceso,
                    "El reclamo está siendo atendido."
                );

                await _historialEstadoRepository.CreateAsync(historial);

                await _reclamoRepository.UpdateAsync(reclamo);
            }

            if (request.Archivos != null && request.Archivos.Count > 0)
            {
                var carpeta = $"Interacciones/{request.TicketId}";

                foreach (var archivoForm in request.Archivos)
                {
                    try
                    {
                        var rutaCompleta = await _fileService.SaveFileAsync(archivoForm, carpeta);

                        string nombreOriginal = archivoForm.FileName;
                        string extension = Path.GetExtension(nombreOriginal);
                        string tipoMime = archivoForm.ContentType;
                        int tamanoBytes = (int)archivoForm.Length;
                        string nombreSistema = Path.GetFileName(rutaCompleta);
                        string rutaAlmacenamiento = rutaCompleta;

                        var archivo = new Archivo(
                            nombreOriginal,
                            nombreSistema,
                            extension,
                            tipoMime,
                            rutaAlmacenamiento,
                            tamanoBytes
                        );

                        int nuevoArchivoId = await _archivoRepository.AddAsync(archivo);

                        if (nuevoArchivoId > 0)
                        {
                            await _interaccionRepository.AsociarArchivoAsync(interaccionId, nuevoArchivoId);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            _ = _mediator.Publish(
                new NuevaInteraccionEvent(reclamo.TicketId, usuarioActual.UsuarioId),
                cancellationToken);
        }
    }
}
