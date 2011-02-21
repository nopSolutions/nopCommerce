using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Web.Framework.Localization
{
    public class LocalizedModels<TModel> : Dictionary<int, TModel> where TModel:ILocalizedModel
    {
    }
}
