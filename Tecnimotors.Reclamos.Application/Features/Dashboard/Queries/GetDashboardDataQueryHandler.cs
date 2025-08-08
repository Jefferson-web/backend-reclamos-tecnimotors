using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;

namespace Tecnimotors.Reclamos.Application.Features.Dashboard.Queries
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDto>
    {
        private readonly IDashboardQueries _dashboardRepository;

        public GetDashboardDataQueryHandler(IDashboardQueries dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
        }

        public async Task<DashboardDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var estadisticas = await _dashboardRepository.GetEstadisticasGeneralesAsync(request.FechaDesde, request.FechaHasta);
            var distribucionEstados = await _dashboardRepository.GetDistribucionEstadosAsync(request.FechaDesde, request.FechaHasta);
            var analisisMotivos = await _dashboardRepository.GetAnalisisMotivosParetoAsync(request.FechaDesde, request.FechaHasta);
            var tendenciaReclamos = await _dashboardRepository.GetTendenciaReclamosAsync(request.Anio ?? DateTime.Now.Year);

            return new DashboardDto
            {
                EstadisticasCards = estadisticas,
                DistribucionEstados = distribucionEstados,
                AnalisisMotivos = analisisMotivos,
                TendenciaReclamos = tendenciaReclamos
            };
        }
    }

    public class GetTendenciasReclamosQueryHandler : IRequestHandler<TendenciaReclamosQuery, TendenciaReclamosDto>
    {
        private readonly IDashboardQueries _dashboardRepository;

        public GetTendenciasReclamosQueryHandler(IDashboardQueries dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
        }

        public async Task<TendenciaReclamosDto> Handle(TendenciaReclamosQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardRepository.GetTendenciaReclamosAsync(request.Anio ?? DateTime.Now.Year);
        }
    }

    public class GetDistribucionEstadosQueryHandler : IRequestHandler<GetDistribucionEstadosQuery, DistribucionEstadosDto>
    {
        private readonly IDashboardQueries _dashboardRepository;

        public GetDistribucionEstadosQueryHandler(IDashboardQueries dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
        }

        public async Task<DistribucionEstadosDto> Handle(GetDistribucionEstadosQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardRepository.GetDistribucionEstadosAsync(request.FechaDesde, request.FechaHasta);
        }
    }

    public class GetEstadisticasGeneralesQueryHandler : IRequestHandler<GetEstadisticasGeneralesQuery, IEnumerable<EstadisticaCardDto>>
    {
        private readonly IDashboardQueries _dashboardRepository;

        public GetEstadisticasGeneralesQueryHandler(IDashboardQueries dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
        }

        public async Task<IEnumerable<EstadisticaCardDto>> Handle(GetEstadisticasGeneralesQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardRepository.GetEstadisticasGeneralesAsync(request.FechaDesde, request.FechaHasta);
        }
    }

    public class GetAnalisisMotivosParetoQueryHandler : IRequestHandler<GetAnalisisMotivosParetoQuery, AnalisisMotivosParetoDto>
    {
        private readonly IDashboardQueries _dashboardRepository;

        public GetAnalisisMotivosParetoQueryHandler(IDashboardQueries dashboardRepository)
        {
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
        }

        public async Task<AnalisisMotivosParetoDto> Handle(GetAnalisisMotivosParetoQuery request, CancellationToken cancellationToken)
        {
            return await _dashboardRepository.GetAnalisisMotivosParetoAsync(request.FechaDesde, request.FechaHasta);
        }
    }
}
