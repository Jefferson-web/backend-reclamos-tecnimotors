using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Archivos.Queries.DownloadArchivo
{
    public class DownloadArchivoQueryHandler : IRequestHandler<DownloadArchivoQuery, ArchivoStreamResultDto>
    {
        private readonly IArchivoRepository _archivoRepository;
        private readonly IFileService _fileService;
        public DownloadArchivoQueryHandler(
            IArchivoRepository archivoRepository,
            IFileService fileService)
        {
            _archivoRepository = archivoRepository;
            _fileService = fileService;
        }

        public async Task<ArchivoStreamResultDto> Handle(DownloadArchivoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var archivo = await _archivoRepository.GetByIdAsync(request.archivoId);
                if (archivo == null)
                {
                    return null;
                }

                var contenido = await _fileService.GetFileAsync(archivo.RutaAlmacenamiento);
                var tipoContenido = archivo.TipoMime ?? _fileService.GetContentType(archivo.Extension);

                return new ArchivoStreamResultDto
                {
                    Contenido = contenido,
                    NombreArchivo = archivo.NombreOriginal,
                    TipoContenido = tipoContenido
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
