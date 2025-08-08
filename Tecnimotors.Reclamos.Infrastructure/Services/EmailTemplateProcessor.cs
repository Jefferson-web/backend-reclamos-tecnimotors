using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Application.Common.Interfaces;

namespace Tecnimotors.Reclamos.Infrastructure.Services
{
    public class EmailTemplateProcessor : IEmailTemplateProcessor
    {
        public string ProcessTemplate(string templateContent, Dictionary<string, string> parameters)
        {
            string result = templateContent;

            foreach (var param in parameters)
            {
                result = result.Replace($"{{{{{param.Key}}}}}", param.Value);
            }

            return result;
        }
    }
}
