using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate
{
    public interface IUbicacionRepository
    {
        Task<IEnumerable<Departamento>> GetDepartamentosAsync();
        Task<Departamento> GetDepartamentoByIdAsync(string id);
        Task<IEnumerable<Provincia>> GetProvinciasByDepartamentoIdAsync(string departamentoId);
        Task<Provincia> GetProvinciaByIdAsync(string id);
        Task<IEnumerable<Distrito>> GetDistritosByProvinciaIdAsync(string provinciaId);
        Task<Distrito> GetDistritoByIdAsync(string id);
        Task<bool> ExisteDepartamentoAsync(string departamentoId);
        Task<bool> ExisteProvinciaAsync(string provinciaId, string departamentoId);
        Task<bool> ExisteDistritoAsync(string distritoId, string provinciaId);
    }
}
