using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Usuarios.GetUsuariosPorRol
{
    public record GetUsuariosPorRolQuery(int rolId): IRequest<IEnumerable<UsuarioDto>>
    {
    }
}
