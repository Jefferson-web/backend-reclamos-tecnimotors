using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate
{
    public interface IMotivoRepository
    {
        Task<IEnumerable<Motivo>> GetAllAsync();
        Task<Motivo> GetByIdAsync(int id);
        Task AddAsync(Motivo motivo);
        Task UpdateAsync(Motivo motivo);
        Task<bool> DeleteAsync(int id);
        Task<(IEnumerable<Motivo>, int)> GetPaginatedAsync(int pageIndex, int pageSize, string nombreFilter = null);
    }
}
