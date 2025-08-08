using MediatR;
using Microsoft.AspNetCore.Http;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CrearReclamo
{
    public class CrearReclamoCommandHandler : IRequestHandler<CrearReclamoCommand, string>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFileService _fileService;
        private readonly IUsuarioService _usuarioService;
        private readonly IArchivoRepository _archivoRepository;
        private readonly IHistorialEstadoRepository _historialEstadoRepository;
        private readonly IMediator _mediator;

        public CrearReclamoCommandHandler(
            IReclamoRepository reclamoRepository, 
            IUsuarioRepository usuarioRepository,
            IFileService fileService,
            IUsuarioService usuarioService,
            IArchivoRepository archivoRepository,
            IHistorialEstadoRepository historialEstadoRepository,
            IMediator mediator)
        {
            _reclamoRepository = reclamoRepository;
            _usuarioRepository = usuarioRepository;
            _fileService = fileService;
            _usuarioService = usuarioService;
            _archivoRepository = archivoRepository;
            _historialEstadoRepository = historialEstadoRepository;
            _mediator = mediator;
        }

        public async Task<string> Handle(CrearReclamoCommand request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            string ticket = await _reclamoRepository.GenerarTicketAsync();

            var reclamo = new Reclamo(
                ticket,
                usuarioActual.UsuarioId,
                request.Cliente,
                request.Nombres,
                request.Apellidos,
                request.Telefono,
                request.Correo,
                request.Detalle,
                request.MotivoId,
                request.DepartamentoId,
                request.ProvinciaId,
                request.DistritoId,
                request.Prioridad
            );

            if (request.UsuariosAsignadosIds != null && request.UsuariosAsignadosIds.Any())
            {
                foreach (var usuarioId in request.UsuariosAsignadosIds)
                {
                    var usuarioAsignado = await _usuarioRepository.GetByIdAsync(usuarioId);
                    if (usuarioAsignado != null && usuarioAsignado.Activo)
                        reclamo.Asignar(usuarioAsignado);
                    else
                        Console.WriteLine("Usuario no encontrado.");
                }
            }

            await _reclamoRepository.AddAsync(reclamo);

            if (request.Archivos != null && request.Archivos.Any()) 
            {
                foreach (var archivoForm in request.Archivos)
                {
                    try
                    {
                        var archivo = await UploadFile(archivoForm);

                        int archivoId = await _archivoRepository.AddAsync(archivo);

                        await _archivoRepository.AsociarConReclamoAsync(
                            archivoId, 
                            reclamo.TicketId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            var historial = new HistorialEstado(
                ticket, 
                usuarioActual.UsuarioId, 
                null, 
                reclamo.Estado, 
                $"El reclamo ha sido registrado.");

            await _historialEstadoRepository.CreateAsync(historial);

            _ = _mediator.Publish(
                new ReclamoRegistradoEvent(reclamo.TicketId),
                cancellationToken);

            return reclamo.TicketId;
        }

        private async Task<Archivo> UploadFile(IFormFile archivoForm)
        {
            string rutaCompleta = await _fileService.SaveFileAsync(archivoForm, "Reclamos");

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

            return archivo;
        }
    }
}
