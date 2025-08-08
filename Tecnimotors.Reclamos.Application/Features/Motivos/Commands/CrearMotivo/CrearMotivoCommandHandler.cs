using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Commands.CrearMotivo
{
    public class CrearMotivoResponse
    {
        public int MotivoId { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CrearMotivoCommandHandler : IRequestHandler<CrearMotivoCommand, CrearMotivoResponse>
    {
        private readonly IMotivoRepository _motivoRepository;
        public CrearMotivoCommandHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
        }

        public async Task<CrearMotivoResponse> Handle(CrearMotivoCommand request, CancellationToken cancellationToken)
        {

            var motivo = new Motivo(request.Nombre, request.Descripcion);

            await _motivoRepository.AddAsync(motivo);

            return new CrearMotivoResponse
            {
                MotivoId = motivo.MotivoId,
                Nombre = motivo.Nombre,
                FechaRegistro = motivo.FechaRegistro
            };
        }
    }
}
