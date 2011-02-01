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
// Contributor(s): Orchard_______. 
//------------------------------------------------------------------------------

using System;
using System.Web;

namespace Nop.Web.Framework.Localization
{
    public class LocalizedString : MarshalByRefObject, IHtmlString {
        private readonly string _localized;
        private readonly string _scope;
        private readonly string _textHint;
        private readonly object[] _args;

        public LocalizedString(string localized) {
            _localized = localized;
        }

        public LocalizedString(string localized, string scope, string textHint, object[] args) {
            _localized = localized;
            _scope = scope;
            _textHint = textHint;
            _args = args;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue) {
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            return new LocalizedString(text);
        }

        public string Scope {
            get { return _scope; }
        }

        public string TextHint {
            get { return _textHint; }
        }

        public object[] Args {
            get { return _args; }
        }

        public string Text {
            get { return _localized; }
        }

        public override string ToString() {
            return _localized;
        }

        string IHtmlString.ToHtmlString() {
            return _localized;
        }

        public override int GetHashCode() {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }

    }
}
