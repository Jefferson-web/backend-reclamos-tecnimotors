using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.EncuestaAggregate
{
    public interface IEncuestaRepository
    {
        // ConfiguracionEncuesta
        Task<ConfiguracionEncuesta> GetConfiguracionActivaAsync(string tipoEncuesta);
        Task<int> CreateConfiguracionAsync(ConfiguracionEncuesta configuracion);

        // EncuestasEnviadas
        Task<EncuestasEnviadas> GetEncuestaByTokenAsync(Guid token);
        Task<int> CreateEncuestaEnviadaAsync(EncuestasEnviadas encuesta);
        Task<bool> UpdateEstadoEncuestaAsync(int id, string estado, DateTime? fechaRespuesta);
        Task<bool> ExisteEncuestaPorTicketAsync(string ticketId);

        // Preguntas
        Task<IEnumerable<PreguntasEncuesta>> GetPreguntasByConfiguracionAsync(int configuracionId);
        Task<int> CreatePreguntaAsync(PreguntasEncuesta pregunta);

        // Respuestas
        Task<int> CreateRespuestaAsync(RespuestasEncuesta respuesta);
        Task<IEnumerable<RespuestasEncuesta>> GetRespuestasByEncuestaAsync(int encuestaId);

        // Comentarios
        Task<int> CreateComentarioAsync(ComentariosEncuesta comentario);

        // Queries complejas
        Task<decimal> CalcularISGAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
