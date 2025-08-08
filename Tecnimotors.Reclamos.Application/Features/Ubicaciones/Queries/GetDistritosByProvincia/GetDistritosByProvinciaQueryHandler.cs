using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDistritosByProvincia
{
    public class GetDistritosByProvinciaQueryHandler : IRequestHandler<GetDistritosByProvinciaQuery, IEnumerable<Distrito>>
    {
        private readonly IUbicacionRepository _ubicacionRepository;

        public GetDistritosByProvinciaQueryHandler(IUbicacionRepository ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
        }

        public async Task<IEnumerable<Distrito>> Handle(GetDistritosByProvinciaQuery request, CancellationToken cancellationToken)
        {
            return await _ubicacionRepository.GetDistritosByProvinciaIdAsync(request.ProvinciaId);
        }
    }
}
