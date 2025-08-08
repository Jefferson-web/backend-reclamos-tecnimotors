using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Usuarios.GetUsuariosPorRol;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{rolId}")]
        public async Task<IActionResult> ListarUsuariosPorRol(int rolId)
        {
            var response = await _mediator.Send(new GetUsuariosPorRolQuery(rolId));
            return Ok(response);
        }
    }
}
