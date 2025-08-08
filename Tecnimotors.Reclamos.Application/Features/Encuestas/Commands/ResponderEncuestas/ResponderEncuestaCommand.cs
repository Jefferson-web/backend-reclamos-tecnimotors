using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Encuestas.Commands.ResponderEncuestas
{
    public record ResponderEncuestaCommand : IRequest<ResponderEncuestaResponse>
    {
        public Guid Token { get; init; }
        public Dictionary<string, int> Respuestas { get; init; } = new();
        public string Comentario { get; init; }
        public string IpAddress { get; init; }
        public string UserAgent { get; init; }
    }

    public record ResponderEncuestaResponse
    {
        public bool Exitoso { get; init; }
        public string Mensaje { get; init; }
        public decimal? ISGCalculado { get; init; }
    }
}
