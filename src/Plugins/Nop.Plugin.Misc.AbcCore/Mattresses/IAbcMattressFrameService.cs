using System.Collections.Generic;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public interface IAbcMattressFrameService
    {
        IList<AbcMattressFrame> GetAbcMattressFramesBySize(string size);
    }
}
