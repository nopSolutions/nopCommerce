using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CustomerController : BaseNopController
    {
        #region Fields

        private readonly ICustomerService _customerService;

        #endregion Fields

        #region Constructors

        public CustomerController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        #endregion Constructors

        #region Utities

        [NonAction]
        private string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //TODO add filtering by 
            var customers = _customerService.GetAllCustomers(null,null, 0, 10);
            var gridModel = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model = x.ToModel();
                    model.FullName = string.Format("{0} {1}", x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), x.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                    model.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    return model;
                }),
                Total = customers.TotalCount
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var customers = _customerService.GetAllCustomers(null, null, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model = x.ToModel();
                    model.FullName = string.Format("{0} {1}", x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), x.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                    model.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    return model;
                }),
                Total = customers.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //public ActionResult Create()
        //{
        //    return View(new CategoryModel());
        //}

        //[HttpPost]
        //public ActionResult Create(CategoryModel model)
        //{
        //    var category = model.ToEntity();
        //    _categoryService.InsertCategory(category);
        //    UpdateLocales(category, model);
        //    return RedirectToAction("Edit", new { id = category.Id });
        //}
        
        //public ActionResult Edit(int id)
        //{
        //    var category = _categoryService.GetCategoryById(id);
        //    if (category == null) throw new ArgumentException("No category found with the specified id", "id");
        //    var model = category.ToModel();
        //    foreach (var language in _languageService.GetAllLanguages(true))
        //    {
        //        var localizedModel = new CategoryLocalizedModel
        //                                 {
        //                                     Language = language,
        //                                     Description = category.GetLocalized(x => x.Description, language.Id, false),
        //                                     Name = category.GetLocalized(x => x.Name, language.Id, false),
        //                                     MetaKeywords =
        //                                         category.GetLocalized(x => x.MetaKeywords, language.Id, false),
        //                                     MetaDescription =
        //                                         category.GetLocalized(x => x.MetaDescription, language.Id, false),
        //                                     MetaTitle = category.GetLocalized(x => x.MetaTitle, language.Id, false),
        //                                     SeName = category.GetLocalized(x => x.SeName, language.Id, false)
        //                                 };
        //        model.Locales.Add(localizedModel);
        //    }
        //    CategoryProductsAttribute.Clear();
        //    return View(model);
        //}

        //[HttpPost, CategoryProducts, FormValueExists("save", "save-continue", "continueEditing")]
        //public ActionResult Edit(CategoryModel categoryModel,
        //    IList<CategoryProductModel> addedCategoryProducts,
        //    IList<CategoryProductModel> removedCategoryProducts,
        //    bool continueEditing)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Edit", categoryModel);
        //    }

        //    var category = _categoryService.GetCategoryById(categoryModel.Id);
        //    category = categoryModel.ToEntity(category);
        //    _categoryService.UpdateCategory(category);

        //    UpdateLocales(category, categoryModel);
        //    UpdateCategoryProducts(category, addedCategoryProducts, removedCategoryProducts);

        //    CategoryProductsAttribute.Clear();

        //    return continueEditing ? RedirectToAction("Edit", category.Id) : RedirectToAction("List");
        //}
        
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var category = _categoryService.GetCategoryById(id);
        //    _categoryService.DeleteCategory(category);
        //    return RedirectToAction("List");
        //}
        
        #endregion
    }
}
