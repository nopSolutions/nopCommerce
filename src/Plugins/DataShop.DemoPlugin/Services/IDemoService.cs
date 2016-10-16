using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataShop.DemoPlugin.Domain;

namespace DataShop.DemoPlugin.Services
{
    public interface IDemoService
    {
        void AddItem(DemoItem demoItem);
        List<DemoItem> GetItems();
    }
}
