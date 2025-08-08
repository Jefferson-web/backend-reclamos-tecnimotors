using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Usuarios.GetUsuariosPorRol
{
    public class GetUsuariosPorRolQueryHandler : IRequestHandler<GetUsuariosPorRolQuery, IEnumerable<UsuarioDto>>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public GetUsuariosPorRolQueryHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<UsuarioDto>> Handle(GetUsuariosPorRolQuery request, CancellationToken cancellationToken)
        {
            var usuarios = await _usuarioRepository.GetByRolIdAsync(request.rolId);

            var result = usuarios.Select(u =>
            {
                return new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Email = u.Email,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    FechaRegistro = u.FechaRegistro
                };
            });

            return result;
        }
    }
}
