using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.LocalidadAggregate;

namespace Tecnimotors.Reclamos.Application.Features.Localidades.Queries.GetLocalidades
{
    public class GetLocalidadesQuery: IRequest<IEnumerable<Localidad>>
    {
    }
}
