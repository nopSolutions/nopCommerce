using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Synchrony.Models
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Response
    {

        private ResponseTerm_Lookup term_LookupField;

        /// <remarks/>
        public ResponseTerm_Lookup Term_Lookup
        {
            get
            {
                return this.term_LookupField;
            }
            set
            {
                this.term_LookupField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ResponseTerm_Lookup
    {

        private string term_NoField;

        private string descriptionField;

        /// <remarks/>
        public string Term_No
        {
            get
            {
                return this.term_NoField;
            }
            set
            {
                this.term_NoField = value;
            }
        }

        /// <remarks/>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
    }


}
