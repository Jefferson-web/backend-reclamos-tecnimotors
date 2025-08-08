using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.Application.Features.Dashboard.Queries
{
    public class GetDashboardDataQuery : IRequest<DashboardDto>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? Anio { get; set; }

        public GetDashboardDataQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? anio = null)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
            Anio = anio ?? DateTime.Now.Year;
        }
    }

    public class TendenciaReclamosQuery : IRequest<TendenciaReclamosDto>
    {
        public int? Anio { get; set; }

        public TendenciaReclamosQuery(int? anio = null)
        {
            Anio = anio;
        }
    }

    public class GetEstadisticasGeneralesQuery : IRequest<IEnumerable<EstadisticaCardDto>>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public GetEstadisticasGeneralesQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
        }
    }

    public class GetDistribucionEstadosQuery : IRequest<DistribucionEstadosDto>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public GetDistribucionEstadosQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
        }
    }

    public class GetAnalisisMotivosParetoQuery : IRequest<AnalisisMotivosParetoDto>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public GetAnalisisMotivosParetoQuery(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            FechaDesde = fechaDesde;
            FechaHasta = fechaHasta;
        }
    }
}
