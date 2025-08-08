using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Encuestas.Queries.ObtenerEncuestaPorToken
{
    public record ObtenerEncuestaPorTokenQuery : IRequest<EncuestaDto>
    {
        public Guid Token { get; init; }
    }

    public record EncuestaDto
    {
        public string TicketId { get; set; }
        public string EstadoEncuesta { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public List<PreguntaDto> Preguntas { get; set; } = new();
        public bool PuedeResponder { get; set; }
        public string MensajeError { get; set; }
    }

    public record PreguntaDto
    {
        public int Id { get; init; }
        public string Codigo { get; init; }
        public string TextoPregunta { get; init; }
        public string Categoria { get; init; }
        public int Orden { get; init; }
        public bool Obligatoria { get; init; }
    }
}
