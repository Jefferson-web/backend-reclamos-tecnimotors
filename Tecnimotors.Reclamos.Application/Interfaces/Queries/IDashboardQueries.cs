using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Dashboard.Dtos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.Application.Interfaces.Queries
{
    public interface IDashboardQueries
    {
        Task<IEnumerable<EstadisticaCardDto>> GetEstadisticasGeneralesAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<DistribucionEstadosDto> GetDistribucionEstadosAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<AnalisisMotivosParetoDto> GetAnalisisMotivosParetoAsync(DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<TendenciaReclamosDto> GetTendenciaReclamosAsync(int anio);
        Task<IEnumerable<EstadisticaISGDto>> GetEstadisticasISGAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null);
    }
}
