using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Framework.Events
{
    public class ModelPrepared<T> where T : BaseNopModel
    {
        public ModelPrepared(T model)
        {
            this.Model = model;
        }
        public T Model { get; private set; }
    }
}
