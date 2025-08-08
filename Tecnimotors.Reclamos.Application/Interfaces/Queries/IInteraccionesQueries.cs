using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.Application.Interfaces.Queries
{
    public interface IInteraccionesQueries
    {
        Task<IEnumerable<InteraccionDto>> GetInteraccionesPorReclamoAsync(string ticketId);
        Task<InteraccionDto> GetInteraccionPorIdAsync(int interaccionId);
    }
}
