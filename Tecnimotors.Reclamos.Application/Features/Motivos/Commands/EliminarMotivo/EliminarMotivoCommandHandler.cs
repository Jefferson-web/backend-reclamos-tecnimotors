using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Commands.EliminarMotivo
{
    public class EliminarMotivoCommand : IRequest<bool>
    {
        public int MotivoId { get; set; }
    }

    public class EliminarMotivoCommandHandler : IRequestHandler<EliminarMotivoCommand, bool>
    {
        private readonly IMotivoRepository _motivoRepository;

        public EliminarMotivoCommandHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
        }

        public async Task<bool> Handle(EliminarMotivoCommand request, CancellationToken cancellationToken)
        {
            return await _motivoRepository.DeleteAsync(request.MotivoId);
        }
    }
}
