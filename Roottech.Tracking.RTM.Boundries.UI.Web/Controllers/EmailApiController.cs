using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Http;

namespace Roottech.Tracking.RTM.Boundries.UI.Web.Controllers
{
    public class EmailApiController : ApiController
    {
        //Don't for get to tunr on this link for the email https://www.google.com/settings/security/lesssecureapps
        [HttpGet]//[HttpPost]
        public void SendBoundaryBreachAlertMail(){//JToken Payload) {
            try {
                /*var emailList = Payload.SelectToken("email").ToObject<List<EmailModel>>();

                foreach (var data in emailList) {
                    
                    //Set Culture
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(data.Locale);
                    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                    
                    //Localize the body
                    var model = JsonConvert.DeserializeObject<Errors>(data.EmailBody);
                    var template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/") + data.EmailTemplate + ".cshtml");
                    data.EmailBody = Razor.Parse<Errors>(template, model);
                    data.EmailSubject = Razor.Parse("@Roottech.Tracking.RTM.Boundries.UI.Web.Resources.Language.ErrorEmailTitle");
                    */
                    //Send Mail
                    var smtpClient = new SmtpClient("smtp.gmail.com", 587) {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EDekha"], 
                            ConfigurationManager.AppSettings["EChupa"]),
                        EnableSsl = true
                    };
                    var mail = new MailMessage
                    {
                        From = new MailAddress("Monitor@Boundary.com", "Monitoring Boundaries"),//data.FromAddress),
                        Subject = "test subject",//data.EmailSubject,
                        Body = "test body",//data.EmailBody,
                        IsBodyHtml = true
                    };
                    mail.To.Add("alerts@roottech.com.pk");
                    /*if (!string.IsNullOrEmpty(data.EmailIds))
                        mail.To.Add(data.EmailIds);
                    if (!string.IsNullOrEmpty(data.EmailIdsCc))
                        mail.CC.Add(data.EmailIdsCc);
                    if (!string.IsNullOrEmpty(data.EmailIdsBcc))
                        mail.Bcc.Add(data.EmailIdsBcc);*/
                    smtpClient.Send(mail);
                //}
            } catch (Exception e) {
                //Log Errors
            }
        }
    }
}
