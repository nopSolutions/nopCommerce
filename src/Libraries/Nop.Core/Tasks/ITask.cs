using System.Xml;

namespace Nop.Core.Tasks 
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        /// <param name="node">Custom configuration node</param>
        void Execute(XmlNode node);
    }
}
