using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services;
using Nop.Web.Framework;
using Nop.Web.MVC.Areas.Admin.Models;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    public abstract class BaseNopController<TEntity, TModel> : Controller
        where TEntity : BaseEntity
        where TModel:BaseNopEntityModel<TEntity>
    {
		#region Fields (1) 

        private readonly IEntityService<TEntity> _entityService;

		#endregion Fields 

		#region Constructors (1) 

        protected BaseNopController(IEntityService<TEntity> entityService)
        {
            _entityService = entityService;
        }

		#endregion Constructors 



        #region Abstract/Virtuals

        public abstract TModel CreateModel();

        public abstract TModel CreateModel(TEntity entity);

        public virtual void EntitySavedOrUpdated(TEntity entity, TModel model)
        {
            
        }

        public abstract void UpdateEntity(TEntity entity, TModel model);

        #endregion

        #region Create

        public ActionResult Create()
        {
            return View(CreateModel());
        }

        [HttpPost]
        public virtual ActionResult Create(TModel model)
        {
            var entity = Activator.CreateInstance<TEntity>();
            UpdateEntity(entity, model);
            _entityService.Insert(entity);
            EntitySavedOrUpdated(entity, model);
            return RedirectToAction("Edit", new { id = entity.Id });
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var entity = _entityService.GetById(id);
            if (entity == null) throw new ArgumentException("No entity found with the specified id", "id");
            var model = CreateModel(entity);
            return View(model);
        }

        [HttpPost, AcceptParameter(Name = "save", Value= "save")]
        public ActionResult Edit(TModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }
            var entity = _entityService.GetById(model.Id);
            UpdateEntity(entity, model);
            _entityService.Update(entity);
            EntitySavedOrUpdated(entity, model);
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit"), AcceptParameter(Name = "save", Value = "save-continue")]
        public ActionResult EditContinue(TModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }
            var entity = _entityService.GetById(model.Id);
            UpdateEntity(entity, model);
            _entityService.Update(entity);
            EntitySavedOrUpdated(entity, model);
            return View("Edit", model);
        }

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var entity = _entityService.GetById(id);
            if (entity == null)
            {
                throw new NopException("Entity '" + typeof(TEntity).Name + "' with an Id of '" + id + "' doesn't exist.");
            }
            var modal = CreateModel(entity);
            return Delete(modal);
        }

        public ActionResult Delete(TModel model)
        {
            return PartialView(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _entityService.GetById(id);
            _entityService.Delete(entity);
            return RedirectToAction("List");
        }

        #endregion
    }
}
