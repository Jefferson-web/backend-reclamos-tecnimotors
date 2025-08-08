using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IHttpContextAccessor httpContextAccessor, IUsuarioRepository usuarioRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> GetCurrentUserAsync()
        {
            // Obtener el ID del usuario desde los claims
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return null;
            }

            // Obtener el usuario de la base de datos usando su ID
            var usuarioId = int.Parse(userIdClaim.Value);
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);

            // Verificar que el usuario exista y esté activo
            if (usuario == null || !usuario.Activo)
            {
                return null;
            }

            return usuario;
        }
    }
}
