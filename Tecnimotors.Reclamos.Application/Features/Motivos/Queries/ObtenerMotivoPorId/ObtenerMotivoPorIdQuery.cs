using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ObtenerMotivoPorId
{
    public class MotivoDTO
    {
        public int MotivoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
    public class ObtenerMotivoPorIdQuery : IRequest<MotivoDTO>
    {
        public int MotivoId { get; set; }
    }
    public class ObtenerMotivoPorIdQueryHandler : IRequestHandler<ObtenerMotivoPorIdQuery, MotivoDTO>
    {
        private readonly IMotivoRepository _motivoRepository;

        public ObtenerMotivoPorIdQueryHandler(IMotivoRepository motivoRepository)
        {
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
        }

        public async Task<MotivoDTO> Handle(ObtenerMotivoPorIdQuery request, CancellationToken cancellationToken)
        {
            var motivo = await _motivoRepository.GetByIdAsync(request.MotivoId);

            if (motivo == null)
                return null;

            return new MotivoDTO
            {
                MotivoId = motivo.MotivoId,
                Nombre = motivo.Nombre,
                Descripcion = motivo.Descripcion,
                Activo = motivo.Activo,
                FechaRegistro = motivo.FechaRegistro,
                FechaActualizacion = motivo.FechaActualizacion
            };
        }
    }
}
