using System;
using System.Web;

namespace Nop.Web.Framework.Localization
{
    /// <summary>
    /// 本地化字符串类
    /// </summary>
    public class LocalizedString : MarshalByRefObject, IHtmlString
    {
        private readonly string _localized;
        private readonly string _scope;
        private readonly string _textHint;
        private readonly object[] _args;

        /// <summary>
        /// 构造本地化字符串
        /// </summary>
        /// <param name="localized"></param>
        public LocalizedString(string localized)
        {
            _localized = localized;
        }

        public LocalizedString(string localized, string scope, string textHint, object[] args)
        {
            _localized = localized;
            _scope = scope;
            _textHint = textHint;
            _args = args;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
        {
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            return new LocalizedString(text);
        }

        public string Scope
        {
            get { return _scope; }
        }

        public string TextHint
        {
            get { return _textHint; }
        }

        public object[] Args
        {
            get { return _args; }
        }

        /// <summary>
        /// 本地化资源字符串值
        /// </summary>
        public string Text
        {
            get { return _localized; }
        }

        public override string ToString()
        {
            return _localized;
        }

        /// <summary>
        /// 返回 HTML 编码的字符串。
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            return _localized;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }

    }
}
