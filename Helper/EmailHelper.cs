////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Suflow.Common.Utils {
    public class EmailHelper {
        public static MailMessage CreateMailMessage(string mailFrom, string mailFromDisplayName, string[] mailTo, string[] mailCc, string subject, string body, string[] attachments) {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(mailFrom, mailFromDisplayName);

            string to = mailTo != null ? string.Join(",", mailTo) : null;
            mail.To.Add(to);

            string cc = mailCc != null ? string.Join(",", mailCc) : null;
            if (!string.IsNullOrEmpty(cc)) {
                mail.CC.Add(cc);
            }
            mail.Subject = subject;
            mail.Body = body.Replace(Environment.NewLine, "<BR>");
            mail.IsBodyHtml = true;

            if (attachments != null) {
                foreach (var attachment in attachments)
                    mail.Attachments.Add(new System.Net.Mail.Attachment(attachment, MediaTypeNames.Application.Octet));
            }

            return mail;

        }

        /// <summary>
        /// http://support.godaddy.com/help/article/4714/setting-up-your-email-address-with-imap?pc_split_value=2&countrysite=au
        /// </summary>        
        public static bool SendEmail(MailMessage mail, EmailConfiguration hostConfiguration) {
            try {
                if (hostConfiguration == null)
                    return false;
                if (hostConfiguration.Host == "smtp.gmail.com")
                    return SendEmailViaGmail(mail, hostConfiguration);

                using (SmtpClient client = new SmtpClient()) {
                    client.Host = hostConfiguration.Host;
                    client.Credentials = new NetworkCredential(hostConfiguration.From, hostConfiguration.Password);
                    client.Port = hostConfiguration.Port;
                    client.EnableSsl = hostConfiguration.EnableSsl;
                    mail.From = new MailAddress(hostConfiguration.From, mail.From.DisplayName);
                    client.Send(mail);
                }
                return true;
            }
            catch (Exception e) {
                var message = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Error: http://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        /// Solution: https://www.google.com/settings/security/lesssecureapps
        /// Solution1: http://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        /// </summary>
        private static bool SendEmailViaGmail(MailMessage mail, EmailConfiguration gmailConfiguration) {
            try {
                if (gmailConfiguration == null)
                    return false;
                using (SmtpClient client = new SmtpClient()) {
                    client.Host = gmailConfiguration.Host;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(gmailConfiguration.From, gmailConfiguration.Password);
                    client.Port = gmailConfiguration.Port;
                    client.EnableSsl = gmailConfiguration.EnableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network; // to prevent "client was not authenticated" ??
                    mail.From = new MailAddress(gmailConfiguration.From, mail.From.DisplayName);
                    client.Send(mail);
                }
                return true;
            }
            catch (Exception e) {
                var message = e.Message;
                return false;
            }
        }

    }

    public class EmailConfiguration {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
    }

    public class EmailConfigurationGoDaddy : EmailConfiguration {
        public EmailConfigurationGoDaddy(string from, string password) {
            Host = "smtpout.secureserver.net";
            Port = 80;
            EnableSsl = false;
            From = from;
            Password = password;
        }
    }

    /// <summary>
    /// https://www.google.com/settings/security/lesssecureapps
    /// </summary>
    public class EmailConfigurationGmail : EmailConfiguration {
        public EmailConfigurationGmail(string from, string password) {
            Host = "smtp.gmail.com";
            Port = 587;
            EnableSsl = true;
            From = from;
            Password = password;
        }
    }
}
