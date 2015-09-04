using DrNajeeb.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using DrNajeeb.Web.API.Models;
using System.Net.Mail;
using System.Text;

namespace DrNajeeb.Web.API.Controllers
{
    [Authorize]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
    [HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie)]
    public class SupportController : BaseController
    {

        public SupportController(IUow uow)
        {
            _Uow = uow;
        }

        [ActionName("SupportMessage")]
        [HttpPost]
        public async Task<IHttpActionResult> SupportMessage(SupportModel model)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var user = await _Uow._Users.GetAll(x => x.Id == userId).FirstOrDefaultAsync();

                //Assign the smtp credentials for gmail
                SmtpClient smtp = new SmtpClient();
                if (true)
                {
                    smtp.Host = "mail.drnajeebvideos.com";
                    smtp.Port = 25;
                    smtp.Credentials = new NetworkCredential("info@drnajeebvideos.com", "@Dmin123");
                    smtp.Timeout = 20000;
                }

                MailAddress fromAddress = new MailAddress("info@drnajeebvideos.com", "Support Request From " + user.FullName);
                MailAddress toAddress = new MailAddress("info@drnajeebvideos.com");

                //Passing values to smtp object
                dynamic message = new MailMessage(fromAddress, toAddress);
                message.Subject = model.Subject;

                var sb = new StringBuilder("");
                sb.Append("<b>From : </b>" + user.Email);
                sb.Append("<br />");
                sb.Append("<b>Time : </b>" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                sb.Append("<br />");
                sb.Append("<b>Name : </b>" + user.FullName);
                sb.Append("<br />");
                sb.Append("<h3>Message</h3>");
                sb.Append(model.Message);
                message.Body = sb.ToString();
                message.IsBodyHtml = true;


                //Send email
                smtp.Send(message);
                return Ok("Your message received successfuly");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
