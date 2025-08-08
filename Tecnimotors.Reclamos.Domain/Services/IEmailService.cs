using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnimotors.Reclamos.Domain.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);

        // Nuevo método para múltiples destinatarios
        Task SendEmailToMultipleRecipientsAsync(
            IEnumerable<string> toRecipients,
            IEnumerable<string> ccRecipients,
            IEnumerable<string> bccRecipients,
            string subject,
            string body,
            bool isHtml = false);

        Task SendEmailUsingTemplateAsync(string to, string templateName, Dictionary<string, string> parameters);
        Task SendEmailUsingTemplateToMultipleRecipientsAsync(
            IEnumerable<string> toRecipients,
            IEnumerable<string> ccRecipients,
            IEnumerable<string> bccRecipients,
            string templateName,
            Dictionary<string, string> parameters);
    }
}
