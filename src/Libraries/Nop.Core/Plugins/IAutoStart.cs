namespace Nop.Core.Plugins
{
    /// <summary>
    /// Informs the IServiceContainer that a component should be started 
    /// through the start method and stopped through the stop method.
    /// </summary>
    public interface IAutoStart
    {
        /// <summary>The method invoked on startup of this component.</summary>
        void Start();

        /// <summary>The method invoked on shutdown of this component.</summary>
        void Stop();
    }
}
