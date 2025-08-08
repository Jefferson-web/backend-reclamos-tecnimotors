using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate
{
    public interface IReclamoRepository
    {
        Task<(IEnumerable<Reclamo> reclamos, int totalCount)> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            string ticket = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string estado = null,
            string prioridad = null);
        Task<Reclamo> GetByTicketAsync(string ticket);
        Task<IEnumerable<Reclamo>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<int> AddAsync(Reclamo reclamo);
        Task UpdateAsync(Reclamo reclamo);
        Task<string> GenerarTicketAsync();

        Task<IEnumerable<Reclamo>> GetReclamosParaExportarAsync(
            string ticketId = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null,
            string estado = null,
            string prioridad = null);

        Task<bool> VerificarUsuarioAsignadoAsync(string ticketId, int usuarioId);
        Task<IEnumerable<Usuario>> GetUsuariosAsignadosAsync(string ticketId);
        Task<IEnumerable<Reclamo>> ObtenerReclamosParaCierreAutomaticoAsync();
        Task<int> CerrarReclamosAutomaticamenteAsync(List<string> ticketIds);
    }
}
