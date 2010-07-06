//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): Jordan Van Gogh_______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Text;
using System.Threading;


namespace NopSolutions.NopCommerce.BusinessLogic.Data
{
    /// <summary>
    /// Defines a scope wherein only one ObjectContext instance is created, and shared by all
    /// of those who use it. Instances of this class are supposed to be used in a using() 
    /// statement.
    /// </summary>
    /// <typeparam name="T">An ObjectContext type.</typeparam>
    public class ObjectContextScope<T> : IDisposable where T : ObjectContext, new()
    {
        [ThreadStatic]
        private static ObjectContextScope<T> _currentScope;
        private T _objectContext;
        private bool _isDisposed, _saveAllChangesAtEndOfScope;

        /// <summary>
        /// Gets or sets a boolean value that indicates whether to automatically save 
        /// all object changes at end of the scope.
        /// </summary>
        public bool SaveAllChangesAtEndOfScope
        {
            get { return _saveAllChangesAtEndOfScope; }
            set { _saveAllChangesAtEndOfScope = value; }
        }

        /// <summary>
        /// Returns a reference to the ObjectContext that is created for the current scope. If
        /// no scope currently exists, null is returned.
        /// </summary>
        protected internal static T CurrentObjectContext
        {
            get { return _currentScope != null ? _currentScope._objectContext : null; }
        }

        /// <summary>
        /// Default constructor. Object changes are not automatically saved at the end of the scope.
        /// </summary>
        public ObjectContextScope()
            : this(false)
        { }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="saveAllChangesAtEndOfScope">
        /// A boolean value that indicates whether to automatically save 
        /// all object changes at end of the scope.
        /// </param>
        public ObjectContextScope(bool saveAllChangesAtEndOfScope)
        {
            if (_currentScope != null && !_currentScope._isDisposed)
                throw new InvalidOperationException("ObjectContextScope instances cannot be nested.");

            _saveAllChangesAtEndOfScope = saveAllChangesAtEndOfScope;
            _objectContext = new T();
            _isDisposed = false;
            Thread.BeginThreadAffinity();
            System.Diagnostics.Debug.WriteLine("Begin of ObjectContextScope");
            _currentScope = this;
        }

        /// <summary>
        /// Disposes the ObjectContext.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _currentScope = null;
                Thread.EndThreadAffinity();
                if (_saveAllChangesAtEndOfScope)
                    _objectContext.SaveChanges();
                _objectContext.Dispose();
                _isDisposed = true;

                System.Diagnostics.Debug.WriteLine("End of ObjectContextScope");
            }
        }
    }
}
