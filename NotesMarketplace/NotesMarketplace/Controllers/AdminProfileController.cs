using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using System.IO;


namespace NotesMarketplace.Controllers
{
    public class AdminProfileController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();
        
        [HttpGet]
        //[Authorize(Roles ="Super Admin,Admin")]
        [Route("AdminProfile")]
        public ActionResult AdminProfile()
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var apobj = dbobj.Admin.Where(x => x.UserID == obj.ID).FirstOrDefault();

            Models.AdminProfile myprofile = new Models.AdminProfile();
            myprofile.FirstName = obj.FirstName;
            myprofile.LastName = obj.LastName;
            myprofile.Email = obj.EmailID;
            if (apobj != null)
            {
                myprofile.SecondaryEmail = apobj.SecondaryEmail;
                myprofile.CountryCode = apobj.CountryCode;
                myprofile.PhoneNumber = apobj.PhoneNumber;
            }
            ViewBag.CountryCodelist = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
            ViewBag.ProfilePicture = dbobj.Admin.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(myprofile);
        }

        [HttpPost]
        //[Authorize(Roles ="Super Admin,Admin")]
        [Route("AdminProfile")]
        public ActionResult AdminProfile(NotesMarketplace.Models.AdminProfile model)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var apobj = dbobj.Admin.Where(x => x.UserID == obj.ID).FirstOrDefault();

            obj.FirstName = model.FirstName;
            obj.LastName = model.LastName;
            obj.EmailID = model.Email;
            apobj.SecondaryEmail = model.SecondaryEmail;
            apobj.CountryCode = model.CountryCode;
            apobj.PhoneNumber = model.PhoneNumber;

            string path = Path.Combine(Server.MapPath("~/Members"), obj.ID.ToString());

            //Checking for directory

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Saving Profile Picture
            if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
            {
                var ProfilePicture = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + Path.GetExtension(model.ProfilePicture.FileName);
                var ImageSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/") + "DP_" + ProfilePicture);
                model.ProfilePicture.SaveAs(ImageSavePath);
                apobj.ProfilePicture = Path.Combine(("Members/" + obj.ID + "/"), "DP_" + ProfilePicture);
            }
            else
            {
                apobj.ProfilePicture = dbobj.SystemConfigurations.Where(x => x.Key == "DefaultProfilePicture").Select(x => x.Value).ToString();
            }

            dbobj.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            dbobj.Entry(apobj).State = System.Data.Entity.EntityState.Modified;
            dbobj.SaveChanges();

            return RedirectToAction("AdminDashboard", "Admin");
        }
    }
}