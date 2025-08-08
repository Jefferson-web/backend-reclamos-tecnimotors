using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetProvinciasByDepartamento
{
    public class GetProvinciasByDepartamentoQueryHandler : IRequestHandler<GetProvinciasByDepartamentoQuery, IEnumerable<Provincia>>
    {
        private readonly IUbicacionRepository _ubicacionRepository;

        public GetProvinciasByDepartamentoQueryHandler(IUbicacionRepository ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
        }

        public async Task<IEnumerable<Provincia>> Handle(GetProvinciasByDepartamentoQuery request, CancellationToken cancellationToken)
        {
            return await _ubicacionRepository.GetProvinciasByDepartamentoIdAsync(request.DepartamentoId);
        }
    }
}
