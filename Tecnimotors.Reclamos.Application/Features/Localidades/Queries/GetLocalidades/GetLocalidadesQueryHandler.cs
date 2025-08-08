using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Localidades.Queries.GetLocalidades
{
    public class GetLocalidadesQueryHandler : IRequestHandler<GetLocalidadesQuery, IEnumerable<Localidad>>
    {
        private readonly ILocalidadRepository _localidadRepository;
        public GetLocalidadesQueryHandler(ILocalidadRepository localidadRepository)
        {
            _localidadRepository = localidadRepository;
        }
        public async Task<IEnumerable<Localidad>> Handle(GetLocalidadesQuery request, CancellationToken cancellationToken)
        {
            return await _localidadRepository.GetAllAsync();
        }
    }
}
