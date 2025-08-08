using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Auth.Commands.Login;
using Tecnimotors.Reclamos.Application.Features.Auth.Commands.RegistrarUsuario;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var authResponse = await _mediator.Send(command);

            if (authResponse == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            return Ok(authResponse);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrarUsuarioCommand command)
        {
            var userId = await _mediator.Send(command);

            if (userId == 0)
            {
                return BadRequest("No se pudo registrar el usuario. El email podría estar ya en uso.");
            }

            return Ok(new { UsuarioId = userId, Message = "Usuario registrado correctamente" });
        }
    }
}
