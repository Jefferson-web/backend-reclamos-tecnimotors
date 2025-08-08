using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Commands.ActualizarMotivo
{
    public class ActualizarMotivoCommand : IRequest<bool>
    {
        public int MotivoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class ActualizarMotivoCommandHandler : IRequestHandler<ActualizarMotivoCommand, bool>
    {
        private readonly IMotivoRepository _motivoRepository;

        public ActualizarMotivoCommandHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
        }

        public async Task<bool> Handle(ActualizarMotivoCommand request, CancellationToken cancellationToken)
        {
            var motivo = await _motivoRepository.GetByIdAsync(request.MotivoId);

            if (motivo == null)
                return false;

            motivo.Actualizar(request.Nombre, request.Descripcion, request.Activo);
            await _motivoRepository.UpdateAsync(motivo);

            return true;
        }
    }
}
