using System.Web;

namespace Nop.Services.Messages
{
    public sealed class Token
    {
        private readonly string key;
        private readonly string value;

        public Token(string key, string value)
        {
            this.key = key;
            this.value = HttpUtility.HtmlEncode(value);
        }

        public string Key { get { return key; } }
        public string Value { get { return value; } }
    }
}
