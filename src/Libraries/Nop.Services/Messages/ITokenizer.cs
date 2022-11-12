<<<<<<< HEAD
﻿using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Tokenizer
    /// </summary>
    public partial interface ITokenizer
    {
        /// <summary>
        /// Replace all of the token key occurrences inside the specified template text with corresponded token values
        /// </summary>
        /// <param name="template">The template with token keys inside</param>
        /// <param name="tokens">The sequence of tokens to use</param>
        /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
        /// <returns>Text with all token keys replaces by token value</returns>
        string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode);
    }
}
=======
﻿using System.Collections.Generic;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Tokenizer
    /// </summary>
    public partial interface ITokenizer
    {
        /// <summary>
        /// Replace all of the token key occurrences inside the specified template text with corresponded token values
        /// </summary>
        /// <param name="template">The template with token keys inside</param>
        /// <param name="tokens">The sequence of tokens to use</param>
        /// <param name="htmlEncode">The value indicating whether tokens should be HTML encoded</param>
        /// <returns>Text with all token keys replaces by token value</returns>
        string Replace(string template, IEnumerable<Token> tokens, bool htmlEncode);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
