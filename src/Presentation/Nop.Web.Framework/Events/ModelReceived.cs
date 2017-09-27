using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Framework.Events
{
    public class ModelReceived<T> where T : BaseNopModel
    {
        public ModelReceived(T model, ModelStateDictionary modelState)
        {
            this.Model = model;
            this.ModelState = modelState;
        }

        public T Model { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }
    }
}
