using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using PagedList;
using System.Data.Entity;
using System.IO;


namespace NotesMarketplace.Controllers
{
    public class AdminController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();


        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        [Route("ManageAdmin")]
        public ActionResult ManageAdmin(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortFirstName = sortby == "FirstName" ? "FirstName Desc" : "FirstName";
            ViewBag.SortLastName = sortby == "LastName" ? "LastName Desc" : "LastName";
            ViewBag.SortEmail = sortby == "Email" ? "Email Desc" : "Email";

            System.Linq.IQueryable<NotesMarketplace.Users> filtered;

            if (String.IsNullOrEmpty(search))   //  All Members
            {
                //  All Members
                filtered = dbobj.Users.Where(x => x.IsActive == true || x.IsActive == false).ToList().AsQueryable();
            }
            else
            {
                filtered = dbobj.Users.Where(x => (x.FirstName.Contains(search) || x.LastName.Contains(search) ||
                x.EmailID.Contains(search) || (x.CreatedData.Value.Day + "-" + x.CreatedData.Value.Month + "-" + x.CreatedData.Value.Year).Contains(search))).ToList().AsQueryable();
            }

            var entry = filtered.Where(x => x.RoleID == 2).ToList().AsQueryable();

            switch (sortby)
            {
                case "Date Desc":
                    entry = entry.OrderByDescending(x => x.CreatedData);
                    break;
                case "FirstName":
                    entry = entry.OrderBy(x => x.FirstName);
                    break;
                case "FirstName Desc":
                    entry = entry.OrderByDescending(x => x.FirstName);
                    break;
                case "LastName":
                    entry = entry.OrderBy(x => x.LastName);
                    break;
                case "LastName Desc":
                    entry = entry.OrderByDescending(x => x.LastName);
                    break;
                case "Email":
                    entry = entry.OrderBy(x => x.EmailID);
                    break;
                case "Email Desc":
                    entry = entry.OrderByDescending(x => x.EmailID);
                    break;
                default:
                    entry = entry.OrderBy(x => x.CreatedData);
                    break;
            }

            //Converting entry into Manage Administrator Model
            var maobj = new List<ManageAdministrator>();
            foreach (var item in entry)
            {
                maobj.Add(new ManageAdministrator()
                {
                    UserID = item.ID,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.EmailID,
                    PhoneNumber = dbobj.Admin.Where(x => x.UserID == item.ID).Select(x => x.PhoneNumber).FirstOrDefault(),
                    CreatedDate = item.CreatedData,
                    IsActive = item.IsActive
                }); ;

            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(maobj.ToPagedList(page ?? 1, 5));
        }


        [Authorize(Roles = "Super Admin")]
        [Route("UpdateAdminStatus")]
        public ActionResult UpdateAdminStatus(int uid, int status)
        {
            var emailid = User.Identity.Name.ToString();
            Users superadmin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Users obj = dbobj.Users.Where(x => x.ID == uid).FirstOrDefault();
            Admin adobj = dbobj.Admin.Where(x => x.UserID == uid).FirstOrDefault();

            if (status == 0)    // Deactivate Admin
            {
                obj.IsActive = false;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = superadmin.ID;

                adobj.IsActive = false;
                adobj.ModifiedDate = DateTime.Now;
                adobj.ModifiedBy = superadmin.ID;
            }
            else    // Activate Admin
            {
                obj.IsActive = true;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = superadmin.ID;

                adobj.IsActive = true;
                adobj.ModifiedDate = DateTime.Now;
                adobj.ModifiedBy = superadmin.ID;
            }

            dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            dbobj.Entry(adobj).State = System.Data.Entity.EntityState.Modified;
            dbobj.SaveChanges();

            return RedirectToAction("ManageAdmin");
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AdminDashboard")]
        public ActionResult AdminDashboard(string search, int? page, string sortby, string Month)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategort = sortby == "Category" ? "Category Desc" : "Category";

            System.Linq.IQueryable<SellerNotes> filtered;     //Empty Variable for Holding Notes

            if (String.IsNullOrEmpty(search) && String.IsNullOrEmpty(Month))   //  All Books
            {
                var filtered_title = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || search == null);
                var filtered_category = dbobj.SellerNotes.Include(x=>x.NoteCategories).Where(x => x.NoteCategories.Name.Contains(search));

                filtered = filtered_title.Union(filtered_category);
            }
            else
            {
                if (String.IsNullOrEmpty(search))   // Search is empty + Dropdown
                {
                    var filtered_month = dbobj.SellerNotes.Where(x => x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year == Month);
                    filtered = filtered_month;
                }
                else    // Search + Dropdown
                {
                    var filtered_title = dbobj.SellerNotes.Where(x => x.Title.Contains(search) || search == null);
                    var filtered_category = dbobj.SellerNotes.Include(x=>x.NoteCategories).Where(x => x.NoteCategories.Name.Contains(search));
                    var filtered_month = dbobj.SellerNotes.Where(x => x.ModifiedDate.Value.Month + "-" + x.ModifiedDate.Value.Year == Month);

                    filtered = filtered_title.Union(filtered_category).Union(filtered_month);
                }
            }

            var entry = filtered.Where(x => x.Status == 4).Include(x => x.ReferenceData).ToList().AsQueryable();

            switch (sortby)
            {
                case "Date Desc":
                    entry = entry.OrderByDescending(x => x.ModifiedDate);
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
                    entry = entry.OrderBy(x => x.ModifiedDate);
                    break;
            }
            //Converting entry into Admin Dashboard Model
            var adobj = new List<AdminDashboard>();
            foreach (var item in entry)
            {
                DirectoryInfo info = new DirectoryInfo(Server.MapPath("~/Members/" + item.SellerID + "/" + item.ID + "/" + "Attachment"));
                long totalSize = info.EnumerateFiles().Sum(file => file.Length);    // Bytes
                totalSize = (long)totalSize / 1024;   // KB
                string file_size = totalSize + " KB";
                if (totalSize >= 1024)
                {
                    totalSize = (long)totalSize / 1024; // MB
                    file_size = totalSize + " MB";
                }
                adobj.Add(new AdminDashboard()
                {
                    ID = item.ID,
                    Title = item.Title,
                    Category = item.NoteCategories.Name,
                    FileSize = file_size,
                    SellType = item.IsPaid == true ? "Paid" : "Free",
                    Price = item.SellingPrice,
                    Publisher = dbobj.Users.Where(x => x.ID == item.ActionedBy).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
                    PublishedDate = item.ModifiedDate,
                    TotalDownloads = dbobj.Transection.Where(x => x.NoteID == item.ID && x.IsDownloaded == true).Count()
                });

            }
            List<SelectListItem> MonthList = new List<SelectListItem>();

            for (int i = 0; i <= 5; i++)
            {
                var previousDate = DateTime.Now.AddMonths(-i);
                MonthList.Add(new SelectListItem()
                {
                    Text = previousDate.Date.ToString("MMMM") + " " + previousDate.Year.ToString(),
                    Value = previousDate.Month.ToString() + "-" + previousDate.Year.ToString()
                });
            }

            ViewBag.Month = MonthList;
            ViewBag.InReview = dbobj.SellerNotes.Where(x => x.Status == 3).Count();
            var seven_day_before = DateTime.Now.AddDays(-7);
            Nullable<bool> t = true;
            ViewBag.NotesDownloaded = dbobj.Transection.Where(x => x.IsDownloaded==t && (x.DownloadDate >= seven_day_before)).Count();
            ViewBag.NewRegistrations = dbobj.Users.Where(x => x.RoleID == 3 && (x.CreatedData >= seven_day_before)).Count();

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(adobj.ToPagedList(page ?? 1, 5));

        }

        [Route("AdminNoteDetails")]
        public ActionResult AdminNoteDetails(int nid)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            SellerNotes Note = dbobj.SellerNotes.Where(x => x.ID == nid).FirstOrDefault();

            // Code for Customer Review

            var reviews = dbobj.SellerNotesReviews.Where(x => x.NoteID == nid);
            var ReviewsList = new List<DisplayReview>();

            foreach (var item in reviews)
            {
                var ReviewByUser = dbobj.UserProfile.Where(x => x.UserID == item.ReviewedByID).FirstOrDefault();
                ReviewsList.Add(new DisplayReview()
                {
                    ReviewID = item.ID,
                    ReviewBy = ReviewByUser.Users.FirstName,
                    UserImage = ReviewByUser.ProfilePicture,
                    Stars = item.Ratings * 20,
                    Comment = item.Comments
                });
            }
            // ==================================================
            ViewBag.Reviews = ReviewsList;
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            ViewBag.Reviews = ReviewsList.OrderByDescending(x => x.Stars);
            return View(Note);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        [Route("EditAdmin")]
        public ActionResult EditAdmin(int uid)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Users adobj = dbobj.Users.Where(x => x.ID == uid).FirstOrDefault();
            Admin admin_profile = dbobj.Admin.Where(x => x.UserID == uid).FirstOrDefault();

            AddAdmin model = new AddAdmin();

            model.UserID = adobj.ID;
            model.FirstName = adobj.FirstName;
            model.LastName = adobj.LastName;
            model.Email = adobj.EmailID;
            model.CountryCode = admin_profile.CountryCode;
            model.PhoneNumber = admin_profile.PhoneNumber;

            ViewBag.CountryCodelist = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        [Route("EditAdmin")]
        public ActionResult EditAdmin(Models.AddAdmin model)
        {
            var emailid = User.Identity.Name.ToString();
            Users superadmin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Users old_adobj = dbobj.Users.Where(x => x.ID == model.UserID).FirstOrDefault();
                if (old_adobj.EmailID != model.Email)
                {
                    var isExist = IsEmailExist(model.Email);
                    if (isExist)
                    {
                        ModelState.AddModelError("Email", "Email already exist");
                        ViewBag.CountryCodelist = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
                        return View(model);
                    }
                }

                Admin old_admin_profile = dbobj.Admin.Where(x => x.UserID == model.UserID).FirstOrDefault();

                old_adobj.FirstName = model.FirstName;
                old_adobj.LastName = model.LastName;
                old_adobj.EmailID = model.Email;
                old_adobj.ModifiedDate = DateTime.Now;
                old_adobj.ModifiedBy = superadmin.ID;

                old_admin_profile.CountryCode = model.CountryCode;
                old_admin_profile.PhoneNumber = model.PhoneNumber;
                old_admin_profile.ModifiedDate = DateTime.Now;
                old_admin_profile.ModifiedBy = superadmin.ID;
                

                dbobj.Entry(old_adobj).State = System.Data.Entity.EntityState.Modified;
                dbobj.Entry(old_admin_profile).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                return RedirectToAction("ManageAdmin", "Admin");
            }
            return View();
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