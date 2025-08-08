using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;

namespace Tecnimotors.Reclamos.Application.Jobs
{
    public class CerrarReclamosJob : IJob
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly ILogger<CerrarReclamosJob> _logger;

        public CerrarReclamosJob(IReclamoRepository reclamoRepository, ILogger<CerrarReclamosJob> logger)
        {
            _reclamoRepository = reclamoRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await ProcesarCierreAutomatico();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de cierre automático de reclamos");
                throw;
            }
        }

        private async Task ProcesarCierreAutomatico()
        {
            var reclamosParaCerrar = await _reclamoRepository.ObtenerReclamosParaCierreAutomaticoAsync();

            if (!reclamosParaCerrar.Any())
            {
                _logger.LogInformation("No se encontraron reclamos para cerrar automáticamente");
                return;
            }

            _logger.LogInformation("Se encontraron {Count} reclamos para cerrar automáticamente",
           reclamosParaCerrar.Count());

            var ticketsParaCerrar = reclamosParaCerrar.Select(r => r.TicketId).ToList();
            _logger.LogInformation("Tickets a cerrar: {Tickets}", string.Join(", ", ticketsParaCerrar));

            const int tamañoLote = 50;
            var totalCerrados = 0;

            for (int i = 0; i < ticketsParaCerrar.Count; i += tamañoLote)
            {
                var lote = ticketsParaCerrar.Skip(i).Take(tamañoLote).ToList();
                var cerradosEnLote = await _reclamoRepository.CerrarReclamosAutomaticamenteAsync(lote);
                totalCerrados += cerradosEnLote;

                _logger.LogInformation("Lote {NumeroLote}: {CerradosEnLote} reclamos cerrados",
                    (i / tamañoLote) + 1, cerradosEnLote);
            }

            _logger.LogInformation("Proceso completado exitosamente. Total de reclamos cerrados: {Total}",
            totalCerrados);
        }
    }
}
