using System.Xml;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;

namespace Nop.Services.Tasks
{
    /// <summary>
    /// Test task. TODO delete this file after testing
    /// </summary>
    public partial class TestTask1 : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var service1 = EngineContext.Current.Resolve<IWebHelper>();

            //TODO find a solution. We can't resolve IWorkContext because HttpContext parameter could not be resolved.
            //var service2 = EngineContext.Current.Resolve<IWorkContext>();
        }

    }
}
