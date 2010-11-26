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
// Contributor(s): BlogEngine.NET (http://www.dotnetblogengine.net/). 
//------------------------------------------------------------------------------


namespace NopSolutions.NopCommerce.Common.Utils.Html.CodeFormatter
{
    /// <summary>
    /// Handles all of the options for changing the rendered code.
    /// </summary>
    public partial class HighlightOptions
    {
        private string language, title, code;
        private bool displayLineNumbers;
        private bool alternateLineNumbers;

        public HighlightOptions()
        {
        }

        public HighlightOptions(string language, string title, bool linenumbers, string code, bool alternateLineNumbers)
        {
            this.language = language;
            this.title = title;
            this.alternateLineNumbers = alternateLineNumbers;
            this.code = code;
            this.displayLineNumbers = linenumbers;
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public bool DisplayLineNumbers
        {
            get { return displayLineNumbers; }
            set { displayLineNumbers = value; }
        }
        public string Language
        {
            get { return language; }
            set { language = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public bool AlternateLineNumbers
        {
            get { return alternateLineNumbers; }
            set { alternateLineNumbers = value; }
        }
    }
}

