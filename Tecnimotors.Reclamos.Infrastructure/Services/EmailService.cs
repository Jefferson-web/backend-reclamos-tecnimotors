using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.Services;
using Tecnimotors.Reclamos.Domain.Models;
using Microsoft.Extensions.Options;
using Tecnimotors.Reclamos.Domain.AggregatesModel.PlantillaAggregate;
using Tecnimotors.Reclamos.Application.Common.Interfaces;

namespace Tecnimotors.Reclamos.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmailTemplateProcessor _emailTemplateProcessor;
        public EmailService(
            IOptions<SmtpSettings> smtpSettings, 
            IEmailTemplateRepository emailTemplateRepository,
            IEmailTemplateProcessor emailTemplateProcessor)
        {
            _smtpSettings = smtpSettings.Value;
            _emailTemplateRepository = emailTemplateRepository;
            _emailTemplateProcessor = emailTemplateProcessor;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El destinatario no puede estar vacío", nameof(to));

            try
            {
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(new MailAddress(to));

                using var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if (!string.IsNullOrEmpty(_smtpSettings.Username) && !string.IsNullOrEmpty(_smtpSettings.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                }

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendEmailToMultipleRecipientsAsync(
             IEnumerable<string> toRecipients,
             IEnumerable<string> ccRecipients,
             IEnumerable<string> bccRecipients,
             string subject,
             string body,
             bool isHtml = false)
        {
            if ((toRecipients == null || !toRecipients.Any()) &&
                (ccRecipients == null || !ccRecipients.Any()) &&
                (bccRecipients == null || !bccRecipients.Any()))
            {
                throw new ArgumentException("Debe proporcionar al menos un destinatario");
            }

            toRecipients = toRecipients ?? new List<string>();
            ccRecipients = ccRecipients ?? new List<string>();
            bccRecipients = bccRecipients ?? new List<string>();

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var recipient in toRecipients)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    mailMessage.To.Add(new MailAddress(recipient));
                }
            }

            foreach (var ccRecipient in ccRecipients)
            {
                if (!string.IsNullOrWhiteSpace(ccRecipient))
                {
                    mailMessage.CC.Add(new MailAddress(ccRecipient));
                }
            }

            foreach (var bccRecipient in bccRecipients)
            {
                if (!string.IsNullOrWhiteSpace(bccRecipient))
                {
                    mailMessage.Bcc.Add(new MailAddress(bccRecipient));
                }
            }

            try
            {
                using var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.UseSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                if (!string.IsNullOrEmpty(_smtpSettings.Username) && !string.IsNullOrEmpty(_smtpSettings.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                }

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendEmailUsingTemplateAsync(string to, string templateName, Dictionary<string, string> parameters)
        {
            var template = await _emailTemplateRepository.GetPlantillaByCodigo(templateName);

            if (template == null)
                throw new Exception($"Email template '{templateName}' not found.");

            if (!template.Activo)
                throw new Exception($"Email template '{templateName}' is not active.");

            string subject = _emailTemplateProcessor.ProcessTemplate(template.Asunto, parameters);
            string body = _emailTemplateProcessor.ProcessTemplate(template.ContenidoHtml, parameters);

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendEmailUsingTemplateToMultipleRecipientsAsync(
            IEnumerable<string> toRecipients,
            IEnumerable<string> ccRecipients,
            IEnumerable<string> bccRecipients,
            string templateName,
            Dictionary<string, string> parameters)
        {
            var template = await _emailTemplateRepository.GetPlantillaByCodigo(templateName);

            if (template == null)
                throw new Exception($"Email template '{templateName}' not found.");

            if (!template.Activo)
                throw new Exception($"Email template '{templateName}' is not active.");

            var subject = _emailTemplateProcessor.ProcessTemplate(template.Asunto, parameters);
            var body = _emailTemplateProcessor.ProcessTemplate(template.ContenidoHtml, parameters);

            await SendEmailToMultipleRecipientsAsync(
                toRecipients,
                ccRecipients,
                bccRecipients,
                subject,
                body,
                true
            );
        }
    }
}
