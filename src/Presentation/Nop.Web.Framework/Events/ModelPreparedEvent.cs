
namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents an event that occurs after the model is prepared for view
/// </summary>
/// <typeparam name="T">Type of the model</typeparam>
public partial class ModelPreparedEvent<T>
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Model</param>
    public ModelPreparedEvent(T model)
    {
        Model = model;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a model
    /// </summary>
    public T Model { get; protected set; }

    #endregion
}