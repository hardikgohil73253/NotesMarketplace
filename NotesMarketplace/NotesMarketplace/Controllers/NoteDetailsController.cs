using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using Ionic.Zip;

namespace NotesMarketplace.Controllers
{
    public class NoteDetailsController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("NoteDetails")]
        public ActionResult NoteDetails(int nid)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            SellerNotes Note = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

            if(obj!= null)
            {
                Transection deal = dbobj.Transection.Where(x => x.ID == nid && x.BuyerID == obj.ID).FirstOrDefault();
                ViewBag.CurrentUserID = obj.ID;
                ViewBag.CurrentUserName = obj.FirstName;
                if (deal != null) { ViewBag.IsDealAvailable = deal.IsAllowed; }

                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();

            }

            var reviews = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid);
            var ReviewsList = new List<DisplayReview>();

            foreach(var item in reviews)
            {
                var ReviewByUser = dbobj.UserProfile.Where(x => x.UserID == item.ReviewedByID).FirstOrDefault();
                ReviewsList.Add(new DisplayReview()
                {
                    ReviewBy = ReviewByUser.Users.FirstName,
                    UserImage = ReviewByUser.ProfilePicture,
                    Stars = item.Ratings * 20,
                    Comment = item.Comments
                });
            }

            ViewBag.Reviews = ReviewsList.OrderByDescending(x => x.Stars);
            return View(Note);
        }



        [Route("Download")]
        public ActionResult Download(int nid)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            SellerNotes noteobj = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

            Users sellerobj = dbobj.Users.Where(x => x.ID == noteobj.SellerID).FirstOrDefault();

            Transection deal = dbobj.Transection.Where(x => x.NoteID == nid && x.BuyerID == obj.ID).FirstOrDefault();

            if (obj.ID == noteobj.SellerID)     // Users own book
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(Server.MapPath("~/Members/" + noteobj.SellerID + "/" + nid + "/" + "Attachment"));

                    MemoryStream output = new MemoryStream();
                    zip.Save(output);
                    return File(output.ToArray(), "Attachment/zip", noteobj.Title + ".zip");
                }
            }
            else
            {
                if (deal == null)   // New Transection
                {
                    if (noteobj.IsPaid == false)    // Download Free Notes
                    {
                        Transection tobj = new Transection();
                        tobj.NoteID = nid;
                        tobj.Title = noteobj.Title;
                        tobj.Category = noteobj.NoteCategories.Name;
                        tobj.IsPaid = false;
                        tobj.Price = noteobj.SellingPrice;
                        tobj.BuyerID = obj.ID;
                        tobj.SellerID = noteobj.SellerID;
                        tobj.IsAllowed = true;
                        tobj.IsDownloaded = true;
                        tobj.DownloadDate = DateTime.Now;
                        tobj.Status = noteobj.ReferenceData.Value;
                        tobj.CreatedDate = DateTime.Now;

                        dbobj.Transection.Add(tobj);
                        dbobj.SaveChanges();

                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddDirectory(Server.MapPath("~/Members/" + noteobj.SellerID + "/" + nid + "/" + "Attachment"));

                            MemoryStream output = new MemoryStream();
                            zip.Save(output);
                            return File(output.ToArray(), "Attachment/zip", noteobj.Title + ".zip");
                        }

                    }
                    else    // Download Paid Notes
                    {
                        Transection tobj = new Transection();
                        tobj.NoteID = nid;
                        tobj.Title = noteobj.Title;
                        tobj.Category = noteobj.NoteCategories.Name;
                        tobj.IsPaid = true;
                        tobj.Price = noteobj.SellingPrice;
                        tobj.BuyerID = obj.ID;
                        tobj.SellerID = noteobj.SellerID;
                        tobj.IsAllowed = false;
                        tobj.IsDownloaded = false;
                        tobj.DownloadDate = null;
                        tobj.Status = noteobj.ReferenceData.Value;
                        tobj.CreatedDate = DateTime.Now;

                        dbobj.Transection.Add(tobj);
                        dbobj.SaveChanges();
                        NotifySeller(sellerobj.EmailID, obj.FirstName, sellerobj.FirstName);
                        //return RedirectToAction("NoteDetails", new { nid });
                        ViewBag.CurrentUserName = obj.FirstName;
                        return RedirectToAction("NoteDetails", new { nid });
                        //return PartialView("ThanksPopup", noteobj);
                    }
                }
                else   // Old Transection Available
                {
                    if (noteobj.IsPaid == false)    // Download Free Notes
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddDirectory(Server.MapPath("~/Members/" + noteobj.SellerID + "/" + nid + "/" + "Attachment"));

                            MemoryStream output = new MemoryStream();
                            zip.Save(output);
                            return File(output.ToArray(), "Attachment/zip", noteobj.Title + ".zip");
                        }

                    }
                    else    // Download Paid Notes
                    {
                        if ((bool)deal.IsAllowed)
                        {
                            deal.IsDownloaded = true;
                            deal.DownloadDate = DateTime.Now;

                            dbobj.Entry(deal).State = System.Data.Entity.EntityState.Modified;
                            dbobj.SaveChanges();

                            using (ZipFile zip = new ZipFile())
                            {
                                zip.AddDirectory(Server.MapPath("~/Members/" + noteobj.SellerID + "/" + nid + "/" + "Attachment"));

                                MemoryStream output = new MemoryStream();
                                zip.Save(output);
                                return File(output.ToArray(), "Attachment/zip", noteobj.Title + ".zip");
                            }
                        }
                        else
                        {
                            return RedirectToAction("NoteDetails", nid);
                        }

                    }
                }
            }

        }

        [Authorize]
        public void NotifySeller(string emailID, string Buyre, string Seller)
        {
            var fromEmail = new MailAddress("gohilhardik087@gmail.com");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Hardik@017018"; // Replace with actual password
            string subject = Buyre + " wants to purchase your notes";

            string body = "Hello " + Seller + "," +
                "<br/><br/>We would like to inform you that, " + Buyre + " wants to purchase your notes. Please see " +
                "Buyer Requests tab and allow download access to Buyer if you have received the payment from him." +
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