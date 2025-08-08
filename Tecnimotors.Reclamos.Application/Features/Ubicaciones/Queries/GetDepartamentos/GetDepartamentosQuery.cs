using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDepartamentos
{
    public class GetDepartamentosQuery : IRequest<IEnumerable<Departamento>>
    {
    }
}
