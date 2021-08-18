#region Copyright © 2001-2003 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */ 
#endregion

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Nop.Services.Html.CodeFormatter
{
    /// <summary>
    /// Provides a base class for formatting most programming languages.
    /// </summary>
    public abstract partial class CodeFormat : SourceFormat
    {
        /// <summary>
        /// Must be overridden to provide a list of keywords defined in 
        /// each language.
        /// </summary>
        /// <remarks>
        /// Keywords must be separated with spaces.
        /// </remarks>
        protected abstract string Keywords 
        {
            get;
        }

        /// <summary>
        /// Can be overridden to provide a list of preprocessors defined in 
        /// each language.
        /// </summary>
        /// <remarks>
        /// Preprocessors must be separated with spaces.
        /// </remarks>
        protected virtual string Preprocessors { get; } = string.Empty;

        /// <summary>
        /// Must be overridden to provide a regular expression string
        /// to match strings literals. 
        /// </summary>
        protected abstract string StringRegex
        {
            get;
        }

        /// <summary>
        /// Must be overridden to provide a regular expression string
        /// to match comments. 
        /// </summary>
        protected abstract string CommentRegex
        {
            get;
        }

        /// <summary>
        /// Determines if the language is case sensitive.
        /// </summary>
        /// <value><b>true</b> if the language is case sensitive, <b>false</b> 
        /// otherwise. The default is true.</value>
        /// <remarks>
        /// A case-insensitive language formatter must override this 
        /// property to return false.
        /// </remarks>
        public virtual bool CaseSensitive { get; } = true;

        /// <summary/>
        protected CodeFormat()
        {
            //generate the keyword and preprocessor regexes from the keyword lists
            var r = new Regex(@"\w+|-\w+|#\w+|@@\w+|#(?:\\(?:s|w)(?:\*|\+)?\w+)+|@\\w\*+");
            var regKeyword = r.Replace(Keywords, @"(?<=^|\W)$0(?=\W)");
            var regPreproc = r.Replace(Preprocessors, @"(?<=^|\s)$0(?=\s|$)");
            r = new Regex(@" +");
            regKeyword = r.Replace(regKeyword, @"|");
            regPreproc = r.Replace(regPreproc, @"|");

            if (regPreproc.Length == 0)
            {
                regPreproc = "(?!.*)_{37}(?<!.*)"; //use something quite impossible...
            }

            //build a master regex with capturing groups
            var regAll = new StringBuilder();
            regAll.Append('(');
            regAll.Append(CommentRegex);
            regAll.Append(")|(");
            regAll.Append(StringRegex);
            //if (regPreproc.Length > 0)
            //{
                regAll.Append(")|(");
                regAll.Append(regPreproc);
            //}
            regAll.Append(")|(");
            regAll.Append(regKeyword);
            regAll.Append(')');

            var caseInsensitive = CaseSensitive ? 0 : RegexOptions.IgnoreCase;
            CodeRegex = new Regex(regAll.ToString(), RegexOptions.Singleline | caseInsensitive);
        }

        /// <summary>
        /// Called to evaluate the HTML fragment corresponding to each 
        /// matching token in the code.
        /// </summary>
        /// <param name="match">The <see cref="Match"/> resulting from a 
        /// single regular expression match.</param>
        /// <returns>A string containing the HTML code fragment.</returns>
        protected override string MatchEval(Match match)
        {
            if (match.Groups[1].Success) //comment
            {
                var reader = new StringReader(match.ToString());
                string line;
                var sb = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append('\n');
                    }

                    sb.Append("<span class=\"rem\">");
                    sb.Append(line);
                    sb.Append("</span>");
                }

                return sb.ToString();
            }

            if (match.Groups[2].Success) //string literal
            {
                return "<span class=\"str\">" + match + "</span>";
            }

            if (match.Groups[3].Success) //preprocessor keyword
            {
                return "<span class=\"preproc\">" + match + "</span>";
            }

            if (match.Groups[4].Success) //keyword
            {
                return "<span class=\"kwrd\">" + match + "</span>";
            }

            System.Diagnostics.Debug.Assert(false, "None of the above!");

            return string.Empty; //none of the above
        }
    }
}
