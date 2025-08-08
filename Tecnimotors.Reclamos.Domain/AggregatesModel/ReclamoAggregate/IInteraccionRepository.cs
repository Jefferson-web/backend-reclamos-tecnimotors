using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public interface IInteraccionRepository
    {
        Task<int> CreateAsync(Interaccion interaccion);
        Task<bool> DeleteAsync(int interaccionId);
        Task<bool> AsociarArchivoAsync(int interaccionId, int archivoId);
        Task<bool> UpdateAsync(Interaccion interaccion);
        Task<IEnumerable<Interaccion>> ObtenerInteracciones(string ticketId); 
    }
}
