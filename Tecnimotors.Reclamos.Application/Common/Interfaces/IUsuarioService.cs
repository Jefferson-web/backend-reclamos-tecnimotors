using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;

namespace Tecnimotors.Reclamos.Application.Common.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> GetCurrentUserAsync();
    }
}
