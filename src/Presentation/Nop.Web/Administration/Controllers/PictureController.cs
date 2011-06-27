using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    //[AdminAuthorize] Do not use [AdminAuthorzie] attribute because of flash cookie bug used in uploadify
    public class PictureController : BaseNopController
    {
        private readonly IPictureService _pictureService;
        private readonly IAuthenticationService _authenticationService;

        public PictureController(IPictureService pictureService,
            IAuthenticationService authenticationService)
        {
            this._pictureService = pictureService;
            this._authenticationService = authenticationService;
        }

        public ActionResult InsertPicture(string authToken, HttpPostedFileBase httpPostedFile)
        {
            //Workaround for flash cookie bug
            //http://stackoverflow.com/questions/1729179/uploadify-session-and-authentication-with-asp-net-mvc
            //http://geekswithblogs.net/apopovsky/archive/2009/05/06/working-around-flash-cookie-bug-in-asp.net-mvc.aspx

            var ticket = FormsAuthentication.Decrypt(authToken);
            if (ticket == null)
                throw new Exception("No token provided");

            var identity = new FormsIdentity(ticket);
            if (!identity.IsAuthenticated)
                throw new Exception("User is not authenticated");
            
            var customer = ((FormsAuthenticationService)_authenticationService).GetAuthenticatedCustomerFromTicket(ticket);
            if (!customer.IsAdmin())
                throw new Exception("User is not admin");

            byte[] pictureBinary = httpPostedFile.GetPictureBits();
            var picture = _pictureService.InsertPicture(pictureBinary, httpPostedFile.ContentType, true);
            return Json(new { pictureId = picture.Id, imageUrl = _pictureService.GetPictureUrl(picture, 100) });
        }

        public ActionResult AsyncUpload(string authToken)
        {
            return InsertPicture(authToken, Request.Files[0]);
        }
    }
}
