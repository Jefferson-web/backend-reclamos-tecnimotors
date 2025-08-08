using MediatR;
using System;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.EventHandlers
{
    public class EnviarCorreoReclamoRegistradoHandler : INotificationHandler<ReclamoRegistradoEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IReclamosQueries _reclamosQueries;
        private readonly IUsuarioRepository _usuarioRepository;

        public EnviarCorreoReclamoRegistradoHandler(
            IEmailService emailService,
            IReclamosQueries reclamosQueries,
            IUsuarioRepository usuarioRepository)
        {
            _emailService = emailService;
            _reclamosQueries = reclamosQueries;
            _usuarioRepository = usuarioRepository;
        }

        public async Task Handle(ReclamoRegistradoEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var reclamo = await _reclamosQueries.GetReclamoDetalleCompletoAsync(notification.TicketId);
                if (reclamo == null) return;

                await sendEmailAsync(reclamo);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task sendEmailAsync(ReclamoDetalleCompletoDto reclamo)
        {
            var parametrosBase = new Dictionary<string, string>
            {
                { "ticket_id", reclamo.TicketId },
                { "cliente", reclamo.Cliente },
                { "nombres", reclamo.Nombres },
                { "apellidos", reclamo.Apellidos },
                { "estado", reclamo.Estado },
                { "year", DateTime.Now.Year.ToString() },
                { "url_sistema", "https://reclamos.tecnimotorseirl.com" }
            };

            if (!string.IsNullOrEmpty(reclamo.Correo))
            {
                try
                {
                    await _emailService.SendEmailUsingTemplateAsync(
                        reclamo.Correo, 
                        "CLI_RECLAMO_REGISTRADO",
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
                    parametrosInternos.Add("prioridad", reclamo.Prioridad);
                    parametrosInternos.Add("telefono", reclamo.Telefono);
                    parametrosInternos.Add("correo", reclamo.Correo);
                    parametrosInternos.Add("prioridad_class", $"priority-{reclamo.Prioridad.ToLower()}");
                    parametrosInternos.Add("fecha_creacion", reclamo.FechaCreacion.ToString("dd/MM/yyyy"));
                    parametrosInternos.Add("usuario_registro", reclamo.UsuarioNombre);
                    parametrosInternos.Add("motivo", reclamo.MotivoNombre);

                    await _emailService.SendEmailUsingTemplateToMultipleRecipientsAsync(
                        recipients.P,
                        recipients.CC,
                        recipients.CCO,
                        "INT_RECLAMO_REGISTRADO",
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
