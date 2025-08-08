using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UbicacionAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Ubicaciones.Queries.GetDistritosByProvincia
{
    public class GetDistritosByProvinciaQuery : IRequest<IEnumerable<Distrito>>
    {
        public string ProvinciaId { get; }

        public GetDistritosByProvinciaQuery(string provinciaId)
        {
            ProvinciaId = provinciaId;
        }
    }
}
