using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Encuestas.Commands.ResponderEncuestas
{
    public class ResponderEncuestaCommandHandler : IRequestHandler<ResponderEncuestaCommand, ResponderEncuestaResponse>
    {
        private readonly IEncuestaRepository _encuestaRepository;
        private readonly ILogger<ResponderEncuestaCommandHandler> _logger;

        public ResponderEncuestaCommandHandler(
            IEncuestaRepository encuestaRepository,
            ILogger<ResponderEncuestaCommandHandler> logger)
        {
            _encuestaRepository = encuestaRepository;
            _logger = logger;
        }

        public async Task<ResponderEncuestaResponse> Handle(
            ResponderEncuestaCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Buscar encuesta
                var encuesta = await _encuestaRepository.GetEncuestaByTokenAsync(request.Token);
                if (encuesta == null)
                {
                    return new ResponderEncuestaResponse
                    {
                        Exitoso = false,
                        Mensaje = "Encuesta no encontrada"
                    };
                }

                // Validar estado
                if (encuesta.EstadoEncuesta != "ENVIADA")
                {
                    return new ResponderEncuestaResponse
                    {
                        Exitoso = false,
                        Mensaje = encuesta.EstadoEncuesta == "RESPONDIDA"
                            ? "Esta encuesta ya fue respondida"
                            : "Esta encuesta no está disponible"
                    };
                }

                // Validar vigencia
                if (encuesta.FechaVencimiento < DateTime.Now)
                {
                    await _encuestaRepository.UpdateEstadoEncuestaAsync(encuesta.Id, "VENCIDA", null);

                    return new ResponderEncuestaResponse
                    {
                        Exitoso = false,
                        Mensaje = "Esta encuesta ha vencido"
                    };
                }

                // Obtener preguntas
                var preguntas = await _encuestaRepository.GetPreguntasByConfiguracionAsync(encuesta.ConfiguracionEncuestaId);

                // Guardar respuestas
                decimal? isgCalculado = null;
                foreach (var pregunta in preguntas)
                {
                    if (request.Respuestas.TryGetValue(pregunta.Codigo, out var valor))
                    {
                        var respuesta = new RespuestasEncuesta
                        {
                            EncuestaEnviadaId = encuesta.Id,
                            PreguntaId = pregunta.Id,
                            ValorLikert = valor,
                            FechaRespuesta = DateTime.Now
                        };

                        await _encuestaRepository.CreateRespuestaAsync(respuesta);

                        // Si es P10, guardar para ISG
                        if (pregunta.Codigo == "P10")
                        {
                            isgCalculado = valor;
                        }
                    }
                }

                // Guardar comentario si existe
                if (!string.IsNullOrWhiteSpace(request.Comentario))
                {
                    var comentario = new ComentariosEncuesta
                    {
                        EncuestaEnviadaId = encuesta.Id,
                        TipoComentario = "GENERAL",
                        Comentario = request.Comentario,
                        FechaComentario = DateTime.Now
                    };

                    await _encuestaRepository.CreateComentarioAsync(comentario);
                }

                // Actualizar encuesta
                encuesta.EstadoEncuesta = "RESPONDIDA";
                encuesta.FechaRespuesta = DateTime.Now;
                encuesta.IPRespuesta = request.IpAddress;
                encuesta.DispositivoRespuesta = ParseUserAgent(request.UserAgent);

                await _encuestaRepository.UpdateEstadoEncuestaAsync(
                    encuesta.Id,
                    "RESPONDIDA",
                    DateTime.Now);

                _logger.LogInformation($"Encuesta respondida exitosamente. Token: {request.Token}");

                return new ResponderEncuestaResponse
                {
                    Exitoso = true,
                    Mensaje = "Gracias por responder nuestra encuesta",
                    ISGCalculado = isgCalculado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar respuestas de encuesta");

                return new ResponderEncuestaResponse
                {
                    Exitoso = false,
                    Mensaje = "Error al procesar sus respuestas"
                };
            }
        }

        private string ParseUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "Desconocido";

            if (userAgent.Contains("Mobile")) return "Mobile";
            if (userAgent.Contains("Tablet")) return "Tablet";
            return "Desktop";
        }
    }
}
