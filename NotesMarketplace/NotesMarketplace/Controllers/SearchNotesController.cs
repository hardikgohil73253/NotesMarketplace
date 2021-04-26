using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Data.Entity;

namespace NotesMarketplace.Controllers
{
    public class SearchNotesController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [Route("SearchNotes")]
        public ActionResult SearchNotes(string search, string NoteType, string Category, string UniversityName, string Course, string Country, string Rating, int? page)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if(obj!= null)
            {
                var upobj = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();
                if(upobj == null)
                {
                    return RedirectToAction("UserProfile", "UserProfile");
                }
            }

            System.Linq.IQueryable<SellerNotes> filtered;

            if (String.IsNullOrEmpty(search) && String.IsNullOrEmpty(NoteType) && String.IsNullOrEmpty(Category)
                && String.IsNullOrEmpty(UniversityName) && String.IsNullOrEmpty(Course) && String.IsNullOrEmpty(Country) && String.IsNullOrEmpty(Rating))
            {
                filtered = dbobj.SellerNotes.Where(x => x.Status == 4 && x.IsActive == true).ToList().AsQueryable();
            }
            else
            {
                if (String.IsNullOrEmpty(search))   // Search is empty + Dropdown
                {
                    var filtered_type = dbobj.SellerNotes.Where(x => x.NoteType.ToString() == NoteType);
                    var filtered_category = dbobj.SellerNotes.Where(x => x.Category.ToString() == Category);
                    var filtered_institute = dbobj.SellerNotes.Where(x => x.UniversityName == UniversityName);
                    var filtered_course = dbobj.SellerNotes.Where(x => x.Course == Course);
                    var filtered_country = dbobj.SellerNotes.Where(x => x.Country.ToString() == Country);

                    var IntRating = Rating == "" ? 6 : Int32.Parse(Rating);
                    var filtered_rating = dbobj.SellerNotes.Where(x => x.Rating >= IntRating * 20);

                    filtered = filtered_type.Union(filtered_category).Union(filtered_institute).Union(filtered_course).Union(filtered_country).Union(filtered_rating).Where(x => x.Status == 4 && x.IsActive == true).ToList().AsQueryable();
                }
                else    // Search + Dropdown
                {
                    var filtered_title = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || x.NoteTypes.Name.Contains(search) || x.NoteCategories.Name.Contains(search) ||
                        x.UniversityName.Contains(search) || x.Course.Contains(search) || x.Countries.Name.Contains(search));
                    var filtered_type = dbobj.SellerNotes.Where(x => x.NoteType.ToString() == NoteType);
                    var filtered_category = dbobj.SellerNotes.Where(x => x.Category.ToString() == Category);
                    var filtered_institute = dbobj.SellerNotes.Where(x => x.UniversityName == UniversityName);
                    var filtered_course = dbobj.SellerNotes.Where(x => x.Course == Course);
                    var filtered_country = dbobj.SellerNotes.Where(x => x.Country.ToString() == Country);

                    var IntRating = Rating == "" ? 6 : Int32.Parse(Rating);
                    var filtered_rating = dbobj.SellerNotes.Where(x => x.Rating >= IntRating * 20);

                    filtered = filtered_title.Union(filtered_type).Union(filtered_category).Union(filtered_institute).Union(filtered_course).Union(filtered_country).Union(filtered_rating).Where(x => x.Status == 4 && x.IsActive == true).ToList().AsQueryable();
                }
            }
            ViewBag.TotalBooks = filtered.Count();

            if (obj != null)
            {
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            }

            ViewBag.Type = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.NoteTypes).Distinct().ToList(), "ID", "Name");
            ViewBag.Category = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.NoteCategories).Distinct().ToList(), "ID", "Name");
            ViewBag.Univercity = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.UniversityName).Distinct().ToList());
            ViewBag.Course = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.Course).Distinct().ToList());
            ViewBag.Country = new SelectList(dbobj.SellerNotes.Where(x => x.IsActive).Select(x => x.Countries).Distinct().ToList(), "ID", "Name");

            return View(filtered.ToPagedList(page ?? 1, 9));
            
        }
    }
}