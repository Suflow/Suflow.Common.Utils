using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Suflow.Common.Utils
{
    public class EmailHelper
    {
        public static MailMessage CreateMailMessage(string mailFrom, string mailFromDisplayName, string[] mailTo, string[] mailCc, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(mailFrom, mailFromDisplayName);

            string to = mailTo != null ? string.Join(",", mailTo) : null;
            mail.To.Add(to);

            string cc = mailCc != null ? string.Join(",", mailCc) : null;
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(cc);
            }
            mail.Subject = subject;
            mail.Body = body.Replace(Environment.NewLine, "<BR>");
            mail.IsBodyHtml = true;

            //mail.Attachments.Add(new System.Net.Mail.Attachment("c:/textfile.txt"));
            return mail;
        }

        public static bool SendEmail(MailMessage mail, EmailConfigurationGoDaddy godaddyConfiguration, EmailConfigurationGmail gmailConfiguration) 
        {
            if (!SendEmailViaGoDaddy(mail, godaddyConfiguration))
                return SendEmailViaGmail(mail, gmailConfiguration);
            return true;
        }

        /// <summary>
        /// http://support.godaddy.com/help/article/4714/setting-up-your-email-address-with-imap?pc_split_value=2&countrysite=au
        /// </summary>        
        public static bool SendEmailViaGoDaddy(MailMessage mail, EmailConfigurationGoDaddy godaddyConfiguration)
        {
            try
            {
                //LogService.GetLog4NetLogger().Info("Sending email via go daddy");
                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = godaddyConfiguration.Host;
                    client.Credentials = new NetworkCredential(godaddyConfiguration.From, godaddyConfiguration.Password);
                    client.Port = godaddyConfiguration.Port;
                    client.EnableSsl = godaddyConfiguration.EnableSsl;
                    mail.From = new MailAddress(godaddyConfiguration.From, mail.From.DisplayName);
                    client.Send(mail);
                }
                //LogService.GetLog4NetLogger().Info("Email sent");
                return true;
            }
            catch (Exception ex)
            {
                //LogService.GetLog4NetLogger().Error("Error sending email via go daddy.", ex);
                return false;
            }
        }

        /// <summary>
        /// Error: http://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        /// Solution: https://www.google.com/settings/security/lesssecureapps
        /// Solution1: http://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        /// </summary>
        public static bool SendEmailViaGmail(MailMessage mail, EmailConfigurationGmail gmailConfiguration)
        {
            try
            {
                var client1 = new SmtpClient(gmailConfiguration.Host, 587)
                {
                    Credentials = new NetworkCredential(gmailConfiguration.From, gmailConfiguration.Password),
                    EnableSsl = gmailConfiguration.EnableSsl
                };
                client1.Send(gmailConfiguration.From, gmailConfiguration.From, "test", "testbody");

                //LogService.GetLog4NetLogger().Info("Sending email via gmail.");
                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = gmailConfiguration.Host;
                    client.Credentials = new NetworkCredential(gmailConfiguration.From, gmailConfiguration.Password);
                    client.Port = gmailConfiguration.Port;
                    client.EnableSsl = gmailConfiguration.EnableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network; // to prevent "client was not authenticated" ??
                    //client.UseDefaultCredentials = false;
                    mail.From = new MailAddress(gmailConfiguration.From, mail.From.DisplayName);
                    client.Send(mail);
                }
                //LogService.GetLog4NetLogger().Info("Email sent via gmail");
                return true;
            }
            catch (Exception ex)
            {
                //LogService.GetLog4NetLogger().Error("Error sending email via gmail.", ex);
                return false;
            }
        }

    }

    public class EmailConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
    }

    public class EmailConfigurationGoDaddy : EmailConfiguration
    {
        public EmailConfigurationGoDaddy()
        {
            Host = "smtpout.secureserver.net";
            Port = 80;
            EnableSsl = false;
            //From = "myEmail";
            //Password = "myPassword";
        }
    }

    public class EmailConfigurationGmail : EmailConfiguration
    {
        public EmailConfigurationGmail()
        {
            Host = "smtp.gmail.com";
            Port = 587;
            EnableSsl = false;
            //From = "myEmail";
            //Password = "myPassword";
        }
    }
}
