using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.Events;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.AtenderReclamo
{
    public class AtenderReclamoCommandHandler : IRequestHandler<AtenderReclamoCommand>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IHistorialEstadoRepository _historialEstadoRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IMediator _mediator;
        public AtenderReclamoCommandHandler(
            IReclamoRepository reclamoRepository,
            IHistorialEstadoRepository historialEstadoRepository,
            IUsuarioService usuarioService,
            IMediator mediator)
        {
            _reclamoRepository = reclamoRepository;
            _historialEstadoRepository = historialEstadoRepository;
            _usuarioService = usuarioService;
            _mediator = mediator;
        }

        public async Task Handle(AtenderReclamoCommand request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            var reclamo = await _reclamoRepository.GetByTicketAsync(request.TicketId);
            if (reclamo == null)
                throw new ApplicationException($"El reclamo con ticket {request.TicketId} no existe");

            if (usuarioActual.NombreRol == "JefeArea")
            {
                bool usuarioAsignado = await _reclamoRepository.VerificarUsuarioAsignadoAsync(
                    request.TicketId, usuarioActual.UsuarioId);

                if (!usuarioAsignado)
                    throw new ApplicationException("No tienes permisos para atender este reclamo");
            }
            else if (usuarioActual.NombreRol != "Administrador")
            {
                throw new ApplicationException("No tienes permisos para atender este reclamo");
            }

            string estadoAnterior = reclamo.Estado;
            if (estadoAnterior == ReclamoEstado.Atendido ||
                estadoAnterior == ReclamoEstado.Cerrado ||
                estadoAnterior == ReclamoEstado.Rechazado)
            {
                throw new ApplicationException($"No se puede atender un reclamo que ya está en estado {estadoAnterior}");
            }

            reclamo.MarcarComoAtendido();

            await _reclamoRepository.UpdateAsync(reclamo);

            var historial = new HistorialEstado(
                reclamo.TicketId,
                usuarioActual.UsuarioId,
                estadoAnterior,
                ReclamoEstado.Atendido,
                "El reclamo ha sido atendido."
            );

            await _historialEstadoRepository.CreateAsync(historial);

            _ = _mediator.Publish(new ReclamoAtendidoEvent(reclamo.TicketId), cancellationToken);
        }
    }
}
