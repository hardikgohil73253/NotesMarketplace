using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using NotesMarketplace.Models;

namespace NotesMarketplace.Controllers
{
    public class UserProfileController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        [HttpGet]
        [Route("UserProfile")]
        public ActionResult UserProfile()
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            var isnew = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();

            if (isnew == null)   // For new user
            {
                Models.UserProfile upobj = new Models.UserProfile();
                upobj.UserID = obj.ID;
                upobj.FirstName = obj.FirstName;
                upobj.LastName = obj.LastName;
                upobj.EmailID = obj.EmailID;

                ViewBag.CountryCode = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
                ViewBag.CountryName = new SelectList(dbobj.Countries, "ID", "Name");
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();

                return View(upobj);
            }
            else
            {
                UserProfile oldupobj = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();
                Models.UserProfile editupobj = new Models.UserProfile();

                editupobj.UserID = obj.ID;
                editupobj.FirstName = obj.FirstName;
                editupobj.LastName = obj.LastName;
                editupobj.EmailID = obj.EmailID;

                editupobj.DateOfBirth = oldupobj.DOB;
                editupobj.Gender = dbobj.ReferenceData.Where(x => x.ID == oldupobj.Gender).Select(x=>x.ID).FirstOrDefault();
                editupobj.CountryCode = oldupobj.PhoneNumberCountryCode;
                editupobj.PhoneNumber = oldupobj.PhoneNumber;
                //editupobj.ProfilePicture = oldupobj.ProfilePicture;
                editupobj.AddressLine1 = oldupobj.AddressLine1;
                editupobj.AddressLine2 = oldupobj.AddressLine2;
                editupobj.City = oldupobj.City;
                editupobj.State = oldupobj.State;
                editupobj.ZipCode = oldupobj.ZipCode;
                editupobj.CountryID = oldupobj.Country;
                editupobj.University = oldupobj.University;
                editupobj.College = oldupobj.College;

                ViewBag.CountryCode = new SelectList(dbobj.Countries, "CountryCode", "CountryCode");
                ViewBag.CountryName = new SelectList(dbobj.Countries, "ID", "Name");
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();

                return View(editupobj);
            }
        }

        [HttpPost]
        [Route("UserProfile")]
        public ActionResult UserProfile(Models.UserProfile model)
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var isnew = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();

                if (isnew == null)   // For new user
                {
                    UserProfile upobj = new UserProfile();
                    upobj.UserID = obj.ID;
                    upobj.DOB = model.DateOfBirth;
                    upobj.Gender = model.Gender;
                    upobj.PhoneNumberCountryCode = model.CountryCode;
                    upobj.PhoneNumber = model.PhoneNumber;
                    upobj.AddressLine1 = model.AddressLine1;
                    upobj.AddressLine2 = model.AddressLine2;
                    upobj.City = model.City;
                    upobj.State = model.State;
                    upobj.ZipCode = model.ZipCode;
                    upobj.Country = model.CountryID;
                    upobj.University = model.University;
                    upobj.College = model.College;
                    upobj.CreatedDate = DateTime.Now;
                    upobj.CreatedBy = obj.ID;
                    upobj.IsActive = true;

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
                        upobj.ProfilePicture = Path.Combine(("Members/" + obj.ID + "/"), "DP_" + ProfilePicture);
                        dbobj.SaveChanges();
                    }
                    else
                    {
                        /*upobj.ProfilePicture = "Default/User.jpg";*/
                        upobj.ProfilePicture = dbobj.SystemConfigurations.Where(x => x.Key== "DefaultProfilePicture").Select(x=>x.Value).ToString();
                        dbobj.SaveChanges();
                    }

                    dbobj.UserProfile.Add(upobj);
                    dbobj.SaveChanges();

                    return RedirectToAction("SearchNotes", "SearchNotes");
                }
                else
                {
                    UserProfile oldupobj = dbobj.UserProfile.Where(x => x.UserID == obj.ID).FirstOrDefault();

                    Users olduserobj = dbobj.Users.Where(x => x.ID == obj.ID).FirstOrDefault();

                    olduserobj.FirstName = model.FirstName;
                    olduserobj.LastName = model.LastName;
                    olduserobj.EmailID = model.EmailID;
                    olduserobj.ModifiedDate = DateTime.Now;
                    olduserobj.ModifiedBy = olduserobj.ID;

                    oldupobj.DOB = model.DateOfBirth;
                    oldupobj.Gender = model.Gender;
                    oldupobj.PhoneNumberCountryCode = model.CountryCode;
                    oldupobj.PhoneNumber = model.PhoneNumber;
                    oldupobj.AddressLine1 = model.AddressLine1;
                    oldupobj.AddressLine2 = model.AddressLine2;
                    oldupobj.City = model.City;
                    oldupobj.State = model.State;
                    oldupobj.ZipCode = model.ZipCode;
                    oldupobj.Country = model.CountryID;
                    oldupobj.University = model.University;
                    oldupobj.College = model.College;
                    oldupobj.ModifiedDate = DateTime.Now;
                    oldupobj.ModifiedBy = obj.ID;

                    string path = Path.Combine(Server.MapPath("~/Members"), obj.ID.ToString());

                    //Saving Profile Picture
                    if (model.ProfilePicture != null && model.ProfilePicture.ContentLength > 0)
                    {
                        var OldProfilePicture = Server.MapPath(oldupobj.ProfilePicture);
                        FileInfo file = new FileInfo(OldProfilePicture);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        var ProfilePicture = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + Path.GetExtension(model.ProfilePicture.FileName);
                        var ImageSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/") + "DP_" + ProfilePicture);
                        model.ProfilePicture.SaveAs(ImageSavePath);
                        oldupobj.ProfilePicture = Path.Combine(("Members/" + obj.ID + "/"), "DP_" + ProfilePicture);
                        dbobj.SaveChanges();
                    }

                    dbobj.Entry(olduserobj).State = System.Data.Entity.EntityState.Modified;
                    dbobj.Entry(oldupobj).State = System.Data.Entity.EntityState.Modified;
                    dbobj.SaveChanges();

                    return RedirectToAction("SearchNotes", "SearchNotes");
                }
            }
            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View(model);
        }
    }
}