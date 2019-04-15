using System;

namespace Nop.Web.Framework.Models.DataTables
{
    /// <summary>
    /// Represent DataTables filter parameter
    /// </summary>
    public partial class FilterParameter
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the FilterParameter class by default as string type parameter
        /// </summary>
        /// <param name="name">Filter parameter name</param>
        public FilterParameter(string name)
        {
            Name = name;
            Type = typeof(string);
        }

        /// <summary>
        /// Initializes a new instance of the FilterParameter class
        /// </summary>
        /// <param name="name">Filter parameter name</param>
        /// <param name="type">Filter parameter type</param>
        public FilterParameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the FilterParameter class
        /// </summary>
        /// <param name="name">Filter parameter name</param>
        /// <param name="value">Filter parameter value</param>
        public FilterParameter(string name, object value)
        {
            Name = name;
            Type = value.GetType();
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Filter field name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Filter field type
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Filter field value
        /// </summary>
        public object Value { get; set; }

        #endregion
    }
}
