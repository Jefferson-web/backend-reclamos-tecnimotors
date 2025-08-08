using MediatR;
using System;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivos
{
    public class ListarMotivosQueryHandler : IRequestHandler<ListarMotivosQuery, IEnumerable<GetMotivosQueryResponse>>
    {
        private readonly IMotivoRepository _motivoRepository;
        public ListarMotivosQueryHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository;
        }

        public async Task<IEnumerable<GetMotivosQueryResponse>> Handle(ListarMotivosQuery request, CancellationToken cancellationToken)
        {
            var motivos = await _motivoRepository.GetAllAsync();

            var response = motivos.Select(m =>
            {
                return new GetMotivosQueryResponse
                {
                    MotivoId = m.MotivoId,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    Activo = m.Activo,
                    FechaRegistro = m.FechaRegistro,
                    FechaActualizacion = m.FechaActualizacion
                };
            });

            return response;
        }
    }
}
