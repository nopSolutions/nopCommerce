using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Widget.Deals.Domain;

namespace Nop.Plugin.Widget.Deals.Services;

public interface IDealService
{
    Task<IEnumerable<Deal>> GetAll();
    Deal GetById(int id);
    Task Insert(Deal deal);
    void Update(Deal deal);
}