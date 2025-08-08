using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tecnimotors.Reclamos.Application.Features.Interacciones.Commands.CrearInteraccion;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class InteraccionesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InteraccionesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearInteraccion([FromForm] CrearInteraccionCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear interacción: {ex.Message}");
            }
        }
    }
}
