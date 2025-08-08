using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Application.Common.Interfaces
{
    public interface IEmailTemplateProcessor
    {
        string ProcessTemplate(string templateContent, Dictionary<string, string> parameters);
    }
}
