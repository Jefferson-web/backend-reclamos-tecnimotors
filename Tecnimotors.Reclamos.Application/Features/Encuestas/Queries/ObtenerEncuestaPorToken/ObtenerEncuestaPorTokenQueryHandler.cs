using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Encuestas.Queries.ObtenerEncuestaPorToken
{
    public class ObtenerEncuestaPorTokenQueryHandler : IRequestHandler<ObtenerEncuestaPorTokenQuery, EncuestaDto>
    {
        private readonly IEncuestaRepository _encuestaRepository;

        public ObtenerEncuestaPorTokenQueryHandler(IEncuestaRepository encuestaRepository)
        {
            _encuestaRepository = encuestaRepository;
        }

        public async Task<EncuestaDto> Handle(
            ObtenerEncuestaPorTokenQuery request,
            CancellationToken cancellationToken)
        {
            var encuesta = await _encuestaRepository.GetEncuestaByTokenAsync(request.Token);
            if (encuesta == null)
                return null;

            var preguntas = await _encuestaRepository.GetPreguntasByConfiguracionAsync(encuesta.ConfiguracionEncuestaId);

            var dto = new EncuestaDto
            {
                TicketId = encuesta.TicketId,
                EstadoEncuesta = encuesta.EstadoEncuesta,
                FechaVencimiento = encuesta.FechaVencimiento,
                Preguntas = preguntas.Select(p => new PreguntaDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    TextoPregunta = p.TextoPregunta,
                    Categoria = p.Categoria,
                    Orden = p.Orden,
                    Obligatoria = p.Obligatoria
                }).ToList()
            };

            // Validar si puede responder
            if (encuesta.EstadoEncuesta != "ENVIADA")
            {
                dto.PuedeResponder = false;
                dto.MensajeError = encuesta.EstadoEncuesta == "RESPONDIDA"
                    ? "Esta encuesta ya fue respondida"
                    : "Esta encuesta no está disponible";
            }
            else if (encuesta.FechaVencimiento < DateTime.Now)
            {
                dto.PuedeResponder = false;
                dto.MensajeError = "Esta encuesta ha vencido";
            }
            else
            {
                dto.PuedeResponder = true;
            }

            return dto;
        }
    }
}
