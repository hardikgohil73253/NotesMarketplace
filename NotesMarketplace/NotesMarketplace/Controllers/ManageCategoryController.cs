using NotesMarketplace.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketplace.Controllers
{
    public class ManageCategoryController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("ManageCategory")]
        public ActionResult ManageCategory(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortCategory = sortby == "Category" ? "Category Desc" : "Category";
            ViewBag.SortDescription = sortby == "Description" ? "Description Desc" : "Description";
            ViewBag.SortAddedBy = sortby == "AddedBy" ? "AddedBy Desc" : "AddedBy";

            System.Linq.IQueryable<NotesMarketplace.NoteCategories> filtered;

            if (String.IsNullOrEmpty(search))   //  All Category
            {
                //  All Category
                filtered = dbobj.NoteCategories.Where(x => x.IsActive == true || x.IsActive == false).ToList().AsQueryable();
            }
            else
            {
                filtered = dbobj.NoteCategories.Where(x => (x.Name.Contains(search) || x.Description.Contains(search) ||
                (x.CreatedDate.Value.Day + "-" + x.CreatedDate.Value.Month + "-" + x.CreatedDate.Value.Year).Contains(search)) ).ToList().AsQueryable();
            }

            switch (sortby)
            {
                case "Date Desc":
                    filtered = filtered.OrderByDescending(x => x.CreatedDate);
                    break;
                case "Category":
                    filtered = filtered.OrderBy(x => x.Name);
                    break;
                case "Category Desc":
                    filtered = filtered.OrderByDescending(x => x.Name);
                    break;
                case "Description":
                    filtered = filtered.OrderBy(x => x.Description);
                    break;
                case "Description Desc":
                    filtered = filtered.OrderByDescending(x => x.Description);
                    break;
                
                default:
                    filtered = filtered.OrderBy(x => x.CreatedDate);
                    break;
            }

            //Converting filtered entry into Manage Category Model
            var mcobj = new List<ManageCategory>();
            foreach (var item in filtered)
            {
                mcobj.Add(new ManageCategory()
                {
                    CategoryID = item.ID,
                    CategoryName = item.Name,
                    Description = item.Description,
                    CreatedDate = item.CreatedDate,
                    
                    IsActive = item.IsActive
                });

            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(mcobj.ToPagedList(page ?? 1, 5));
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

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditCategory")]
        public ActionResult EditCategory(int CategoryID)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            NoteCategories obj = dbobj.NoteCategories.Where(x => x.ID == CategoryID).FirstOrDefault();

            AddCategory model = new AddCategory();
            model.CategoryID = obj.ID;
            model.CategoryName = obj.Name;
            model.Description = obj.Description;

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditCategory")]
        public ActionResult EditCategory(AddCategory model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                NoteCategories obj = dbobj.NoteCategories.Where(x => x.ID == model.CategoryID).FirstOrDefault();

                obj.Name = model.CategoryName;
                obj.Description = model.Description;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = admin.ID;

                dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                return RedirectToAction("ManageCategory");
            }
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [Route("UpdateCategoryStatus")]
        public ActionResult UpdateCategoryStatus(int CategoryID, int status)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            NoteCategories obj = dbobj.NoteCategories.Where(x => x.ID == CategoryID).FirstOrDefault();

            if (status == 0)    // Deactivate Category
            {
                obj.IsActive = false;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = admin.ID;

            }
            else    // Activate Category
            {
                obj.IsActive = true;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = admin.ID;

            }

            dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            dbobj.SaveChanges();

            return RedirectToAction("ManageCategory");
        }



    }
}