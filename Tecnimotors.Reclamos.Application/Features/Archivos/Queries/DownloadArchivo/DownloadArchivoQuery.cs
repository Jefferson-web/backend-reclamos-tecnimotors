using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Features.Archivos.Queries.DownloadArchivo
{
    public record DownloadArchivoQuery(int archivoId): IRequest<ArchivoStreamResultDto>
    {
    }
}
