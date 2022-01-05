using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketplace.Models;
using System.IO;



namespace NotesMarketplace.Controllers
{
    [Authorize]
    public class AddNotesController : Controller
    {

        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();

        // edit method
        [HttpGet]
        [Route("AddNotes")]
        public ActionResult AddNotes(int? id)
        {
            if(id!=null)
            {
                SellerNotes noteobj = dbobj.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
                AddNotes editobj = new AddNotes();
                editobj.ID = noteobj.ID;
                editobj.Title = noteobj.Title;
                editobj.CategoryID = noteobj.Category; //category id
                editobj.TypeID = noteobj.NoteType;
                editobj.NumberOfPages = noteobj.NumberofPages;
                editobj.Description = noteobj.Description;
                editobj.CountryID = noteobj.Country;   // country id
                editobj.InstituteName = noteobj.UniversityName;
                editobj.IsPaid = noteobj.IsPaid;
                editobj.CourseName = noteobj.Course;
                editobj.CourseCode = noteobj.CourseCode;
                editobj.Professor = noteobj.Professor;
                editobj.Price = noteobj.SellingPrice;

                ViewBag.Category = new SelectList(dbobj.NoteCategories, "ID", "Name");
                ViewBag.Type = new SelectList(dbobj.NoteTypes, "ID", "Name");
                ViewBag.Country = new SelectList(dbobj.Countries, "ID", "Name");
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == noteobj.SellerID).Select(x => x.ProfilePicture).FirstOrDefault();
                return View(editobj);
            }
            var emailID = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailID).FirstOrDefault();

            ViewBag.Category = new SelectList(dbobj.NoteCategories, "ID", "Name");
            ViewBag.Type = new SelectList(dbobj.NoteTypes, "ID", "Name");
            ViewBag.Country = new SelectList(dbobj.Countries, "ID", "Name");
            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.RoleID).Select(x => x.ProfilePicture).FirstOrDefault();

            return View();
        }

        [HttpPost]
        [Route("AddNotes")]
        public ActionResult AddNotes(Models.AddNotes model, string submitButton)
        {
            var emailID = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailID).FirstOrDefault();
            if(ModelState.IsValid)
            {
                if(model.ID==null)
                {
                    if((model.File[0]==null)||((model.IsPaid==true)&&(model.PreviewAttachment==null)))
                    {
                        if (model.File[0] == null)
                        {
                            ModelState.AddModelError("File", "File Required");
                        }
                        if (model.PreviewAttachment == null)
                        {
                            ModelState.AddModelError("PreviewAttachment", "PreviewAttachment Required");
                        }
                        ViewBag.Category = new SelectList(dbobj.NoteCategories, "ID", "Name");
                        ViewBag.Type = new SelectList(dbobj.NoteTypes, "ID", "Name");
                        ViewBag.Country = new SelectList(dbobj.Countries, "ID", "Name");
                        ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.RoleID).Select(x => x.ProfilePicture).FirstOrDefault();

                        return View(model);
                    }
                    string path = Path.Combine(Server.MapPath("~/Members"), obj.ID.ToString());

                    //Checking for directory

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    SellerNotes noteobj = new SellerNotes();
                    noteobj.SellerID = obj.ID;
                    noteobj.Title = model.Title;
                    noteobj.Category = model.CategoryID;
                    noteobj.NoteType = model.TypeID;
                    noteobj.NumberofPages = model.NumberOfPages;
                    noteobj.Description = model.Description;
                    noteobj.Country = model.CountryID == null ? 8 : model.CountryID;  //if null then country id = 8
                    noteobj.UniversityName = model.InstituteName;
                    noteobj.Course = model.CourseName == null ? "Other" : model.CourseName;
                    noteobj.CourseCode = model.CourseCode;
                    noteobj.Professor = model.Professor;
                    noteobj.IsPaid = model.IsPaid;
                    noteobj.SellingPrice = model.Price;
                    if (submitButton == "1")
                    {
                        noteobj.Status = 1;
                    }
                    else
                    {
                        noteobj.Status = 2;
                    }
                    noteobj.ActionedBy = obj.ID;
                    noteobj.CreatedDate = DateTime.Now;
                    noteobj.IsActive = true;

                    dbobj.SellerNotes.Add(noteobj);
                    dbobj.SaveChanges();

                    var NoteID = noteobj.ID;
                    string finalpath = Path.Combine(Server.MapPath("~/Members/" + obj.ID), NoteID.ToString());

                    if (!Directory.Exists(finalpath))
                    {
                        Directory.CreateDirectory(finalpath);
                    }
                    if (model.DisplayPicture != null && model.DisplayPicture.ContentLength > 0)
                    {
                        var displayimagename = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + Path.GetExtension(model.DisplayPicture.FileName);
                        var ImageSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + noteobj.ID + "/") + "DP_" + displayimagename);
                        model.DisplayPicture.SaveAs(ImageSavePath);
                        noteobj.DisplayPicture = Path.Combine(("Members/" + obj.ID + "/" + noteobj.ID + "/"), "DP_" + displayimagename);
                        dbobj.SaveChanges();
                    }
                    else
                    {
                        noteobj.DisplayPicture = "Default/Book.jpg";
                        dbobj.SaveChanges();
                    }

                    if (model.PreviewAttachment != null && model.PreviewAttachment.ContentLength > 0)
                    {
                        var notespreviewname = "Preview_" + DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + "_" + Path.GetFileName(model.PreviewAttachment.FileName);
                        var PreviewSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + noteobj.ID + "/") + notespreviewname);
                        model.PreviewAttachment.SaveAs(PreviewSavePath);
                        noteobj.NotesPreview = Path.Combine(("Members/" + obj.ID + "/" + noteobj.ID + "/") + notespreviewname);
                        dbobj.SaveChanges();
                    }



                    SellerNotesAttachements natobj = new SellerNotesAttachements();
                    natobj.NoteID = NoteID;    //nat stands for note attachment table
                    natobj.IsActive = true;
                    natobj.CreatedBy = obj.ID;
                    natobj.CreatedDate = DateTime.Now;

                    string AttachmentPath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + noteobj.ID), "Attachment");

                    if (!Directory.Exists(AttachmentPath))
                    {
                        Directory.CreateDirectory(AttachmentPath);
                    }

                    int counter = 1;
                    var uploadfilepath = "";
                    var uploadfilename = "";

                    foreach (HttpPostedFileBase file in model.File)
                    {
                        //Checking file is available to save.  
                        if (file != null)
                        {
                            var InputFileName = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + "_" + Path.GetFileName(file.FileName);
                            var ServerSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + noteobj.ID + "/Attachment/") + "Attachment_" + counter + "_" + InputFileName);
                            counter++;
                            //Save file to server folder
                            file.SaveAs(ServerSavePath);
                            uploadfilepath += Path.Combine(("Members/" + obj.ID + "/" + noteobj.ID + "/Attachment/") + "Attachment_" + counter + "_" + InputFileName) + ";";
                            uploadfilename += Path.GetFileName(file.FileName) + ";";
                        }

                    }

                    natobj.FileName = uploadfilename;
                    natobj.FilePath = uploadfilepath;
                    dbobj.SellerNotesAttachements.Add(natobj);
                    dbobj.SaveChanges();
                }
                else //for edit note
                {
                    //saving into database
                    SellerNotes oldnote = dbobj.SellerNotes.Where(x=>x.ID == model.ID).FirstOrDefault();
                    oldnote.Title = model.Title;
                    oldnote.Category = model.CategoryID;
                    oldnote.NoteType = model.TypeID;
                    oldnote.NumberofPages = model.NumberOfPages;
                    oldnote.Description = model.Description;
                    oldnote.Country = model.CountryID == null ? 8 : model.CountryID;  //if null then country id = 8
                    oldnote.UniversityName = model.InstituteName;
                    oldnote.Course = model.CourseName == null ? "Other" : model.CourseName;
                    oldnote.CourseCode = model.CourseCode;
                    oldnote.Professor = model.Professor;
                    oldnote.IsPaid = model.IsPaid;
                    oldnote.SellingPrice = model.Price;

                    if (submitButton == "1")
                    {
                        oldnote.Status = 1;
                    }
                    else
                    {
                        oldnote.Status = 2;
                    }
                    oldnote.ActionedBy = obj.ID;
                    oldnote.ModifiedDate = DateTime.Now;
                    oldnote.IsActive = true;

                    dbobj.Entry(oldnote).State = System.Data.Entity.EntityState.Modified;
                    dbobj.SaveChanges();

                    var NoteID = oldnote.ID;
                    string finalpath = Path.Combine(Server.MapPath("~/Members/" + obj.ID), NoteID.ToString());

                    //  For New Display Picture
                    if (model.DisplayPicture != null && model.DisplayPicture.ContentLength > 0)
                    {
                        var OldDisplayPicture = Server.MapPath(oldnote.DisplayPicture);
                        FileInfo file = new FileInfo(OldDisplayPicture);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        var displayimagename = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + Path.GetExtension(model.DisplayPicture.FileName);
                        var ImageSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + oldnote.ID + "/") + "DP_" + displayimagename);
                        model.DisplayPicture.SaveAs(ImageSavePath);
                        oldnote.DisplayPicture = Path.Combine(("Members/" + obj.ID + "/" + oldnote.ID + "/"), "DP_" + displayimagename);
                        dbobj.SaveChanges();
                    }

                    //  For New PreviewAttachment
                    if (model.PreviewAttachment != null && model.PreviewAttachment.ContentLength > 0)
                    {
                        var OldPreviewAttachment = Server.MapPath(oldnote.NotesPreview);
                        FileInfo file = new FileInfo(OldPreviewAttachment);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        var notespreviewname = "Preview_" + DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + "_" + Path.GetFileName(model.PreviewAttachment.FileName);
                        var PreviewSavePath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + oldnote.ID + "/") + notespreviewname);
                        model.PreviewAttachment.SaveAs(PreviewSavePath);
                        oldnote.NotesPreview = Path.Combine(("Members/" + obj.ID + "/" + oldnote.ID + "/") + notespreviewname);
                        dbobj.SaveChanges();
                    }

                    if (model.File[0] != null)      // New file Uploaded
                    {
                        SellerNotesAttachements oldnatobj = dbobj.SellerNotesAttachements.Where(x => x.NoteID == NoteID).FirstOrDefault();
                        oldnatobj.ModifiedDate = DateTime.Now;

                        string AttachmentPath = Path.Combine(Server.MapPath("~/Members/" + obj.ID + "/" + oldnote.ID), "Attachment");

                        Directory.Delete(AttachmentPath, true);
                        Directory.CreateDirectory(AttachmentPath);

                        int counter = 1;
                        var uploadfilepath = "";
                        var uploadfilename = "";

                        foreach (HttpPostedFileBase file in model.File)
                        {
                            //Checking file is available to save.  
                            if (file != null)
                            {
                                var InputFileName = DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_') + "_" + Path.GetFileName(file.FileName);
                                var ServerSavePath = Path.Combine(Server.MapPath("Members/" + obj.ID + "/" + oldnote.ID + "/Attachment/") + "Attachment_" + counter + "_" + InputFileName);
                                counter++;
                                //Save file to server folder
                                file.SaveAs(ServerSavePath);
                                uploadfilepath += Path.Combine(("Members/" + obj.ID + "/" + oldnote.ID + "/Attachment/") + "Attachment_" + counter + "_" + InputFileName) + ";";
                                uploadfilename += Path.GetFileName(file.FileName) + ";";
                            }

                        }

                        oldnatobj.FileName = uploadfilename;
                        oldnatobj.FilePath = uploadfilepath;
                        dbobj.Entry(oldnatobj).State = System.Data.Entity.EntityState.Modified;
                        dbobj.SaveChanges();
                    }
                }
                return RedirectToAction("Dashboard","Dashboard");
            }
            ViewBag.Category = new SelectList(dbobj.NoteCategories, "ID", "Name");
            ViewBag.Type = new SelectList(dbobj.NoteTypes, "ID", "Name");
            ViewBag.Country = new SelectList(dbobj.Countries, "ID", "Name");
            ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            return View();
        }
    }
}