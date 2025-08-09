using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.Events;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CerrarReclamo
{
    public class CerrarReclamoCommandHandler : IRequestHandler<CerrarReclamoCommand, bool>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly IHistorialEstadoRepository _historialEstadoRepository;
        private readonly IMediator _mediator;

        public CerrarReclamoCommandHandler(
            IReclamoRepository reclamoRepository,
            IUsuarioService usuarioService,
            IHistorialEstadoRepository historialEstadoRepository,
            IMediator mediator)
        {
            _reclamoRepository = reclamoRepository ?? throw new ArgumentNullException(nameof(reclamoRepository));
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
            _historialEstadoRepository = historialEstadoRepository ?? throw new ArgumentNullException(nameof(historialEstadoRepository));
            _mediator = mediator;
        }

        public async Task<bool> Handle(CerrarReclamoCommand request, CancellationToken cancellationToken)
        {
            var usuarioActual = await _usuarioService.GetCurrentUserAsync();

            if (usuarioActual.NombreRol != "Administrador" && usuarioActual.NombreRol != "AtencionCliente")
                throw new ApplicationException("No tienes permisos para cerrar este reclamo");

            var reclamo = await _reclamoRepository.GetByTicketAsync(request.TicketId);
            if (reclamo == null)
                throw new ApplicationException($"El reclamo con ticket {request.TicketId} no existe");

            string estadoAnterior = reclamo.Estado;
            if (estadoAnterior != ReclamoEstado.Atendido)
            {
                throw new ApplicationException($"Solo se pueden cerrar reclamos en estado Atendido. Estado actual: {estadoAnterior}");
            }

            reclamo.Cerrar();

            await _reclamoRepository.UpdateAsync(reclamo);

            var historial = new HistorialEstado(
                request.TicketId,
                usuarioActual.UsuarioId,
                estadoAnterior,
                ReclamoEstado.Cerrado,
                "El reclamo ha sido cerrado."
            );

            await _historialEstadoRepository.CreateAsync(historial);

            await _mediator.Publish(new ReclamoCerradoEvent(reclamo.TicketId), cancellationToken);

            await _mediator.Publish(new EnviarEncuestaEvent(reclamo.TicketId), cancellationToken);

            return true;
        }
    }
}
