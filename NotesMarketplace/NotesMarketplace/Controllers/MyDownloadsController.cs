using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using PagedList;


namespace NotesMarketplace.Controllers
{
    public class MyDownloadsController : Controller
    {

        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("MyDownloads")]
        public ActionResult MyDownloads(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var upobj = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();
            if(upobj==null)
            {
                return RedirectToAction("UserProfile", "UserProfile");
            }

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategort = sortby == "Category" ? "Category Desc" : "Category";

            var filtered_title = dbobj.Transection.Where(x => x.Title.Contains(search) || search == null);
            var filtered_category = dbobj.Transection.Where(x => x.Category.Contains(search));

            var filtered = filtered_title.Union(filtered_category);

            var entry = filtered.Where(x => x.BuyerID == obj.ID && x.IsAllowed == true).ToList().AsQueryable();

            switch (sortby)
            {
                case "Date Desc":
                    entry = entry.OrderByDescending(x => x.DownloadDate);
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
                    entry = entry.OrderBy(x => x.DownloadDate);
                    break;
            }

            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(entry.ToPagedList(page ?? 1, 10));

        }

        public ActionResult AddReview(int nid, int rate, string Comments)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var oldreview = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid && x.ReviewedByID == obj.ID).FirstOrDefault();

            if (oldreview == null)   //New Review
            {
                SellerNotesReviews review = new SellerNotesReviews();

                review.NoteID = nid;
                review.ReviewedByID = obj.ID;
                review.Ratings = rate;
                review.Comments = Comments;
                review.CreatedDate = DateTime.Now;

                dbobj.SellerNotesReviews.Add(review);
                dbobj.SaveChanges();

                // Adding Ratings in note table

                var book = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

                int total_reviews = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid).Count();
                decimal total_stars =dbobj.SellerNotesReviews.Where(x => x.NoteID == nid).Select(x => x.Ratings).Sum();

                book.TotalReviews = total_reviews;
                book.Rating = ((double)total_stars / total_reviews) * 20;

                dbobj.Entry(book).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                // ------------------------------------------------------------

                return RedirectToAction("MyDownloads");
            }
            else   //Update Review
            {
                oldreview.Ratings = rate;
                oldreview.Comments = Comments;

                dbobj.Entry(oldreview).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                // Adding Ratings in note table

                var book = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

                int total_reviews = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid).Count();
                decimal total_stars = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid).Select(x => x.Ratings).Sum();

                book.TotalReviews = total_reviews;
                book.Rating = ((double)total_stars / total_reviews) * 20;

                dbobj.Entry(book).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                // ------------------------------------------------------------

                return RedirectToAction("MyDownloads");
            }

        }

        public ActionResult SpamReport(int nid, string SpamComments)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var oldspam = dbobj.SpamTable.Where(x => x.NoteID == nid && x.SpamBy == obj.ID).FirstOrDefault();

            var book = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

            Users sellerobj = dbobj.Users.Where(x => x.ID == book.SellerID).FirstOrDefault();

            if (oldspam == null)    //New Spam
            {
                SpamTable spam = new SpamTable();
                spam.NoteID = nid;
                spam.SpamBy = obj.ID;
                spam.Comments = SpamComments;
                spam.CreatedDate = DateTime.Now;

                dbobj.SpamTable.Add(spam);
                dbobj.SaveChanges();

                // Adding Ratings in note table

                int total_spams = dbobj.SpamTable.Where(x => x.ID == nid).Count();

                book.TotalSpams = total_spams;

                dbobj.Entry(book).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                // ------------------------------------------------------------

                NotifyAdmin(obj.FirstName, book.Title, sellerobj.FirstName);

                return RedirectToAction("MyDownloads");
            }
            else    //Update Old Spam
            {
                oldspam.Comments = SpamComments;

                dbobj.Entry(oldspam).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                NotifyAdmin(obj.FirstName, book.Title, sellerobj.FirstName);

                return RedirectToAction("MyDownloads");
            }

        }

        public void NotifyAdmin(string ReportedBy, string NoteTitle, string SellerName)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com");//Support Email Address
            var toEmail = new MailAddress("gohilhardik73253@gmail.com");
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = ReportedBy + " Reported an issue for " + NoteTitle;

            string body = "Hello Admins, " +
                "<br/><br/>We want to inform you that, " + ReportedBy + " Reported an issue for " +
                SellerName + "’s Note with title " + NoteTitle + ". Please look at the notes and take required actions." +
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