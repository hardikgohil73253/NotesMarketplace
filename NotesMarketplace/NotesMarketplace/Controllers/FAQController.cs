using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NotesMarketplace.Controllers
{
    public class FAQController : Controller
    {
        NotesMarketplaceEntities dbobj = new NotesMarketplaceEntities();
        
        [Route("FAQ")]
        public ActionResult FAQ()
        {
            var emailid = User.Identity.Name.ToString();
            Users obj = dbobj.Users.Where(x => x.EmailID == emailid).FirstOrDefault();
            if (obj != null)
            {
                ViewBag.ProfilePicture = dbobj.UserProfile.Where(x => x.UserID == obj.ID).Select(x => x.ProfilePicture).FirstOrDefault();
            }
            return View();
        }
    }
}