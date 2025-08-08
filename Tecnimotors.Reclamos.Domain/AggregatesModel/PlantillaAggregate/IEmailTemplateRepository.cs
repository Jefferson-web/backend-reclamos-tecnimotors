using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.AggregatesModel.PlantillaAggregate
{
    public interface IEmailTemplateRepository
    {
        Task<PlantillaCorreo> GetPlantillaByCodigo(string codigo);
    }
}
