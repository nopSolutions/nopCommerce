using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class UserController : BaseNopController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly UserSettings _userSettings;
        
        #endregion Fields

        #region Constructors

        public UserController(IUserService userService, IDateTimeHelper dateTimeHelper,
            UserSettings userSettings)
        {
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._userSettings = userSettings;
        }

        #endregion Constructors
        
        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //TODO add filtering by email, username, registration date, etc
            var users = _userService.GetUsers(null,null, 0, 10);
            //TODO pass _userSettings.UsernamesEnabled to the view and display usernames (if enabled)
            var gridModel = new GridModel<UserModel>
            {
                Data = users.Select(x =>
                {
                    var model = new UserModel()
                    {
                        Id= x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        IsApproved = x.IsApproved,
                        IsLockedOut = x.IsLockedOut,
                        CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString()
                    };
                    return model;
                }),
                Total = users.TotalCount
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var users = _userService.GetUsers(null, null, command.Page - 1, command.PageSize);
            //TODO pass _userSettings.UsernamesEnabled to the view and display usernames (if enabled)
            var gridModel = new GridModel<UserModel>
            {
                Data = users.Select(x =>
                {
                    var model = new UserModel()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Username = x.Username,
                        IsApproved = x.IsApproved,
                        IsLockedOut = x.IsLockedOut,
                        CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString()
                    };
                    return model;
                }),
                Total = users.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            var model = new UserModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Create(UserModel model, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //UNDONE set password format
            var registrationRequest = new UserRegistrationRequest(model.Email,
                model.Username, model.Password, PasswordFormat.Clear, model.SecurityQuestion,
                model.SecurityAnswer, model.IsApproved);

            var registrationResult = _userService.RegisterUser(registrationRequest);

            if (!registrationResult.Success)
            {
                //TODO display errors
                throw new NopException(registrationResult.Errors.First());
            }
            var user = registrationResult.User;
            user.Comments = model.Comments;
            user.IsLockedOut = model.IsLockedOut;
            _userService.UpdateUser(user);
            
            return continueEditing ? RedirectToAction("Edit", new { id = user.Id }) : RedirectToAction("List");
        }

        public ActionResult Edit(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "id");

            var model = new UserModel()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                SecurityQuestion = user.SecurityQuestion,
                SecurityAnswer = user.SecurityAnswer,
                Comments = user.Comments,
                IsApproved = user.IsApproved,
                IsLockedOut = user.IsLockedOut,
                CreatedOnStr = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).ToString(),
            };
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(UserModel model, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "id");

            //email
            if (String.IsNullOrEmpty(model.Email))
                throw new ArgumentException("Email is not provided");
            //username 
            if (_userSettings.UsernamesEnabled &&
                this._userSettings.AllowUsersToChangeUsernames)
            {
                if (String.IsNullOrEmpty(model.Username))
                    throw new ArgumentException("Username is not provided");
                if (!user.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    _userService.SetUsername(user, model.Username);
            }
            if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                _userService.SetEmail(user, model.Email);
            user.SecurityQuestion = model.SecurityQuestion;
            user.SecurityAnswer = model.SecurityAnswer;
            user.Comments = model.Comments;
            user.IsApproved = model.IsApproved;
            user.IsLockedOut = model.IsLockedOut;

            _userService.UpdateUser(user);

            return continueEditing ? RedirectToAction("Edit", user.Id) : RedirectToAction("List");
        }


        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(UserModel model)
        {
            var user = _userService.GetUserById(model.Id);
            if (user == null)
                throw new ArgumentException("No user found with the specified id");

            var changePassRequest = new ChangePasswordRequest(model.Email, false, model.Password);
            var changePassResult = _userService.ChangePassword(changePassRequest);
            if (!changePassResult.Success)
            {
                //TODO display errors
                throw new NopException(changePassResult.Errors.First());
            }
            
            //UNDONE show message that password is changed
            return Edit(model.Id);
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "id");

            _userService.DeleteUser(user);
            return RedirectToAction("List");
        }
        
        #endregion
    }
}
