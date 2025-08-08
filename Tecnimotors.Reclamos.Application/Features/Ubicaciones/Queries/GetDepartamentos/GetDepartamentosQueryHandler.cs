using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDepartamentos
{
    public class GetDepartamentosQueryHandler : IRequestHandler<GetDepartamentosQuery, IEnumerable<Departamento>>
    {
        private readonly IUbicacionRepository _ubicacionRepository;

        public GetDepartamentosQueryHandler(IUbicacionRepository ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
        }

        public async Task<IEnumerable<Departamento>> Handle(GetDepartamentosQuery request, CancellationToken cancellationToken)
        {
            return await _ubicacionRepository.GetDepartamentosAsync();
        }
    }
}
