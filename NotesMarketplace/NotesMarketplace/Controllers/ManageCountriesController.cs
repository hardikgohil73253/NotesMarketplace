using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using PagedList;


namespace NotesMarketplace.Controllers
{
    public class ManageCountriesController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("ManageCountries")]
        public ActionResult ManageCountries(string search, int? page, string sortby)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.SortDate = string.IsNullOrEmpty(sortby) ? "Date Desc" : "";
            ViewBag.SortCountry = sortby == "Country" ? "Country Desc" : "Country";
            ViewBag.SortAddedBy = sortby == "AddedBy" ? "AddedBy Desc" : "AddedBy";

            System.Linq.IQueryable<Countries> filtered;

            if (String.IsNullOrEmpty(search))   //  All Type
            {
                //  All Type
                filtered = dbobj.Countries.Where(x => x.IsActive == true || x.IsActive == false).ToList().AsQueryable();
            }
            else
            {
                filtered = dbobj.Countries.Where(x => (x.Name.Contains(search) || x.Name.Contains(search) ||
                (x.CreatedDate.Value.Day + "-" + x.CreatedDate.Value.Month + "-" + x.CreatedDate.Value.Year).Contains(search) 
                )).ToList().AsQueryable();
            }

            switch (sortby)
            {
                case "Date Desc":
                    filtered = filtered.OrderByDescending(x => x.CreatedDate);
                    break;
                case "Country":
                    filtered = filtered.OrderBy(x => x.Name);
                    break;
                case "Country Desc":
                    filtered = filtered.OrderByDescending(x => x.Name);
                    break;
                
                default:
                    filtered = filtered.OrderBy(x => x.CreatedDate);
                    break;
            }

            //Converting filtered entry into Manage Country Model
            var mcobj = new List<ManageCountry>();
            foreach (var item in filtered)
            {
                mcobj.Add(new ManageCountry()
                {
                    CountryID = item.ID,
                    CountryName = item.Name,
                    CountryCode = item.CountryCode,
                    CreatedDate = item.CreatedDate,
                    AddedBy = " " ,
                    IsActive = item.IsActive
                });

            }

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(mcobj.ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddCountry")]
        public ActionResult AddCountry()
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("AddCountry")]
        public ActionResult AddCountry(AddCountry model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Countries obj = new Countries();
                obj.Name = model.CountryName;
                obj.CountryCode = model.CountryCode;
                obj.CreatedDate = DateTime.Now;
                obj.CreatedBy = admin.ID;
                obj.IsActive = true;

                dbobj.Countries.Add(obj);
                dbobj.SaveChanges();

                return RedirectToAction("ManageCountries");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditCountry")]
        public ActionResult EditCountry(int CountryID)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Countries obj = dbobj.Countries.Where(x => x.ID == CountryID).FirstOrDefault();

            AddCountry model = new AddCountry();
            model.CountryID = obj.ID;
            model.CountryName = obj.Name;
            model.CountryCode = obj.CountryCode;

            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        [Route("EditCountry")]
        public ActionResult EditCountry(AddCountry model)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Countries obj = dbobj.Countries.Where(x => x.ID == model.CountryID).FirstOrDefault();

                obj.Name = model.CountryName;
                obj.CountryCode = model.CountryCode;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = admin.ID;

                dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                dbobj.SaveChanges();

                return RedirectToAction("ManageCountries");
            }
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == admin.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }

        [Authorize(Roles = "Super Admin,Admin")]
        [Route("UpdateCountryStatus")]
        public ActionResult UpdateCountryStatus(int CountryID, int status)
        {
            var emailid = User.Identity.Name.ToString();
            Users admin = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            Countries obj = dbobj.Countries.Where(x => x.ID == CountryID).FirstOrDefault();

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

            return RedirectToAction("ManageCountries");
        }
    }
}