using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
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
    public class CategoryController : BaseNopController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IExportManager _exportManager;

        #endregion Fields

        #region Constructors

        public CategoryController(ICategoryService categoryService,
            IPermissionService permissionService, ILanguageService languageService, ILocalizedEntityService localizedEntityService, IProductService productService, IExportManager exportManager)
        {
            this._exportManager = exportManager;
            this._categoryService = categoryService;
            this._permissionService = permissionService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._productService = productService;
        }

        #endregion Constructors
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.Description,
                                                           HttpUtility.HtmlDecode(localized.Description),
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.SeName,
                                                           localized.SeName,
                                                           localized.LanguageId);
            }
        }

        [NonAction]
        public void UpdateCategoryProducts(Category category, IList<CategoryProductModel> addedCategoryProducts, IList<CategoryProductModel> removedCategoryProducts)
        {
            var currentProductCategories = category.ProductCategories;
            foreach (var added in addedCategoryProducts)
            {
                var productId = added.ProductId;
                var updated = category.ProductCategories.SingleOrDefault(x => x.ProductId == productId);
                if (updated != null)
                {
                    updated.IsFeaturedProduct = added.IsFeaturedProduct;
                    updated.DisplayOrder = added.DisplayOrder;
                    _categoryService.UpdateProductCategory(updated);
                }
                else
                {
                    var newProductCategory = added.ToEntity();
                    _categoryService.InsertProductCategory(newProductCategory);
                }
            }

            foreach (var removed in removedCategoryProducts)
            {
                int productId = removed.ProductId;
                var toDelete = currentProductCategories.SingleOrDefault(x => x.ProductId == productId);
                if (toDelete != null)
                {
                    _categoryService.DeleteProductCategory(toDelete);
                }
            }
        }
        
        [NonAction]
        private string GetCategoryBreadCrumb(Category category)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                    result = category.Name;
                else
                    result = category.Name + " >> " + result;

                category = _categoryService.GetCategoryById(category.ParentCategoryId);

            }
            return result;
        }

        #endregion
        
        #region List / tree

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(0, 10, true);
            var gridModel = new GridModel<CategoryModel>
            {
                Data = categories.Select(x =>
                {
                    var model = x.ToModel();
                    model.Breadcrumb = GetCategoryBreadCrumb(x);
                    return model;
                }),
                Total = categories.TotalCount
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var categories = _categoryService.GetAllCategories(command.Page - 1, command.PageSize);
            var gridModel = new GridModel<CategoryModel>
            {
                Data = categories.Select(x =>
                {
                    var model = x.ToModel();
                    model.Breadcrumb = GetCategoryBreadCrumb(x);
                    return model;
                }),
                Total = categories.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        //ajax
        public ActionResult AllCategories(string text, int selectedId)
        {
            var categories = _categoryService.GetAllCategories(true);
            categories.Insert(0, new Category { Name = "[None]", Id = 0 });
            var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Tree()
        {
            var rootCategories = _categoryService.GetAllCategoriesByParentCategoryId(0, true);
            return View(rootCategories);
        }

        //ajax
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TreeLoadChildren(TreeViewItem node)
        {
            var parentId = !string.IsNullOrEmpty(node.Value) ? Convert.ToInt32(node.Value) : 0;

            var children = _categoryService.GetAllCategoriesByParentCategoryId(parentId).Select(x =>
                new TreeViewItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    LoadOnDemand = _categoryService.GetAllCategoriesByParentCategoryId(x.Id).Count > 0,
                    Enabled = true,
                    ImageUrl = Url.Content("~/Administration/Content/images/ico-content.png")
                });

            return new JsonResult { Data = children };
        }

        //ajax
        public ActionResult TreeDrop(int item, int destinationitem, string position)
        {
            var categoryItem = _categoryService.GetCategoryById(item);
            var categoryDestinationItem = _categoryService.GetCategoryById(destinationitem);

            switch (position)
            {
                case "over":
                    categoryItem.ParentCategoryId = categoryDestinationItem.Id;
                    break;
                case "before":
                    categoryItem.ParentCategoryId = categoryDestinationItem.ParentCategoryId;
                    categoryItem.DisplayOrder = categoryDestinationItem.DisplayOrder - 1;
                    break;
                case "after":
                    categoryItem.ParentCategoryId = categoryDestinationItem.ParentCategoryId;
                    categoryItem.DisplayOrder = categoryDestinationItem.DisplayOrder + 1;
                    break;
            }

            _categoryService.UpdateCategory(categoryItem);

            return Json(new { success = true });
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            var model = new CategoryModel();
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CategoryModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var category = model.ToEntity();
                _categoryService.InsertCategory(category);
                UpdateLocales(category, model);

                return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _categoryService.GetCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.Name, Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) 
                throw new ArgumentException("No category found with the specified id", "id");
            var model = category.ToModel();
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _categoryService.GetCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.Name, Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = category.GetLocalized(x => x.Name, languageId, false);
                locale.Description = category.GetLocalized(x => x.Description, languageId, false);
                locale.MetaKeywords = category.GetLocalized(x => x.MetaKeywords, languageId, false);
                locale.MetaDescription = category.GetLocalized(x => x.MetaDescription, languageId, false);
                locale.MetaTitle = category.GetLocalized(x => x.MetaTitle, languageId, false);
                locale.SeName = category.GetLocalized(x => x.SeName, languageId, false);
            });

            CategoryProductsAttribute.Clear();
            return View(model);
        }

        [HttpPost, CategoryProducts, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CategoryModel model,
            IList<CategoryProductModel> addedCategoryProducts,
            IList<CategoryProductModel> removedCategoryProducts,
            bool continueEditing)
        {
            var category = _categoryService.GetCategoryById(model.Id);
            if (category == null)
                throw new ArgumentException("No category found with the specified id", "id");

            if (ModelState.IsValid)
            {
                category = model.ToEntity(category);
                _categoryService.UpdateCategory(category);

                UpdateLocales(category, model);
                UpdateCategoryProducts(category, addedCategoryProducts, removedCategoryProducts);

                CategoryProductsAttribute.Clear();

                return continueEditing ? RedirectToAction("Edit", category.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //parent categories
            model.ParentCategories = new List<DropDownItem> { new DropDownItem { Text = "[None]", Value = "0" } };
            if (model.ParentCategoryId > 0)
            {
                var parentCategory = _categoryService.GetCategoryById(model.ParentCategoryId);
                if (parentCategory != null && !parentCategory.Deleted)
                    model.ParentCategories.Add(new DropDownItem { Text = parentCategory.Name, Value = parentCategory.Id.ToString() });
                else
                    model.ParentCategoryId = 0;
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            _categoryService.DeleteCategory(category);
            return RedirectToAction("List");
        }

        #endregion

        #region Export/Import

        public ActionResult Export()
        {
            var fileName = string.Format("categories_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            var xml = _exportManager.ExportCategoriesToXml();
            return new XmlDownloadResult(xml, fileName);
        }

        #endregion

        #region Products

        public ActionResult Products(int id)
        {
            CategoryProductsAttribute.Clear();
            return PartialView(id);
        }

        //ajax
        [GridAction]
        public ActionResult ProductsSelect(int id)
        {
            var products = _categoryService.GetProductCategoriesByCategoryId(id).Select(x => x.ToModel()).ToList();
            products = CategoryProductsAttribute.MakeStateful(products);

            return new JsonResult
            {
                Data = new GridModel(products)
            };
        }

        [GridAction]
        public ActionResult ProductsRemove(int id, CategoryProductModel categoryProduct)
        {
            categoryProduct.CategoryId = id;
            var products = _categoryService.GetProductCategoriesByCategoryId(categoryProduct.CategoryId).Select(x => x.ToModel()).ToList();
            CategoryProductsAttribute.Remove(categoryProduct);
            products = CategoryProductsAttribute.MakeStateful(products);

            return new JsonResult
            {
                Data = new GridModel(products)
            };
        }

        [GridAction]
        public ActionResult ProductsAdd(int id, CategoryProductModel categoryProduct)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors.
                return new JsonResult { Data = "error" };
            }

            categoryProduct.CategoryId = id;
            if (string.IsNullOrEmpty(categoryProduct.ProductName))
            {
                //Lets add the product name to use in the grid.
                categoryProduct.ProductName = _productService.GetProductById(categoryProduct.ProductId).Name;
            }

            CategoryProductsAttribute.Add(categoryProduct);
            var products = _categoryService.GetProductCategoriesByCategoryId(categoryProduct.CategoryId).Select(x => x.ToModel()).ToList();
            products = CategoryProductsAttribute.MakeStateful(products);

            return new JsonResult
            {
                Data = new GridModel(products)
            };
        }

        [GridAction]
        public ActionResult ProductsEdit(int id, CategoryProductModel categoryProduct)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors.
                return new JsonResult { Data = "error" };
            }

            categoryProduct.CategoryId = id;
            var products = _categoryService.GetProductCategoriesByCategoryId(categoryProduct.CategoryId).Select(x => x.ToModel()).ToList();
            CategoryProductsAttribute.Add(categoryProduct);
            products = CategoryProductsAttribute.MakeStateful(products);

            return new JsonResult
            {
                Data = new GridModel(products)
            };
        }

        #endregion
    }
}
