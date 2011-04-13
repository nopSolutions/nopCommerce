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
using Nop.Web.Framework;
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
        private IExportManager _exportManager;

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
        
        #region Saving/Updating/Inserting

        public void UpdateLocales(Category category, CategoryModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(category,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.Description,
                                                           HttpUtility.HtmlDecode(localized.Description),
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.Language.Id);

                _localizedEntityService.SaveLocalizedValue(category,
                                                           x => x.SeName,
                                                           localized.SeName,
                                                           localized.Language.Id);
            }
        }

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

        #endregion

        #region Common Ajax

        public ActionResult AllCategories(string text, int selectedId)
        {
            var categories = _categoryService.GetAllCategories(true);
            categories.Insert(0, new Category { Name = "[None]", Id = 0 });
            var selectList = new SelectList(categories, "Id", "Name", selectedId);
            return new JsonResult { Data = selectList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Utility

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

        #region Products

        public ActionResult Products(int id)
        {
            CategoryProductsAttribute.Clear();
            return PartialView(id);
        }

        #region Ajax

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

        #endregion

        #region Create

        public ActionResult Create()
        {
            return View(new CategoryModel());
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(CategoryModel model, bool continueEditing)
        {
            var category = model.ToEntity();
            _categoryService.InsertCategory(category);
            UpdateLocales(category, model);

            return continueEditing ? RedirectToAction("Edit", new { id = category.Id }) : RedirectToAction("List");
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) throw new ArgumentException("No category found with the specified id", "id");
            var model = category.ToModel();
            foreach (var language in _languageService.GetAllLanguages(true))
            {
                var localizedModel = new CategoryLocalizedModel
                                         {
                                             Language = language,
                                             Description = category.GetLocalized(x => x.Description, language.Id, false),
                                             Name = category.GetLocalized(x => x.Name, language.Id, false),
                                             MetaKeywords =
                                                 category.GetLocalized(x => x.MetaKeywords, language.Id, false),
                                             MetaDescription =
                                                 category.GetLocalized(x => x.MetaDescription, language.Id, false),
                                             MetaTitle = category.GetLocalized(x => x.MetaTitle, language.Id, false),
                                             SeName = category.GetLocalized(x => x.SeName, language.Id, false)
                                         };
                model.Locales.Add(localizedModel);
            }
            CategoryProductsAttribute.Clear();
            return View(model);
        }

        [HttpPost, CategoryProducts, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(CategoryModel categoryModel,
            IList<CategoryProductModel> addedCategoryProducts,
            IList<CategoryProductModel> removedCategoryProducts,
            bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", categoryModel);
            }

            var category = _categoryService.GetCategoryById(categoryModel.Id);
            category = categoryModel.ToEntity(category);
            _categoryService.UpdateCategory(category);

            UpdateLocales(category, categoryModel);
            UpdateCategoryProducts(category, addedCategoryProducts, removedCategoryProducts);

            CategoryProductsAttribute.Clear();

            return continueEditing ? RedirectToAction("Edit", category.Id) : RedirectToAction("List");
        }

        #endregion

        #region Delete

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

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            {
                //TODO redirect to access denied page
            }

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

        #endregion

        #region Tree

        public ActionResult Tree()
        {
            var rootCategories = _categoryService.GetAllCategoriesByParentCategoryId(0, true);
            return View(rootCategories);
        }

        #region Ajax

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

        #endregion
    }
}
