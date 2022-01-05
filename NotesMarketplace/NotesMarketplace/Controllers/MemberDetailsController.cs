using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using NotesMarketplace.Models;

namespace NotesMarketplace.Controllers
{
    public class MemberDetailsController : Controller
    {

        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("MemberDetails")]
        public ActionResult MemberDetails(int uid, int? page, string sortby)
        {
            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortTitle = sortby == "Title" ? "Title Desc" : "Title";
            ViewBag.SortCategory = sortby == "Category" ? "Category Desc" : "Category";
            ViewBag.SortStatus = sortby == "Status" ? "Status Desc" : "Status";
            ViewBag.SortDateAdded = sortby == "DateAdded" ? "DateAdded Desc" : "DateAdded";

            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Users user = dbobj.Users.Where(x => x.ID == uid).FirstOrDefault();
            UserProfile user_profile = dbobj.UserProfile.Where(x => x.UserID == uid).FirstOrDefault();

            MemberDetails Member = new MemberDetails();

            MemberProfile member_profile = new MemberProfile();

            member_profile.UserID = user.ID;
            member_profile.FirstName = user.FirstName;
            member_profile.LastName = user.LastName;
            member_profile.Email = user.EmailID;
            member_profile.ProfilePicture = user_profile.ProfilePicture;
            member_profile.DateOfBirth = user_profile.DOB;
            member_profile.PhoneNumber = user_profile.PhoneNumber;
            member_profile.University = user_profile.University;
            member_profile.AddressLine1 = user_profile.AddressLine1;
            member_profile.AddressLine2 = user_profile.AddressLine2;
            member_profile.City = user_profile.City;
            member_profile.State = user_profile.State;
            member_profile.Country = user_profile.Countries.Name;
            member_profile.ZipCode = user_profile.ZipCode;

            var notes = dbobj.SellerNotes.Where(x => x.SellerID == uid && x.Status != 1).ToList().AsQueryable();

            switch (sortby)
            {
                case "Date Desc":
                    notes = notes.OrderByDescending(x => x.ModifiedDate);
                    break;
                case "Title":
                    notes = notes.OrderBy(x => x.Title);
                    break;
                case "Title Desc":
                    notes = notes.OrderByDescending(x => x.Title);
                    break;
                case "Category":
                    notes = notes.OrderBy(x => x.NoteCategories.Name);
                    break;
                case "Category Desc":
                    notes = notes.OrderByDescending(x => x.NoteCategories.Name);
                    break;
                case "Status":
                    notes = notes.OrderBy(x => x.ReferenceData.Value);
                    break;
                case "Status Desc":
                    notes = notes.OrderByDescending(x => x.ReferenceData.Value);
                    break;
                case "DateAdded":
                    notes = notes.OrderBy(x => x.CreatedDate);
                    break;
                case "DateAdded Desc":
                    notes = notes.OrderByDescending(x => x.CreatedDate);
                    break;
                default:
                    notes = notes.OrderBy(x => x.ModifiedDate);
                    break;
            }

            var member_notes = new List<MemberNotes>();
            foreach (var item in notes)
            {
                double Earning;
                int sold = dbobj.Transection.Where(x => x.NoteID == item.ID && x.SellerID == uid && x.IsAllowed == true).Count();
                if (sold == 0)
                {
                    Earning = 0;
                }
                else
                {
                    Earning = (double)dbobj.Transection.Where(x => x.NoteID == item.ID && x.SellerID == uid && x.IsAllowed == true).Select(x => x.Price).Sum();
                }
                member_notes.Add(new MemberNotes()
                {
                    ID = item.ID,
                    UserID = item.SellerID,
                    Title = item.Title,
                    Category = item.NoteCategories.Name,
                    Status = item.ReferenceData.Value,
                    TotalDownloads = dbobj.Transection.Where(x => x.NoteID == item.ID && x.SellerID == uid && x.IsDownloaded == true).Count(),
                    Earnings = Earning,
                    CreatedDate = item.CreatedDate,
                    PublishedDate = item.ModifiedDate
                });

            }

            Member.MemberProfile = member_profile;
            Member.MemberNotes = member_notes.ToPagedList(page ?? 1, 5);

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(Member);
        }


        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddCategory")]
        public ActionResult AddCategory()
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddCategory")]
        public ActionResult AddCategory(AddCategory model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                NoteCategories obj = new NoteCategories();
                obj.Name = model.CategoryName;
                obj.Description = model.Description;
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = admin.ID;
                obj.IsActive = true;

                dbobj.NoteCategories.Add(obj);
                dbobj.SaveChanges();

                return RedirectToAction("ManageCategory");
            }
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }


    }
}