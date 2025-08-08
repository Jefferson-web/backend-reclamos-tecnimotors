using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate
{
    public interface ILocalidadRepository
    {
        Task<Localidad> GetByIdAsync(int id);
        Task<IEnumerable<Localidad>> GetAllAsync();
        Task<int> CreateAsync(Localidad localidad);
        Task<bool> UpdateAsync(Localidad localidad);
        Task<bool> DeleteAsync(int id);
    }
}
