using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.EventHandlers
{
    public class EnviarCorreoReclamoNuevaInteraccionHandler : INotificationHandler<NuevaInteraccionEvent>
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IReclamosQueries _reclamosQueries;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService;
        public EnviarCorreoReclamoNuevaInteraccionHandler(
            IUsuarioService usuarioService,
            IReclamosQueries reclamosQueries,
            IUsuarioRepository usuarioRepository,
            IEmailService emailService)
        {
            _usuarioService = usuarioService;
            _reclamosQueries = reclamosQueries;
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }
        public async Task Handle(NuevaInteraccionEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var reclamo = await _reclamosQueries.GetReclamoDetalleCompletoAsync(notification.TicketId);
                if (reclamo == null) return;

                var usuario = await _usuarioRepository.GetByIdAsync(notification.UsuarioId);
                if (usuario == null) return;

                await sendEmailAsync(reclamo, usuario);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task sendEmailAsync(ReclamoDetalleCompletoDto reclamo, Usuario usuario)
        {
            try
            {
                var recipients = await GetDestinatariosInternosAsync(reclamo);
                if (recipients.P.Any() ||
                    recipients.CC.Any() ||
                    recipients.CCO.Any())
                {
                    var parametrosInternos = new Dictionary<string, string>
                    {
                        { "ticket_id", reclamo.TicketId },
                        { "cliente", reclamo.Cliente },
                        { "estado", reclamo.Estado },
                        { "estado_class", getStatusClass(reclamo.Estado) },
                        { "year", DateTime.Now.Year.ToString() },
                        { "usuario", $"{usuario.Nombre} {usuario.Apellidos}" }
                    };

                    await _emailService.SendEmailUsingTemplateToMultipleRecipientsAsync(
                        recipients.P,
                        recipients.CC,
                        recipients.CCO,
                        "INT_NUEVA_INTERACCION",
                        parametrosInternos
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        string getStatusClass(string estado)
        {
            switch (estado.ToLower())
            {
                case "registrado":
                    return "registered";
                case "enproceso":
                    return "in-process";
                case "atendido":
                    return "attended";
                case "cerrado":
                    return "closed";
                case "rechazado":
                    return "rejected";
                default:
                    throw new ArgumentException();
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