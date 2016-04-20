//using Microsoft.Exchange.WebServices.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace Suflow.Common.Utils {
//    /// <summary>
//    /// Needs refeerence to Microsoft.Exchange.WebServices.dll
//    /// </summary>
//    public class ExchangeHelper {
//        //This is slower because it usage autodiscover
//        public static ExchangeService CreateExchangeServiceFromEmail(string domain, string user, string password, string email) {
//            ExchangeService service = new ExchangeService();
//            service.Credentials = new WebCredentials(user, password, domain);
//            service.AutodiscoverUrl(email);
//            return service;
//        }

//        public static ExchangeService CreateExchangeServiceFromUrl(string domain, string user, string password, string ewsUrl, ExchangeVersion exchangeVersion) {
//            ExchangeService service = new ExchangeService(exchangeVersion);
//            service.Credentials = new WebCredentials(user, password, domain);
//            service.Url = new Uri(ewsUrl);
//            return service;
//        }

//        public static Appointment CreateAppointment(ExchangeService service, string subject, string body, DateTime from, DateTime to, string attendees) {
//            Appointment appointment = new Appointment(service);
//            appointment.Subject = subject;
//            appointment.Body = body;
//            appointment.Start = from;
//            appointment.End = to;
//            appointment.RequiredAttendees.Add(attendees);
//            return appointment;
//        }
//    }
//}
