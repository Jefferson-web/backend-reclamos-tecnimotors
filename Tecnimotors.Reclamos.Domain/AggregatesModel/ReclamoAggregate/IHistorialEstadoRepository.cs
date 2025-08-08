using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public interface IHistorialEstadoRepository
    {
        Task<HistorialEstado> GetByIdAsync(int historialId);
        Task<IEnumerable<HistorialEstado>> GetByTicketIdAsync(string ticketId);
        Task<int> CreateAsync(HistorialEstado historialEstado);
    }
}
