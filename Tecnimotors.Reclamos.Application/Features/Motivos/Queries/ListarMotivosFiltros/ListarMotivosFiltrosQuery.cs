using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivos;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivosFiltros
{
    public class ListarMotivosFiltrosQuery : IRequest<PaginatedList<GetMotivosQueryResponse>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Nombre { get; set; } = string.Empty;
    }
}
