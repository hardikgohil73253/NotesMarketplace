using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NotesMarketplace.Controllers
{
    public class AddAdminController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        
        [Route("AddAdmin")]
        public ActionResult AddAdmin()
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.CountryCode = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [HttpPost]
        
        [Route("AddAdmin")]
        public ActionResult AddAdmin(Models.AddAdmin model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var isExist = IsEmailExist(model.Email);
                if (isExist)
                {
                    ModelState.AddModelError("Email", "Email already exist");
                    ViewBag.CountryCode = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
                    return View(model);
                }

                Users obj = new Users();
                obj.RoleID = 2;
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.EmailID = model.Email;
                string pwd = Membership.GeneratePassword(6, 2);
                obj.Password = pwd;
                obj.IsEmailVerified = true;
                obj.CreatedData = DateTime.Now;
                obj.CreatedBy = admin.ID;
                obj.IsActive = true;

                Admin adobj = new Admin();
                adobj.UserID = obj.ID;
                adobj.CountryCode = model.CountryCode;
                adobj.PhoneNumber = model.PhoneNumber;
                adobj.CreatedDate = DateTime.Now;
                adobj.CreatedBy = admin.ID;
                adobj.IsActive = true;

                dbobj.Users.Add(obj);
                dbobj.Admin.Add(adobj);
                dbobj.SaveChanges();

                SendPasswordToAdmin(model.Email, pwd);

                return RedirectToAction("ManageAdmin", "Admin");
            }
            return View();
        }

        [NonAction]
        public void SendPasswordToAdmin(string emailID, string pwd)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Hardik@018"; // Replace with actual password
            string subject = "Note Marketplace - You are Admin now.";

            string body = "Hello," +
                "<br/><br/>We have generated a new password for you" +
                "<br/><br/>Password: " + pwd +
                "<br/><br/>Regards,<br/>Notes Marketplace";

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

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities())
            {
                var v = dbobj.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }
    }
}