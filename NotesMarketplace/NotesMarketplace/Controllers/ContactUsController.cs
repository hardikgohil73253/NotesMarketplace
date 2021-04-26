using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;

namespace NotesMarketplace.Controllers
{
    public class ContactUsController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("ContactUs")]
        public ActionResult ContactUs()
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();
            if (obj != null)
            {
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            }
            return View();
        }

        [HttpPost]
        [Route("ContactUs")]
        public ActionResult ContactUs(Models.ContactUsModel model)
        {
            if (ModelState.IsValid)
            {
                ContactUs obj = new ContactUs();
                obj.FullName = model.FullName;
                obj.EmailID = model.EmailID;
                obj.Subjects = model.Subject;
                obj.Comments = model.Comments;

                dbobj.ContactUs.Add(obj);
                dbobj.SaveChanges();
                SendEmailToAdmin(obj);
            }
            ModelState.Clear();
            return RedirectToAction("ContactUs");
        }


        [NonAction]
        public void SendEmailToAdmin(ContactUs obj)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com"); //Email of Company
            var toEmail = new MailAddress("gohilhardik241274@gmail.com"); //Email of admin
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = obj.FullName + " - Query";

            string body = "Hello," +
                "<br/><br/>" + obj.Comments +
                "<br/><br/>Regards, " + obj.FullName;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }
    }
}