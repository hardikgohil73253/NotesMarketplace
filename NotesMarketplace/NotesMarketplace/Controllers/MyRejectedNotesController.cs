using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace NotesMarketplace.Controllers
{
    public class MyRejectedNotesController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("MyRejectedNotes")]
        public ActionResult MyRejectedNotes(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var upobj = dbobj.UserProfile.Where(a => a.UserID == obj.ID).FirstOrDefault();
            if (upobj == null)
            {
                return RedirectToAction("UserProfile", "UserProfile");
            }

            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategort = sortby == "Category" ? "Category Desc" : "Category";

            var filtered_title = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || search == null);
            var filtered_category = dbobj.SellerNotes.Where(x => x.NoteCategories.Name.Contains(search));

            var filtered = filtered_title.Union(filtered_category);

            var entry = filtered.Where(x => x.SellerID == obj.ID && x.Status == 5).ToList().AsQueryable();

            switch (sortby)
            {
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
                    entry = entry.OrderBy(x => x.ModifiedDate);
                    break;
            }

            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(entry.ToPagedList(page ?? 1, 10));
            
        }
    }
}