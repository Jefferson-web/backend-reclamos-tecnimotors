using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.ArchivoAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Archivos.Queries.GetArchivoById
{
    public record GetArchivoByIdQuery(int archivoId): IRequest<Archivo>
    {
    }
}
