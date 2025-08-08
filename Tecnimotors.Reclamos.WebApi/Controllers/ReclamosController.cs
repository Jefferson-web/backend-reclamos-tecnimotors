using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Common.Models;
using Tecnimotors.Reclamos.Application.Features.Interacciones.Commands.CrearInteraccion;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.AtenderReclamo;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CerrarReclamo;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.CrearReclamo;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Commands.RechazarReclamo;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.ConsultaReclamoPublico;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.ExportarReclamos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetDetalleReclamoCompleto;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamos;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.GetReclamosListado;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReclamosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReclamosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearReclamo([FromForm] CrearReclamoCommand command)
        {
            string ticket = await _mediator.Send(command);
            return Ok(new { data = ticket, message = "Reclamo creado satisfactoriamente." });
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<ReclamoListadoDto>>> GetReclamos(
       [FromQuery] string ticketId = "",
       [FromQuery] DateTime? fechaDesde = null,
       [FromQuery] DateTime? fechaHasta = null,
       [FromQuery] string estado = "",
       [FromQuery] string prioridad = "",
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
        {
            var query = new GetReclamosListadoQuery
            {
                TicketId = ticketId,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                Estado = estado,
                Prioridad = prioridad,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportarReclamos([FromQuery] ExportarReclamosQuery query)
        {
            var resultado = await _mediator.Send(query);

            return File(
                resultado.Content,
                resultado.ContentType,
                resultado.FileName);
        }

        [HttpGet("{ticketId}/detalle-completo")]
        public async Task<ActionResult<ReclamoDetalleCompletoDto>> GetReclamoDetalleCompleto(string ticketId)
        {
            var query = new GetReclamoDetalleCompletoQuery(ticketId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"No se encontró el reclamo con ticket {ticketId}");

            return Ok(result);
        }

        [HttpPost("interacciones")]
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

        [HttpPut("{ticketId}/atender")]
        public async Task<IActionResult> Atender(string ticketId, [FromBody] AtenderReclamoCommand command)
        {
            if (ticketId != command.TicketId)
            {
                return BadRequest("El TicketId en la URL no coincide con el TicketId en el cuerpo");
            }

            try
            {
                await _mediator.Send(command);
                return Ok(new { success = true, message = "Reclamo marcado como atendido correctamente" });
            }
            catch (System.ApplicationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{ticketId}/cerrar")]
        public async Task<IActionResult> Cerrar(string ticketId, [FromBody] CerrarReclamoCommand command)
        {
            if (ticketId != command.TicketId)
            {
                return BadRequest("El TicketId en la URL no coincide con el TicketId en el cuerpo");
            }

            try
            {
                var resultado = await _mediator.Send(command);
                return Ok(new { success = resultado, message = "Reclamo cerrado correctamente" });
            }
            catch (System.ApplicationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{ticketId}/rechazar")]
        public async Task<IActionResult> Rechazar(string ticketId, [FromBody] RechazarReclamoCommand command)
        {
            if (ticketId != command.TicketId)
            {
                return BadRequest("El TicketId en la URL no coincide con el TicketId en el cuerpo");
            }

            try
            {
                var resultado = await _mediator.Send(command);
                return Ok(new { success = resultado, message = "Reclamo rechazado correctamente" });
            }
            catch (System.ApplicationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("reclamo/{ticketId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConsultarReclamo(string ticketId)
        {
            var query = new ConsultaReclamoPublicoQuery { TicketId = ticketId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = "No se encontró el reclamo solicitado" });

            return Ok(result);
        }
    }
}
