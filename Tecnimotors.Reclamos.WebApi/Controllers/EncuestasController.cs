using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tecnimotors.Reclamos.Application.Features.Encuestas.Commands.ResponderEncuestas;
using Tecnimotors.Reclamos.Application.Features.Encuestas.Queries.ObtenerEncuestaPorToken;
using Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.Dtos;

namespace Tecnimotors.Reclamos.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncuestasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EncuestasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Envía encuestas a tickets cerrados
        /// </summary>
        //[HttpPost("enviar")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<EnviarEncuestasResponse>> EnviarEncuestas(
        //    [FromBody] EnviarEncuestasCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //    return Ok(result);
        //}

        /// <summary>
        /// Obtiene una encuesta por token
        /// </summary>
        [HttpGet("{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<EncuestaDto>> ObtenerEncuesta(Guid token)
        {
            var query = new ObtenerEncuestaPorTokenQuery { Token = token };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Responde una encuesta
        /// </summary>
        [HttpPost("{token}/responder")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponderEncuestaResponse>> ResponderEncuesta(
            Guid token,
            [FromBody] ResponderEncuestaRequest request)
        {
            var command = new ResponderEncuestaCommand
            {
                Token = token,
                Respuestas = request.Respuestas,
                Comentario = request.Comentario,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            var result = await _mediator.Send(command);

            if (!result.Exitoso)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene el ISG para un período
        /// </summary>
        //[HttpGet("isg")]
        //[Authorize]
        //public async Task<ActionResult<ISGResponse>> ObtenerISG(
        //    [FromQuery] DateTime? fechaInicio,
        //    [FromQuery] DateTime? fechaFin)
        //{
        //    var query = new ObtenerISGQuery
        //    {
        //        FechaInicio = fechaInicio ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
        //        FechaFin = fechaFin ?? DateTime.Now
        //    };

        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}

        ///// <summary>
        ///// Obtiene estadísticas de satisfacción para dashboard
        ///// </summary>
        //[HttpGet("estadisticas/satisfaccion")]
        //[Authorize]
        //public async Task<ActionResult<EstadisticaCardDto>> ObtenerEstadisticasSatisfaccion()
        //{
        //    var query = new ObtenerEstadisticasSatisfaccionQuery();
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}
    }

    // Request models
    public record ResponderEncuestaRequest
    {
        public Dictionary<string, int> Respuestas { get; init; } = new();
        public string Comentario { get; init; }
    }
}
