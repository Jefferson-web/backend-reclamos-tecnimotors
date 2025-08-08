using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate;
using Tecnimotors.Reclamos.Domain.Events;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.EventHandlers
{
    public class EnviarEncuestaSatisfaccionHandler : INotificationHandler<EnviarEncuestaEvent>
    {
        private readonly IEncuestaRepository _encuestaRepository;
        private readonly IReclamosQueries _reclamosQueries;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EnviarEncuestaSatisfaccionHandler> _logger;

        public EnviarEncuestaSatisfaccionHandler(
            IEncuestaRepository encuestaRepository, 
            IReclamosQueries reclamosQueries,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<EnviarEncuestaSatisfaccionHandler> logger)
        {
            _encuestaRepository = encuestaRepository;
            _reclamosQueries = reclamosQueries;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Handle(EnviarEncuestaEvent notification, CancellationToken cancellationToken)
        {
            var existeEncuesta = await _encuestaRepository.ExisteEncuestaPorTicketAsync(notification.TicketId);
            if (existeEncuesta)
            {
                _logger.LogError("Ya existe una encuesta para este ticket");
                return;
            }

            var reclamo = await _reclamosQueries.GetReclamoDetalleCompletoAsync(notification.TicketId);
            if (reclamo == null || string.IsNullOrEmpty(reclamo.Correo))
            {
                _logger.LogError("No se encontró el reclamo o no tiene email asociado");
                return;
            }

            var configuracion = await _encuestaRepository.GetConfiguracionActivaAsync("POST_CIERRE");
            if (configuracion == null)
            {
                _logger.LogError("No hay configuración de encuesta activa");
                return;
            }

            var encuesta = new EncuestasEnviadas
            {
                TicketId = notification.TicketId,
                ConfiguracionEncuestaId = configuracion.Id,
                TokenAcceso = Guid.NewGuid(),
                FechaEnvio = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(configuracion.DiasVigencia),
                EstadoEncuesta = "ENVIADA",
                EmailEnviado = reclamo.Correo
            };


            encuesta.Id = await _encuestaRepository.CreateEncuestaEnviadaAsync(encuesta);

            var urlBase = _configuration["UrlBase"];
            var urlEncuesta = $"{urlBase}/publico/encuesta/{encuesta.TokenAcceso}";

            var parametros = new Dictionary<string, string>
            {
                { "nombres", reclamo.Nombres },
                { "apellidos", reclamo.Apellidos },
                { "ticket_id", reclamo.TicketId },
                { "url_encuesta", urlEncuesta },
                { "fecha_cierre", reclamo.FechaCierre.HasValue ? reclamo.FechaCierre.Value.ToString("dd-MM-yyyy") : string.Empty },
                { "fecha_vencimiento", encuesta.FechaVencimiento.ToString("dd-MM-yyyy") },
                { "year", DateTime.Now.Year.ToString() }
            };


            try
            {
                await _emailService.SendEmailUsingTemplateAsync(
                reclamo.Correo,
                "CLI_ENCUESTA_SATISFACCION",
                parametros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
