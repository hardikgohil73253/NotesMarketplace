using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace NotesMarketplace.Controllers
{
    public class SpamReportsController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        
        [Route("SpamReports")]
        public ActionResult SpamReports(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortReportedBy = sortby == "ReportedBy" ? "ReportedBy Desc" : "ReportedBy";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategory = sortby == "Category" ? "Category Desc" : "Category";

            System.Linq.IQueryable<NotesMarketplace.SpamTable> filtered;

            if (String.IsNullOrEmpty(search))   //  All Spam Reports
            {
                //  All Spam Reports
                filtered = dbobj.SpamTable.ToList().AsQueryable();
            }
            else
            {
                filtered = dbobj.SpamTable.Where(x => (x.SellerNotes.Title.Contains(search) || x.SellerNotes.Title.Contains(search) ||
                 x.SellerNotes.NoteCategories.Name.Contains(search) ||
                (x.CreatedDate.Value.Day + "-" + x.CreatedDate.Value.Month + "-" + x.CreatedDate.Value.Year).Contains(search) )).ToList().AsQueryable();
            }

            switch (sortby)
            {
                case "Date Desc":
                    filtered = filtered.OrderByDescending(x => x.CreatedDate);
                    break;
                
                    break;
                case "Title":
                    filtered = filtered.OrderBy(x => x.SellerNotes.Title);
                    break;
                case "Title Desc":
                    filtered = filtered.OrderByDescending(x => x.SellerNotes.Title);
                    break;
                case "Category":
                    filtered = filtered.OrderBy(x => x.SellerNotes.NoteCategories.Name);
                    break;
                case "TitCategoryle Desc":
                    filtered = filtered.OrderByDescending(x => x.SellerNotes.NoteCategories.Name);
                    break;
                default:
                    filtered = filtered.OrderBy(x => x.CreatedDate);
                    break;
            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(filtered.ToPagedList(page ?? 1, 5));
        }

        
        [Route("DeleteSpamReports")]
        public ActionResult DeleteSpamReports(int sid)
        {
            SpamTable report = dbobj.SpamTable.Where(x => x.ID == sid).FirstOrDefault();
            int nid = report.NoteID;

            dbobj.SpamTable.Remove(report);
            dbobj.SaveChanges();

            var book = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

            int total_spams = dbobj.SpamTable.Where(x => x.NoteID == nid).Count();

            book.TotalSpams = total_spams;

            dbobj.Entry(book).State = System.Data.Entity.EntityState.Modified;
            dbobj.SaveChanges();

            return RedirectToAction("SpamReports");
        }
    }
}