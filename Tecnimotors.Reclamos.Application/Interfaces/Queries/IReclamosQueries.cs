using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Interfaces.Queries
{
    public interface IReclamosQueries
    {
        Task<ReclamoDetalleCompletoDto> GetReclamoDetalleCompletoAsync(string ticketId);
        Task<PaginatedList<ReclamoListadoDto>> GetReclamosPaginadosAsync(
        int pageNumber,
        int pageSize,
        string ticketId = "",
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        string estado = "",
        string prioridad = "",
        Usuario usuarioActual = null);
    }
}
