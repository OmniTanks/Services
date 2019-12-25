using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

using CentralServices.Models;
using CentralServices.Databases;
using System.Net;

namespace CentralServices
{
    public static class Mailer
    {
        public static void SendRegistrationEmail(User user)
        {
            using (var settings = new LocalSettingsDB())
            {
                string mailFrom = settings.GetSetting("RegMailFrom");
                string mailTemplate = settings.GetSetting("RegMailBodyTemplate");
                string mailSubject = settings.GetSetting("RegMailSubjectTemplate");

                MailMessage message = new MailMessage(mailFrom, user.Email, mailSubject, mailTemplate);
                SmtpClient client = new SmtpClient(settings.GetSetting("MailSMTPServer"));
                string smtpUser = settings.GetSetting("MailSMTPUser");
                if (!string.IsNullOrEmpty(smtpUser))
                {
                    client.Credentials = new NetworkCredential(smtpUser, settings.GetSetting("MailSMTPPassword"));
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.SendAsync(message, null);
            }
        }

        public static void SendResetEmail(User user)
        {
            using (var settings = new LocalSettingsDB())
            {
                string mailFrom = settings.GetSetting("ResetMailFrom");
                string mailTemplate = FillBody(settings.GetSetting("ResetMailBodyTemplate"), user, settings);
                string mailSubject = FillBody(settings.GetSetting("ResetMailSubjectTemplate"), user, settings);

                MailMessage message = new MailMessage(mailFrom, user.Email, mailSubject, mailTemplate);
                SmtpClient client = new SmtpClient(settings.GetSetting("MailSMTPServer"));
                string smtpUser = settings.GetSetting("MailSMTPUser");
                if (!string.IsNullOrEmpty(smtpUser))
                {
                    client.Credentials = new NetworkCredential(smtpUser, settings.GetSetting("MailSMTPPassword"));
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.SendAsync(message, null);
            }
        }

        public static string FillBody(string template, User user, LocalSettingsDB settings)
        {
            string host = settings.GetSetting("MailLinkURL");
            string results = template.Replace("[URL]", host);
            results = results.Replace("[NAME]", user.Name);
            results = results.Replace("[ID]", user.ID);
            results = results.Replace("[Key]", user.VerificationHash);

            return results;
        }
    }
}
