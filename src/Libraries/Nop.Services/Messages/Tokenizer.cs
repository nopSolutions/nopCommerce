using System;
using System.Collections.Generic;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public partial class Tokenizer:ITokenizer
    {
        private readonly StringComparison stringComparizon;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings">Message templates settings</param>
        public Tokenizer(MessageTemplatesSettings settings)
        {
            stringComparizon = settings.CaseInvariantReplacement ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        /// <summary>
        /// Replace all of the token key occurences inside the specified template text with corresponded token values
        /// </summary>
        /// <param name="template">The template with token keys inside</param>
        /// <param name="tokens">The sequence of tokens to use</param>
        /// <returns>Text with all token keys replaces by token value</returns>
        public string Replace(string template, IEnumerable<Token> tokens)
        {
            if (string.IsNullOrWhiteSpace(template))
                throw new ArgumentNullException("template");

            if (tokens == null)
                throw new ArgumentNullException("tokens");

            foreach (var token in tokens)
                template = Replace(template, String.Format(@"%{0}%", token.Key),  token.Value);
            return template;

        }

        private string Replace(string original, string pattern, string replacement)
        {
            if (stringComparizon == StringComparison.Ordinal)
            {
                return original.Replace(pattern, replacement);
            }
            else
            {
                int count, position0, position1;
                count = position0 = position1 = 0;
                int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
                char[] chars = new char[original.Length + Math.Max(0, inc)];
                while ((position1 = original.IndexOf(pattern, position0, stringComparizon)) != -1)
                {
                    for (int i = position0; i < position1; ++i)
                        chars[count++] = original[i];
                    for (int i = 0; i < replacement.Length; ++i)
                        chars[count++] = replacement[i];
                    position0 = position1 + pattern.Length;
                }
                if (position0 == 0) return original;
                for (int i = position0; i < original.Length; ++i)
                    chars[count++] = original[i];
                return new string(chars, 0, count);
            }
        }

    }
}
