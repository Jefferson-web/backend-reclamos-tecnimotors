using MediatR;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.Features.Auth.Commands.RegistrarUsuario
{
    internal class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, int>
    {
        private readonly IAuthService _authService;

        public RegistrarUsuarioCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<int> Handle(RegistrarUsuarioCommand request, CancellationToken cancellationToken)
        {
            var registrationModel = new UserRegistrationModel
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Apellidos = request.Apellidos,
                Password = request.Password,
                RolId = request.RolId
            };

            var userId = await _authService.RegisterUserAsync(registrationModel);

            return userId;
        }
    }
}
