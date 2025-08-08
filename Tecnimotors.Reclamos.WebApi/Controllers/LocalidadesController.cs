using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Localidades.Queries.GetLocalidades;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalidadesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LocalidadesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> ListarLocalidades()
        {
            var localidades = await _mediator.Send(new GetLocalidadesQuery());
            return Ok(localidades);
        }
    }
}
