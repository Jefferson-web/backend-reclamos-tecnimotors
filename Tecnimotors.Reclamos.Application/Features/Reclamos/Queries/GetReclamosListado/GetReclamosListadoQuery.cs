using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Models;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado
{
    public class GetReclamosListadoQuery : IRequest<PaginatedList<ReclamoListadoDto>>
    {
        public string TicketId { get; set; } = string.Empty;
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
