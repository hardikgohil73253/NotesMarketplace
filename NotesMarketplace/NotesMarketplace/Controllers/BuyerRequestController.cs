using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;
using System.Net;
using System.Net.Mail;


namespace NotesMarketplace.Controllers
{
    public class BuyerRequestController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();
        
        [Route("BuyerRequest")]
        public ActionResult BuyerRequest(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var upobj = dbobj.UserProfile.Where(a => a.UserID == obj.ID).FirstOrDefault();
            if (upobj == null)
            {
                return RedirectToAction("UserProfile", "UserProfile");
            }

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategort = sortby == "Category" ? "Category Desc" : "Category";

            var filtered_title = dbobj.Transection.Where(x => x.Title.Contains(search) || search == null);
            var filtered_category = dbobj.Transection.Where(x => x.Category.Contains(search));

            var filtered = filtered_title.Union(filtered_category);

            //Fetching current users notes
            var entry = filtered.Where(x => x.SellerID == obj.ID && x.IsAllowed == false).ToList().AsQueryable();

            switch (sortby)
            {
                case "Date Desc":
                    entry = entry.OrderByDescending(x => x.CreatedDate);
                    break;
                case "Title":
                    entry = entry.OrderBy(x => x.Title);
                    break;
                case "Title Desc":
                    entry = entry.OrderByDescending(x => x.Title);
                    break;
                case "Category":
                    entry = entry.OrderBy(x => x.Category);
                    break;
                case "Category Desc":
                    entry = entry.OrderByDescending(x => x.Category);
                    break;
                default:
                    entry = entry.OrderBy(x => x.CreatedDate);
                    break;
            }

            var brobj = new List<BuyerRequest>();
            foreach (var item in entry)
            {
                var buyer = dbobj.UserProfile.Where(x => x.UserID == item.BuyerID).FirstOrDefault();
                brobj.Add(new BuyerRequest()
                {
                    TID = item.ID,
                    NID = item.NoteID,
                    Title = item.Title,
                    Category = item.Category,
                    BuyerEmail = item.Users.EmailID,
                    BuyerContact = buyer.PhoneNumber,
                    IsPaid = item.SellerNotes.IsPaid,
                    Price = item.Price,
                    DownloadedDate = item.CreatedDate
                });

            }
            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(brobj.ToList().AsQueryable().ToPagedList(page ?? 1, 10));
        }

        public ActionResult AllowDownload(int tid)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Transection deal = dbobj.Transection.Where(x => x.ID == tid).FirstOrDefault();
            deal.IsAllowed = true;

            dbobj.Entry(deal).State = System.Data.Entity.EntityState.Modified;
            dbobj.SaveChanges();
            NotifyBuyer(deal.Users.EmailID, deal.Users.FirstName, obj.FirstName);

            return RedirectToAction("BuyerRequest");
        }

        [Authorize]
        public void NotifyBuyer(string emailID, string Buyre, string Seller)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = Seller + " Allows you to download a note";

            string body = "Hello " + Buyre + "," +
                "<br/><br/>We would like to inform you that, " + Seller + " Allows you to download a note." +
                "Please login and see My Download tabs to download particular note." +
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
    }
}