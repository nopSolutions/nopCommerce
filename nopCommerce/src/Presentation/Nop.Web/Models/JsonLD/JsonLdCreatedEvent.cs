namespace Nop.Web.Models.JsonLD;

/// <summary>
/// Represents JSON-LD model created event
/// </summary>
public partial class JsonLdCreatedEvent<T> where T : JsonLdModel
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="model">Created model</param>
    public JsonLdCreatedEvent(T model)
    {
        Model = model;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Created model
    /// </summary>
    public T Model { get; }

    #endregion
}