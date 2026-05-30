namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the patch operation
/// </summary>
public enum PatchOpType
{
    /// <summary>
    /// Depending on the target location reference, completes one of these functions:
    /// - The target location is an array index. Inserts a new value into the array at the specified index.
    /// - The target location is an object parameter that does not already exist. Adds a new parameter to the object.
    /// - The target location is an object parameter that does exist. Replaces that parameter's value.
    /// </summary>
    ADD,

    /// <summary>
    /// Removes the value at the target location. For the operation to succeed, the target location must exist.
    /// </summary>
    REMOVE,

    /// <summary>
    /// Replaces the value at the target location with a new value. The operation object must contain a value parameter that defines the replacement value. For the operation to succeed, the target location must exist.
    /// </summary>
    REPLACE,

    /// <summary>
    /// Removes the value at a specified location and adds it to the target location. The operation object must contain a from parameter, which is a string that contains a JSON pointer value that references the location in the target document from which to move the value. For the operation to succeed, the from location must exist.
    /// </summary>
    MOVE,

    /// <summary>
    /// Copies the value at a specified location to the target location. The operation object must contain a from parameter, which is a string that contains a JSON pointer value that references the location in the target document from which to copy the value. For the operation to succeed, the from location must exist.
    /// </summary>
    COPY,

    /// <summary>
    /// Tests that a value at the target location is equal to a specified value. The operation object must contain a value parameter that defines the value to compare to the target location's value. For the operation to succeed, the target location must be equal to the value value. For test, equal indicates that the value at the target location and the value that value defines are of the same JSON type.
    /// </summary>
    TEST
}