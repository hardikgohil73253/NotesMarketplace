using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Data.Entity;
using NotesMarketplace.Models;

namespace NotesMarketplace.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();
        [Route("Dashboard")]
        public ActionResult Dashboard(string search, string search2, int? page, int? page2, string sortby, string sortby2)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategory = sortby == "Category" ? "Category Desc" : "Category";

            ViewBag.SortDate2 = string.IsNullOrEmpty(sortby2) ? "Date Desc" : "";
            ViewBag.SortTitle2 = sortby2 == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategory2 = sortby2 == "Category" ? "Category Desc" : "Category";

            Dashboard Dashboard = new Dashboard();


            var filtered_title = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || search == null);
            var filtered_status = dbobj.SellerNotes.Include(x => x.NoteCategories).Where(x => x.ReferenceData.Value.Contains(search));
            var filtered_category = dbobj.SellerNotes.Include(x => x.NoteCategories).Where(x => x.NoteCategories.Name.Contains(search));

            var filtered = filtered_title.Union(filtered_status).Union(filtered_category);

            var entry = filtered.Where(x => x.SellerID == obj.ID && (x.Status == 1 || x.Status == 2 || x.Status == 3)).Include(x => x.ReferenceData).ToList().AsQueryable();



            var filtered_title2 = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || search == null);
            var filtered_status2 = dbobj.SellerNotes.Include(x => x.NoteCategories).Where(x => x.ReferenceData.Value.Contains(search));
            var filtered_category2 = dbobj.SellerNotes.Include(x => x.NoteCategories).Where(x => x.NoteCategories.Name.Contains(search));

            var filtered2 = filtered_title2.Union(filtered_status2).Union(filtered_category2);

            var entry2 = filtered2.Where(x => x.SellerID == obj.ID && (x.Status == 4) ).Include(x => x.ReferenceData).ToList().AsQueryable();

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
                    entry = entry.OrderBy(x => x.NoteCategories.Name);
                    break;
                case "Category Desc":
                    entry = entry.OrderByDescending(x => x.NoteCategories.Name);
                    break;
                default:
                    entry = entry.OrderBy(x => x.CreatedDate);
                    break;
            }

            switch (sortby2)
            {
                case "Date Desc":
                    entry2 = entry2.OrderByDescending(x => x.CreatedDate);
                    break;
                case "Title":
                    entry2 = entry2.OrderBy(x => x.Title);
                    break;
                case "Title Desc":
                    entry2 = entry2.OrderByDescending(x => x.Title);
                    break;
                case "Category":
                    entry2 = entry2.OrderBy(x => x.NoteCategories.Name);
                    break;
                case "Category Desc":
                    entry2 = entry2.OrderByDescending(x => x.NoteCategories.Name);
                    break;
                default:
                    entry2 = entry2.OrderBy(x => x.CreatedDate);
                    break;
            }

            Dashboard.Progress = entry.ToPagedList(page ?? 1, 5);
            Dashboard.Published = entry2.ToPagedList(page2 ?? 1, 5);

            ViewBag.SoldNotes = dbobj.Transection.Where(x => x.SellerID == obj.ID && x.IsAllowed == true).Count();
            if (ViewBag.SoldNotes == 0)
            {
                ViewBag.Earning = 0;
            }
            else
            {
                ViewBag.Earning = dbobj.Transection.Where(x => x.SellerID == obj.ID && x.IsAllowed == true).Select(x => x.Price).Sum();
            }
            ViewBag.DownloadNotes = dbobj.Transection.Where(x => x.BuyerID == obj.ID && x.IsDownloaded == true).Count();
            ViewBag.BuyerRequests = dbobj.Transection.Where(x => x.SellerID == obj.ID && x.IsAllowed == false).Count();

            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(Dashboard);
        }

        [Route("DeleteBook/{noteid}")]
        public ActionResult DeleteBook(int noteid)
        {

            SellerNotesAttachements attachment = dbobj.SellerNotesAttachements.Where(x => x.NoteID == noteid).FirstOrDefault();
            SellerNotes noteobj = dbobj.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();

            string mappedPath = Server.MapPath("/Members/" + noteobj.SellerID + "/" + noteid);
            Directory.Delete(mappedPath, true);

            dbobj.SellerNotesAttachements.Remove(attachment);
            dbobj.SellerNotes.Remove(noteobj);
            dbobj.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}