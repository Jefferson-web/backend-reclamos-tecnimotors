using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate
{
    public interface IArchivoRepository
    {
        Task<Archivo> GetByIdAsync(int id);
        Task<IEnumerable<Archivo>> GetByReclamoAsync(string ticketId);
        Task<IEnumerable<Archivo>> GetByInteraccionAsync(string interaccionId);
        Task<int> AddAsync(Archivo archivo);
        Task<bool> AsociarConReclamoAsync(int archivoId, string ticketId);
        Task<bool> AsociarArchivoInteraccionAsync(string interaccionId, int archivoId);
    }
}
