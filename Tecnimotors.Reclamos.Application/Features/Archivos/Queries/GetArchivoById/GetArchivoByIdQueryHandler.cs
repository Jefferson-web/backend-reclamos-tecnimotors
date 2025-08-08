using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Archivos.Queries.GetArchivoById
{
    public class GetArchivoByIdQueryHandler : IRequestHandler<GetArchivoByIdQuery, Archivo>
    {
        private readonly IArchivoRepository _repository;

        public GetArchivoByIdQueryHandler(IArchivoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Archivo> Handle(GetArchivoByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.archivoId);
        }
    }
}
