using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.Events;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.RechazarReclamo
{
    internal class RechazarReclamoCommandHandler : IRequestHandler<RechazarReclamoCommand, bool>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IHistorialEstadoRepository _historialEstadoRepository;
        private readonly IMediator _mediator;

        public RechazarReclamoCommandHandler(
            IReclamoRepository reclamoRepository, 
            IUsuarioService usuarioService, 
            IHistorialEstadoRepository historialEstadoRepository,
            IMediator mediator)
        {
            _reclamoRepository = reclamoRepository;
            _usuarioService = usuarioService;
            _historialEstadoRepository = historialEstadoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(RechazarReclamoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MotivoRechazo))
                throw new ApplicationException("Debe proporcionar un motivo para rechazar el reclamo");

            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            if (usuarioActual.NombreRol != "Administrador" && usuarioActual.NombreRol != "JefeArea")
                throw new ApplicationException("No tienes permisos para rechazar este reclamo");

            var reclamo = await _reclamoRepository.GetByTicketAsync(request.TicketId);
            if (reclamo == null)
                throw new ApplicationException($"El reclamo con ticket {request.TicketId} no existe");

            if (usuarioActual.NombreRol == "JefeArea")
            {
                bool usuarioAsignado = await _reclamoRepository.VerificarUsuarioAsignadoAsync(
                    request.TicketId, usuarioActual.UsuarioId);

                if (!usuarioAsignado)
                    throw new ApplicationException("No tienes permisos para rechazar este reclamo");
            }
            else if (usuarioActual.NombreRol != "Administrador")
            {
                throw new ApplicationException("No tienes permisos para rechazar este reclamo");
            }

            string estadoAnterior = reclamo.Estado;
            if (estadoAnterior == ReclamoEstado.Cerrado || estadoAnterior == ReclamoEstado.Rechazado)
            {
                throw new ApplicationException($"No se puede rechazar un reclamo que ya está en estado {estadoAnterior}");
            }

            reclamo.Rechazar(request.MotivoRechazo);

            await _reclamoRepository.UpdateAsync(reclamo);

            var historial = new HistorialEstado(
                request.TicketId,
                usuarioActual.UsuarioId,
                estadoAnterior,
                ReclamoEstado.Rechazado,
                $"Reclamo rechazado. Motivo: {request.MotivoRechazo}"
            );

            await _historialEstadoRepository.CreateAsync(historial);

            _ = _mediator.Publish(new ReclamoRechazadoEvent(
                reclamo.TicketId,
                usuarioActual.UsuarioId),
                cancellationToken);

            return true;

        }
    }
}
