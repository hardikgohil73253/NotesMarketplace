using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using PagedList;


namespace NotesMarketplace.Controllers
{
    public class AllMembersController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AllMembers")]
        public ActionResult AllMembers(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortFirstName = sortby == "FirstName" ? "FirstName Desc" : "FirstName";
            ViewBag.SortLastName = sortby == "LastName" ? "LastName Desc" : "LastName";
            ViewBag.SortEmail = sortby == "Email" ? "Email Desc" : "Email";

            System.Linq.IQueryable<NotesMarketplace.Users> filtered;     //Empty Variable for Holding Members

            if (String.IsNullOrEmpty(search))   //  All Members
            {
                //  All Members
                filtered = dbobj.Users.Where(x => x.IsActive == true || x.IsActive == false).ToList().AsQueryable();
            }
            else
            {
                /*filtered = dbobj.UserTable.Where(x => x.IsActive == true || x.IsActive == false && (x.FirstName.Contains(search) || x.LastName.Contains(search) ||
                x.Email.Contains(search) || (x.CreatedDate.Value.Day + "-" + x.CreatedDate.Value.Month + "-" + x.CreatedDate.Value.Year).Contains(search))).ToList().AsQueryable();*/
                filtered = dbobj.Users.Where(x => x.IsActive && (x.FirstName.Contains(search) || x.LastName.Contains(search) ||
                x.EmailID.Contains(search) || (x.CreatedData.Value.Day + "-" + x.CreatedData.Value.Month + "-" + x.CreatedData.Value.Year).Contains(search))).ToList().AsQueryable();
            }

            var entry = filtered.Where(x => x.RoleID == 3).ToList().AsQueryable();

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

            //Converting entry into Members Model
            /*var mobj = new List<Members>();
            foreach (var item in entry)
            {
                mobj.Add(new Members()
                {
                    UID = item.UID,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    JoiningDate = item.CreatedDate,
                    UnderReviewNotes = dbobj.NoteTable.Where(x=>x.UID==item.UID && (x.Status == 2 || x.Status == 3)).Count(),
                    PublishedNotes = dbobj.NoteTable.Where(x => x.UID == item.UID && x.Status == 4).Count(),
                    DownloadedNotes = dbobj.TransectionTable.Where(x => x.BuyerID == item.UID && x.IsDownloaded == true).Count(),
                    TotalExpenses = dbobj.TransectionTable.Where(x => x.BuyerID == item.UID && x.IsDownloaded == true).Sum(x=>x.Price),
                    TotalEarnings = dbobj.TransectionTable.Where(x => x.SellerID == item.UID && x.IsAllowed == true).Sum(x => x.Price),
                    IsActive = item.IsActive
                });

            }*/

            var mobj = new List<Members>();
            foreach (var item in entry)
            {
                mobj.Add(new Members()
                {
                    UserID = item.ID,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.EmailID,
                    JoiningDate = item.CreatedData,
                    UnderReviewNotes = dbobj.SellerNotes.Where(x => x.SellerID == item.ID && (x.Status == 2 || x.Status == 3)).Count(),
                    PublishedNotes = dbobj.SellerNotes.Where(x => x.SellerID == item.ID && x.Status == 4).Count(),
                    DownloadedNotes = dbobj.Transection.Where(x => x.BuyerID == item.ID && x.IsDownloaded == true).Count(),
                    TotalExpenses = (int?)(dbobj.Transection.Where(x => x.BuyerID == item.ID && x.IsDownloaded == true).Count() == 0 ? 0 : dbobj.Transection.Where(x => x.BuyerID == item.ID && x.IsDownloaded == true).Sum(x => x.Price)),
                    TotalEarnings = (int?)(dbobj.Transection.Where(x => x.SellerID == item.ID && x.IsAllowed == true).Count() == 0 ? 0 : dbobj.Transection.Where(x => x.SellerID == item.ID && x.IsAllowed == true).Sum(x => x.Price)),
                    IsActive = item.IsActive
                });

            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(mobj.ToPagedList(page ?? 1, 5));
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [Route("Deactivate")]
        public ActionResult Deactivate(int uid)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Users userobj = dbobj.Users.Where(x => x.ID == uid).FirstOrDefault();
            UserProfile upobj = dbobj.UserProfile.Where(x => x.UserID == uid).FirstOrDefault();

            userobj.IsActive = false;
            upobj.IsActive = false;

            var usernotes = dbobj.SellerNotes.Where(x => x.SellerID == uid);

            foreach (var item in usernotes)
            {
                item.IsActive = false;
                dbobj.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            /*var user_notes_attachments = dbobj.NoteTable.Where(x => x.UID == uid);

            foreach (var item in user_notes_attachments)
            {
                item.IsActive = false;
            }*/

            dbobj.Entry(userobj).State = System.Data.Entity.EntityState.Modified;
            dbobj.Entry(upobj).State = System.Data.Entity.EntityState.Modified;

            dbobj.SaveChanges();

            return RedirectToAction("AllMembers");
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [Route("Activate")]
        public ActionResult Activate(int uid)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Users userobj = dbobj.Users.Where(x => x.ID == uid).FirstOrDefault();
            UserProfile upobj = dbobj.UserProfile.Where(x => x.UserID == uid).FirstOrDefault();

            userobj.IsActive = true;
            upobj.IsActive = true;

            var usernotes = dbobj.SellerNotes.Where(x => x.SellerID == uid);

            foreach (var item in usernotes)
            {
                item.IsActive = true;
                dbobj.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }

            /*var user_notes_attachments = dbobj.NoteTable.Where(x => x.UID == uid);

            foreach (var item in user_notes_attachments)
            {
                item.IsActive = false;
            }*/

            dbobj.Entry(userobj).State = System.Data.Entity.EntityState.Modified;
            dbobj.Entry(upobj).State = System.Data.Entity.EntityState.Modified;

            dbobj.SaveChanges();

            return RedirectToAction("AllMembers");
        }
    }
}