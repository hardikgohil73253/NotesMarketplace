using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using PagedList;

namespace NotesMarketplace.Controllers
{
    public class ManageTypesController : Controller
    {

        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();


        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("ManageTypes")]
        public ActionResult ManageTypes(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortType = sortby == "Type" ? "Type Desc" : "Type";
            ViewBag.SortDescription = sortby == "Description" ? "Description Desc" : "Description";
            ViewBag.SortAddedBy = sortby == "AddedBy" ? "AddedBy Desc" : "AddedBy";

            System.Linq.IQueryable<NotesMarketplace.NoteTypes> filtered;

            if (String.IsNullOrEmpty(search))   //  All Type
            {
                //  All Type
                filtered = dbobj.NoteTypes.Where(x => x.IsActive == true || x.IsActive == false).ToList().AsQueryable();
            }
            else
            {
                filtered = dbobj.NoteTypes.Where(x => (x.Name.Contains(search) || x.Description.Contains(search) ||
                (x.CreatedDate.Value.Day + "-" + x.CreatedDate.Value.Month + "-" + x.CreatedDate.Value.Year).Contains(search)
                )).ToList().AsQueryable();
            }

            switch (sortby)
            {
                case "Date Desc":
                    filtered = filtered.OrderByDescending(x => x.CreatedDate);
                    break;
                case "Type":
                    filtered = filtered.OrderBy(x => x.Name);
                    break;
                case "Type Desc":
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

            //Converting filtered entry into Manage Type Model
            var mtobj = new List<ManageType>();
            foreach (var item in filtered)
            {
                mtobj.Add(new ManageType()
                {
                    TypeID = item.ID,
                    TypeName = item.Name,
                    Description = item.Description,
                    CreatedDate = item.CreatedDate,
                    AddedBy = "",
                    IsActive = item.IsActive
                });

            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(mtobj.ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddType")]
        public ActionResult AddType()
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddType")]
        public ActionResult AddType(AddType model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                NoteTypes obj = new NoteTypes();
                obj.Name = model.TypeName;
                obj.Description = model.Description;
                obj.CreatedDate = DateTime.Now;
                obj.CreaedBy = admin.ID;
                obj.IsActive = true;

                dbobj.NoteTypes.Add(obj);
                dbobj.SaveChanges();

                return RedirectToAction("ManageTypes");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditType")]
        public ActionResult EditType(int TypeID)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            NoteTypes obj = dbobj.NoteTypes.Where(x => x.ID == TypeID).FirstOrDefault();

            AddType model = new AddType();
            model.TypeID = obj.ID;
            model.TypeName = obj.Name;
            model.Description = obj.Description;

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditType")]
        public ActionResult EditType(AddType model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                NoteTypes obj = dbobj.NoteTypes.Where(x => x.ID == model.TypeID).FirstOrDefault();

                obj.Name = model.TypeName;
                obj.Description = model.Description;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = admin.ID;

                dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                return RedirectToAction("ManageTypes");
            }
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [Route("UpdateTypeStatus")]
        public ActionResult UpdateTypeStatus(int TypeID, int status)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            NoteTypes obj = dbobj.NoteTypes.Where(x => x.ID == TypeID).FirstOrDefault();

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

            return RedirectToAction("ManageTypes");
        }
    }
}