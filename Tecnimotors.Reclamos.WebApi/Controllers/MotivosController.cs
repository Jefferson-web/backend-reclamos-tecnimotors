using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Common.DTOs.Motivos;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Motivos.Commands.ActualizarMotivo;
using Tecnimotors.Reclamos.Application.Features.Motivos.Commands.CrearMotivo;
using Tecnimotors.Reclamos.Application.Features.Motivos.Commands.EliminarMotivo;
using Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivos;
using Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ListarMotivosFiltros;
using Tecnimotors.Reclamos.Application.Features.Motivos.Queries.ObtenerMotivoPorId;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MotivosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MotivosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMotivos()
        {
            var response = await _mediator.Send(new ListarMotivosQuery());
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMotivo(int id)
        {
            var motivo = await _mediator.Send(new ObtenerMotivoPorIdQuery { MotivoId = id });

            if (motivo == null)
                return NotFound();

            return Ok(motivo);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CrearMotivo([FromBody] CrearMotivoDTO request)
        {
            try
            {
                var response = await _mediator.Send(new CrearMotivoCommand
                {
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion
                });

                // Devolver 201 Created con la ubicación del recurso
                return CreatedAtAction(
                    nameof(GetMotivo),
                    new { id = response.MotivoId },
                    null // No devolvemos el objeto completo como solicitaste
                );
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarMotivo(int id, [FromBody] ActualizarMotivoDTO request)
        {
            try
            {
                var resultado = await _mediator.Send(new ActualizarMotivoCommand
                {
                    MotivoId = id,
                    Nombre = request.Nombre,
                    Descripcion = request.Descripcion,
                    Activo = request.Activo
                });

                if (!resultado)
                    return NotFound();

                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarMotivo(int id)
        {
            var resultado = await _mediator.Send(new EliminarMotivoCommand { MotivoId = id });

            if (!resultado)
                return NotFound();

            return NoContent();
        }

        [HttpGet("ListarMotivosByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedList<GetMotivosQueryResponse>>> Get([FromQuery] ListarMotivosFiltrosQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}