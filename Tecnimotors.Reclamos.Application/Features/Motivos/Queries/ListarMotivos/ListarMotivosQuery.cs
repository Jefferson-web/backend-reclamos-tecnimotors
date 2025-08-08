using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivos
{
    public class ListarMotivosQuery : IRequest<IEnumerable<GetMotivosQueryResponse>> { }
}
