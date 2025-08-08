using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Interfaces.Queries;
using Tecnimotors.Reclamos.Domain.AggregatesModel.MotivoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ReclamoAggregate;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Reclamos.Queries.ConsultaReclamoPublico
{
    public class ConsultaReclamoPublicoQuery : IRequest<ReclamoPublicoDto>
    {
        public string TicketId { get; set; }
    }

    public class ConsultaReclamoPublicoQueryHandler : IRequestHandler<ConsultaReclamoPublicoQuery, ReclamoPublicoDto>
    {
        private readonly IReclamoRepository _reclamoRepository;
        private readonly IMotivoRepository _motivoRepository;
        private readonly IHistorialEstadoRepository _historialRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ConsultaReclamoPublicoQueryHandler(
            IReclamoRepository reclamoRepository,
            IMotivoRepository motivoRepository,
            IHistorialEstadoRepository historialRepository,
            IUsuarioRepository usuarioRepository)
        {
            _reclamoRepository = reclamoRepository ?? throw new ArgumentNullException(nameof(reclamoRepository));
            _motivoRepository = motivoRepository ?? throw new ArgumentNullException(nameof(motivoRepository));
            _historialRepository = historialRepository ?? throw new ArgumentNullException(nameof(historialRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<ReclamoPublicoDto> Handle(ConsultaReclamoPublicoQuery request, CancellationToken cancellationToken)
        {
            // Obtener el reclamo por su ID
            var reclamo = await _reclamoRepository.GetByTicketAsync(request.TicketId);

            if (reclamo == null)
                return null;

            // Obtener el motivo del reclamo
            var motivo = await _motivoRepository.GetByIdAsync(reclamo.MotivoId);

            // Obtener historial de estados
            var historial = await _historialRepository.GetByTicketIdAsync(request.TicketId);

            // Crear el DTO de respuesta
            var reclamoDto = new ReclamoPublicoDto
            {
                TicketId = reclamo.TicketId,
                Estado = reclamo.Estado,
                FechaCreacion = reclamo.FechaCreacion,
                UltimaModificacion = reclamo.UltimaModificacion,
                TipoReclamo = motivo?.Nombre ?? "No especificado",
                Descripcion = reclamo.Detalle,
                Correo = reclamo.Correo
            };

            // Agregar el historial de estados
            foreach (var item in historial.OrderBy(h => h.FechaRegistro))
            {
                var usuario = await _usuarioRepository.GetByIdAsync(item.UsuarioId);

                reclamoDto.Historial.Add(new HistorialEstadoDto
                {
                    Estado = item.EstadoNuevo,
                    EstadoTexto = GetEstadoTexto(item.EstadoNuevo),
                    EstadoAnterior = item.EstadoAnterior,
                    EstadoAnteriorTexto = GetEstadoTexto(item.EstadoAnterior),
                    Fecha = item.FechaRegistro,
                    Comentario = item.Comentario ?? GetDescripcionEstado(item.EstadoNuevo)
                });
            }

            return reclamoDto;
        }

        private string GetEstadoTexto(string estado)
        {
            if (string.IsNullOrEmpty(estado))
                return null;

            switch (estado.ToUpper())
            {
                case "REGISTRADO":
                    return "Registrado";
                case "EN_PROCESO":
                    return "En Proceso";
                case "ATENDIDO":
                    return "Atendido";
                case "CERRADO":
                    return "Cerrado";
                case "RECHAZADO":
                    return "Rechazado";
                default:
                    return estado;
            }
        }

        private string GetDescripcionEstado(string estado)
        {
            if (string.IsNullOrEmpty(estado))
                return null;

            switch (estado.ToUpper())
            {
                case "REGISTRADO":
                    return "Tu reclamo ha sido registrado en nuestro sistema";
                case "EN_PROCESO":
                    return "Tu reclamo está siendo revisado por nuestro equipo";
                case "ATENDIDO":
                    return "Tu reclamo ha sido validado y pronto será resuelto";
                case "CERRADO":
                    return "Tu reclamo ha sido resuelto satisfactoriamente";
                case "RECHAZADO":
                    return "Tu reclamo no pudo ser procesado";
                default:
                    return "Estado actualizado";
            }
        }
    }
}
