using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using DataShop.DemoPlugin.Domain;

namespace DataShop.DemoPlugin.Services
{
    public class DemoService : IDemoService
    {
        private readonly IRepository<DemoItem> DemoItemRepository;

        public DemoService(IRepository<DemoItem> demoItemRepository)
        {
            this.DemoItemRepository = demoItemRepository;
        }

        public void AddItem(DemoItem demoItem)
        {
            DemoItemRepository.Insert(demoItem);
        }

        public List<DemoItem> GetItems()
        {
            return DemoItemRepository.Table.ToList();
        }
    }
}
