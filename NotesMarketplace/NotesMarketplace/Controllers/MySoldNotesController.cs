using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace NotesMarketplace.Controllers
{
    public class MySoldNotesController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("MySoldNotes")]
        public ActionResult MySoldNotes(string search, int? page, string sortby)
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

            var entry = filtered.Where(x => x.SellerID == obj.ID && x.IsAllowed == true).ToList().AsQueryable();

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
    }
}