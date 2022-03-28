using Nop.Services.Tasks;

//Because the sli export is part of the update, this interface exists to resolve the circular dependency
namespace Nop.Plugin.Misc.AbcCore
{
    public interface ISliExportTask : IScheduleTask
    {

    }
}
