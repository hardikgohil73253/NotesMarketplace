using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NotesMarketplace.Models;
using System.Web.Helpers;

namespace NotesMarketplace.Controllers
{
    public class SignUpController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();


        [HttpGet]
        [Route("SignUp")]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("SignUp")]
        public ActionResult SignUp(Models.UsersModel model)
        {
            if (ModelState.IsValid)
            {
                var isExist= IsEmailExist(model.EmailID);
                if(isExist)
                {
                    ModelState.AddModelError("Email", "Email already exist");
                    return View(model);
                }

                Users obj = new Users();
                obj.RoleID = 3;
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.EmailID = model.EmailID;
                obj.Password = model.Password;
                obj.IsEmailVerified = model.IsEmailVerified;
                obj.IsActive = model.IsActive;
                obj.SecretCode = Guid.NewGuid();

                dbobj.Users.Add(obj);
                dbobj.SaveChanges();
                SendVerificationLinkEmail(model.EmailID, model.FirstName, obj.SecretCode.ToString());
                TempData["Success"]= "Your account has been successfully created.";
                
            }
            ModelState.Clear();

            return RedirectToAction("SignUp");

        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            using(NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities())
            {
                var v= dbobj.Users.Where(a => a.EmailID == email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string username, string SecretCode)
        {
            var verifyUrl = "/EmailVerification/" + SecretCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("gohilhardik087@gmail.com");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = "Note Marketplace - Email Verification";

            string body = "Hello " + username + "," +
                "<br/><br/>Thank you for signing up with us. Please click on below link to verify your email address and to do login." +
                "<br/><br/><a href='" + link + "'>" + link + "</a> " +
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

        [Route("EmailVerification/{code}")]
        public ActionResult EmailVerification(string code)
        {
            Users obj = dbobj.Users.Where(x => x.SecretCode.ToString() == code).FirstOrDefault();
            ViewBag.name = obj.FirstName;
            return View(obj);
        }

        [Route("Verify/{code}")]
        public ActionResult Verify(string code)
        {
            Users obj = dbobj.Users.Where(x => x.SecretCode.ToString() == code).FirstOrDefault();
            obj.IsEmailVerified = true;
            dbobj.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("Login")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
        public ActionResult Login(Models.UsersLoginModel login, string ReturnUrl="")
        {
            using(NotesMarketplaceEntities dbobj= new NotesMarketplaceEntities())
            {
                var v = dbobj.Users.Where(a => a.EmailID == login.EmailId).FirstOrDefault();
                if(v!= null)
                {
                    if(v.IsEmailVerified == true)
                    {
                        if(string.Compare(login.Password,v.Password)==0)
                        {
                            int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                            var ticket = new FormsAuthenticationTicket(login.EmailId, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            if (v.RoleID == 3) //normal user
                            {
                                var upobj = dbobj.UserProfile.Where(a => a.UserID == v.ID).FirstOrDefault();
                                if (upobj == null)
                                {
                                    return RedirectToAction("UserProfile", "UserProfile");
                                }
                                else if (!String.IsNullOrEmpty(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("SearchNotes", "SearchNotes");
                                }
                            }
                            else //admin
                            {
                                var upobj = dbobj.Admin.Where(a => a.UserID == v.ID).FirstOrDefault();
                                if (upobj == null)
                                {
                                    return RedirectToAction("AdminProfile", "AdminProfile");
                                }
                                else
                                {
                                    return RedirectToAction("AdminDashboard","Admin");
                                }
                            }

                        }
                        else
                        {
                            //message = "Invalid Password";
                            ModelState.AddModelError("Password", "The password that you've entered is incorrect");
                            return View(login);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailID", "Email is not verified");
                        return View(login);
                    }
                }
                else
                {
                    //message = "Invalid Email";
                    ModelState.AddModelError("EmailID", "Invalid Email");
                    return View(login);
                }
            }
        }


        [Authorize]
        [Route("Logout")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        [HttpGet]
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public ActionResult ForgotPassword(Models.ForgotPassword model)
        {
            if(ModelState.IsValid)
            {
                var isExist = IsEmailExist(model.EmailID);
                if(isExist==false)
                {
                    ModelState.AddModelError("EmailID", "Email does not Exist");
                    return View(model);
                }
                Users obj=dbobj.Users.Where(x => x.EmailID == model.EmailID).FirstOrDefault();
                String pwd = Membership.GeneratePassword(6, 2);
                obj.Password = pwd;
                dbobj.SaveChanges();
                SendPassword(obj.EmailID, pwd);
                TempData["Success"] = "New password has been sent to your email";
            }
            return RedirectToAction("Login");
        }

        [NonAction]
        public void SendPassword(string emailID, string pwd)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = "Note Marketplace - Forgot Password";

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

        [Authorize]
        [Route("ChangePassword")]
        public ActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [Route("ChangePassword")]
        public ActionResult ChangePassword(Models.ChangePassword model)
        {
            var emailID = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailID).FirstOrDefault();
            if(ModelState.IsValid)
            {
                if(String.Compare(model.OldPassword,obj.Password)==0)
                {
                    obj.Password = model.NewPassword;
                    obj.ModifiedDate = DateTime.Now;

                    dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                    dbobj.SaveChanges();

                    FormsAuthentication.SignOut();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "OldPassword Is Incorrect");
                }
            }
            return View();
        }
    }
}