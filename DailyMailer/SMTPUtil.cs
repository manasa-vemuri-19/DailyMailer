using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;


public class SMTPUtil
{

    private static void sendSMTP(MailMessage mailMessage)
    {
        SmtpClient smtp = new SmtpClient();
        smtp.Host = ConfigurationManager.AppSettings["Host"];
        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
        NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
        NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
        smtp.UseDefaultCredentials = true;
        smtp.Credentials = NetworkCred;
        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
        smtp.Send(mailMessage);
    }

    public static void SendMail(string subject, string html, string[] to, string[] cc, string[] bcc)
    {
        string domain = "@infosys.com";

        using (MailMessage mailMessage = new MailMessage())
        {
            mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = html;




            //Uncomment the below line while sending to everyone

           



           // comment the below three lines while sending to everyone
            mailMessage.To.Clear();
            mailMessage.CC.Clear();
            mailMessage.Bcc.Clear();

            //mailMessage.Bcc.Add("Dhanush.manangi@infosys.com");

            foreach (string id in to)
                mailMessage.To.Add(new MailAddress(id + domain));

            foreach (string id in cc)
                mailMessage.CC.Add(new MailAddress(id + domain));

            foreach (string id in bcc)
                mailMessage.Bcc.Add(new MailAddress(id + domain));







            sendSMTP(mailMessage);
        }
    }

}
