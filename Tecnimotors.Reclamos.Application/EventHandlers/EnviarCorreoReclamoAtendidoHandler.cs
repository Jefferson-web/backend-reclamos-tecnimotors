using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.EventHandlers
{
    public class EnviarCorreoReclamoAtendidoHandler : INotificationHandler<ReclamoAtendidoEvent>
    {
        private readonly IMediator _mediator;
        private readonly IReclamosQueries _reclamoQueries;
        private readonly IEmailService _emailService;
        private readonly IUsuarioRepository _usuarioRepository;
        public EnviarCorreoReclamoAtendidoHandler(
            IMediator mediator,
            IReclamosQueries reclamosQueries,
            IEmailService emailService,
            IUsuarioRepository usuarioRepository)
        {
            _mediator = mediator;
            _reclamoQueries = reclamosQueries;
            _emailService = emailService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task Handle(ReclamoAtendidoEvent notification, CancellationToken cancellationToken)
        {
            var reclamo = await _reclamoQueries.GetReclamoDetalleCompletoAsync(notification.TicketId);
            if (reclamo == null) return;


            await sendEmailAsync(reclamo);
        }

        async Task sendEmailAsync(ReclamoDetalleCompletoDto reclamo)
        {
            var parametrosBase = new Dictionary<string, string>
            {
                { "nombres", reclamo.Nombres },
                { "apellidos", reclamo.Apellidos },
                { "ticket_id", reclamo.TicketId },
                { "year", DateTime.Now.Year.ToString() }
            };

            if (!string.IsNullOrEmpty(reclamo.Correo))
            {
                try
                {
                    await _emailService.SendEmailUsingTemplateAsync(
                        reclamo.Correo,
                        "CLI_RECLAMO_RECHAZADO",
                        parametrosBase);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            try
            {
                var recipients = await GetDestinatariosInternosAsync(reclamo);
                if (recipients.P.Any() ||
                    recipients.CC.Any() ||
                    recipients.CCO.Any())
                {
                    var parametrosInternos = new Dictionary<string, string>(parametrosBase);
                    parametrosInternos.Add("cliente", reclamo.Cliente);
                    parametrosInternos.Add("telefono", reclamo.Telefono);
                    parametrosInternos.Add("correo", reclamo.Correo);
                    parametrosInternos.Add("prioridad", reclamo.Prioridad);
                    parametrosInternos.Add("prioridad_class", $"priority-{reclamo.Prioridad.ToLower()}");

                    await _emailService.SendEmailUsingTemplateToMultipleRecipientsAsync(
                        recipients.P,
                        recipients.CC,
                        recipients.CCO,
                        "INT_RECLAMO_ATENDIDO",
                        parametrosInternos
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<Recipients> GetDestinatariosInternosAsync(ReclamoDetalleCompletoDto reclamo)
        {
            var recipients = new Recipients();

            var usuariosAtencionCliente = await _usuarioRepository.GetUsuariosByRol("AtencionCliente");
            recipients.P.AddRange(usuariosAtencionCliente.Select(u => u.Email));

            if (reclamo.Asignaciones != null && reclamo.Asignaciones.Any())
            {
                var usuariosAsignados = reclamo.Asignaciones.Select(a => a.UsuarioEmail);
                recipients.CC.AddRange(usuariosAsignados);
            }

            var administradores = await _usuarioRepository.GetUsuariosByRol("Administrador");
            recipients.CCO.AddRange(administradores.Select(u => u.Email));

            return recipients;
        }
    }
}
