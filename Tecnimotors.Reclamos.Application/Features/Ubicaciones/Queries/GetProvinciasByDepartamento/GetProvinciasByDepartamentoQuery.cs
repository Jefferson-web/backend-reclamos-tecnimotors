using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetProvinciasByDepartamento
{
    public class GetProvinciasByDepartamentoQuery : IRequest<IEnumerable<Provincia>>
    {
        public string DepartamentoId { get; }

        public GetProvinciasByDepartamentoQuery(string departamentoId)
        {
            DepartamentoId = departamentoId;
        }
    }
}
