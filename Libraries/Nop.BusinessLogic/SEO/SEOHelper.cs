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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.SEO
{
    /// <summary>
    /// Represents a SEO helper
    /// </summary>
    public partial class SEOHelper
    {
        #region Fields

        private static Dictionary<string, string> seoCharacterTable = null;
        private static object s_lock;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        static SEOHelper()
        {
            s_lock = new object();
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Takes a hexadecimal string and converts it to an Unicode character
        /// </summary>
        /// <param name="hexString">A four-digit number in hex notation (eg, 00E7).</param>
        /// <returns>A unicode character, as string.</returns>
        private static string ToUnichar(string hexString)
        {
            string returnChar;
            byte[] b = new byte[2];
            UnicodeEncoding ue = new UnicodeEncoding();

            // Take hexadecimal as text and make a Unicode char number
            b[0] = Convert.ToByte(hexString.Substring(2, 2), 16);
            b[1] = Convert.ToByte(hexString.Substring(0, 2), 16);
            // Get the character the number represents
            returnChar = ue.GetString(b);
            return returnChar;
        }

        /// <summary>
        /// Stores Unicode characters and their "normalized"
        /// values to a hash table. Character codes are referenced
        /// by hex numbers because that's the most common way to
        /// refer to them.
        /// 
        /// Upper-case comments are identifiers from the Unicode database. 
        /// Lower- and mixed-case comments are the author's
        /// </summary>
        private static void InitializeSeoCharacterTable()
        {
            lock (s_lock)
            {
                if (seoCharacterTable == null)
                {
                    seoCharacterTable = new Dictionary<string, string>();
                    seoCharacterTable.Add(ToUnichar("0041"), "A");
                    seoCharacterTable.Add(ToUnichar("0042"), "B");
                    seoCharacterTable.Add(ToUnichar("0043"), "C");
                    seoCharacterTable.Add(ToUnichar("0044"), "D");
                    seoCharacterTable.Add(ToUnichar("0045"), "E");
                    seoCharacterTable.Add(ToUnichar("0046"), "F");
                    seoCharacterTable.Add(ToUnichar("0047"), "G");
                    seoCharacterTable.Add(ToUnichar("0048"), "H");
                    seoCharacterTable.Add(ToUnichar("0049"), "I");
                    seoCharacterTable.Add(ToUnichar("004A"), "J");
                    seoCharacterTable.Add(ToUnichar("004B"), "K");
                    seoCharacterTable.Add(ToUnichar("004C"), "L");
                    seoCharacterTable.Add(ToUnichar("004D"), "M");
                    seoCharacterTable.Add(ToUnichar("004E"), "N");
                    seoCharacterTable.Add(ToUnichar("004F"), "O");
                    seoCharacterTable.Add(ToUnichar("0050"), "P");
                    seoCharacterTable.Add(ToUnichar("0051"), "Q");
                    seoCharacterTable.Add(ToUnichar("0052"), "R");
                    seoCharacterTable.Add(ToUnichar("0053"), "S");
                    seoCharacterTable.Add(ToUnichar("0054"), "T");
                    seoCharacterTable.Add(ToUnichar("0055"), "U");
                    seoCharacterTable.Add(ToUnichar("0056"), "V");
                    seoCharacterTable.Add(ToUnichar("0057"), "W");
                    seoCharacterTable.Add(ToUnichar("0058"), "X");
                    seoCharacterTable.Add(ToUnichar("0059"), "Y");
                    seoCharacterTable.Add(ToUnichar("005A"), "Z");
                    seoCharacterTable.Add(ToUnichar("0061"), "a");
                    seoCharacterTable.Add(ToUnichar("0062"), "b");
                    seoCharacterTable.Add(ToUnichar("0063"), "c");
                    seoCharacterTable.Add(ToUnichar("0064"), "d");
                    seoCharacterTable.Add(ToUnichar("0065"), "e");
                    seoCharacterTable.Add(ToUnichar("0066"), "f");
                    seoCharacterTable.Add(ToUnichar("0067"), "g");
                    seoCharacterTable.Add(ToUnichar("0068"), "h");
                    seoCharacterTable.Add(ToUnichar("0069"), "i");
                    seoCharacterTable.Add(ToUnichar("006A"), "j");
                    seoCharacterTable.Add(ToUnichar("006B"), "k");
                    seoCharacterTable.Add(ToUnichar("006C"), "l");
                    seoCharacterTable.Add(ToUnichar("006D"), "m");
                    seoCharacterTable.Add(ToUnichar("006E"), "n");
                    seoCharacterTable.Add(ToUnichar("006F"), "o");
                    seoCharacterTable.Add(ToUnichar("0070"), "p");
                    seoCharacterTable.Add(ToUnichar("0071"), "q");
                    seoCharacterTable.Add(ToUnichar("0072"), "r");
                    seoCharacterTable.Add(ToUnichar("0073"), "s");
                    seoCharacterTable.Add(ToUnichar("0074"), "t");
                    seoCharacterTable.Add(ToUnichar("0075"), "u");
                    seoCharacterTable.Add(ToUnichar("0076"), "v");
                    seoCharacterTable.Add(ToUnichar("0077"), "w");
                    seoCharacterTable.Add(ToUnichar("0078"), "x");
                    seoCharacterTable.Add(ToUnichar("0079"), "y");
                    seoCharacterTable.Add(ToUnichar("007A"), "z");
                    seoCharacterTable.Add(ToUnichar("00AA"), "a");    // FEMININE ORDINAL INDICATOR
                    seoCharacterTable.Add(ToUnichar("00BA"), "o");	// MASCULINE ORDINAL INDICATOR
                    seoCharacterTable.Add(ToUnichar("00C0"), "A");	// LATIN CAPITAL LETTER A WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00C1"), "A");	// LATIN CAPITAL LETTER A WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00C2"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00C3"), "A");	// LATIN CAPITAL LETTER A WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00C4"), "A");	// LATIN CAPITAL LETTER A WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00C5"), "A");	// LATIN CAPITAL LETTER A WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("00C6"), "AE");	// LATIN CAPITAL LETTER AE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("00C7"), "C");	// LATIN CAPITAL LETTER C WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("00C8"), "E");	// LATIN CAPITAL LETTER E WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00C9"), "E");	// LATIN CAPITAL LETTER E WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00CA"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00CB"), "E");	// LATIN CAPITAL LETTER E WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00CC"), "I");	// LATIN CAPITAL LETTER I WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00CD"), "I");	// LATIN CAPITAL LETTER I WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00CE"), "I");	// LATIN CAPITAL LETTER I WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00CF"), "I");	// LATIN CAPITAL LETTER I WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00D0"), "D");	// LATIN CAPITAL LETTER ETH -- no decomposition  	// Eth [D for Vietnamese]
                    seoCharacterTable.Add(ToUnichar("00D1"), "N");	// LATIN CAPITAL LETTER N WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00D2"), "O");	// LATIN CAPITAL LETTER O WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00D3"), "O");	// LATIN CAPITAL LETTER O WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00D4"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00D5"), "O");	// LATIN CAPITAL LETTER O WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00D6"), "O");	// LATIN CAPITAL LETTER O WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00D8"), "O");	// LATIN CAPITAL LETTER O WITH STROKE -- no decom
                    seoCharacterTable.Add(ToUnichar("00D9"), "U");	// LATIN CAPITAL LETTER U WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00DA"), "U");	// LATIN CAPITAL LETTER U WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00DB"), "U");	// LATIN CAPITAL LETTER U WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00DC"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00DD"), "Y");	// LATIN CAPITAL LETTER Y WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00DE"), "Th");	// LATIN CAPITAL LETTER THORN -- no decomposition; // Thorn - Could be nothing other than thorn
                    seoCharacterTable.Add(ToUnichar("00DF"), "s");	// LATIN SMALL LETTER SHARP S -- no decomposition
                    seoCharacterTable.Add(ToUnichar("00E0"), "a");	// LATIN SMALL LETTER A WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00E1"), "a");	// LATIN SMALL LETTER A WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00E2"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00E3"), "a");	// LATIN SMALL LETTER A WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00E4"), "a");	// LATIN SMALL LETTER A WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00E5"), "a");	// LATIN SMALL LETTER A WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("00E6"), "ae");	// LATIN SMALL LETTER AE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("00E7"), "c");	// LATIN SMALL LETTER C WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("00E8"), "e");	// LATIN SMALL LETTER E WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00E9"), "e");	// LATIN SMALL LETTER E WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00EA"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00EB"), "e");	// LATIN SMALL LETTER E WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00EC"), "i");	// LATIN SMALL LETTER I WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00ED"), "i");	// LATIN SMALL LETTER I WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00EE"), "i");	// LATIN SMALL LETTER I WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00EF"), "i");	// LATIN SMALL LETTER I WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00F0"), "d");	// LATIN SMALL LETTER ETH -- no decomposition         // small eth, "d" for benefit of Vietnamese
                    seoCharacterTable.Add(ToUnichar("00F1"), "n");	// LATIN SMALL LETTER N WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00F2"), "o");	// LATIN SMALL LETTER O WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00F3"), "o");	// LATIN SMALL LETTER O WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00F4"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00F5"), "o");	// LATIN SMALL LETTER O WITH TILDE
                    seoCharacterTable.Add(ToUnichar("00F6"), "o");	// LATIN SMALL LETTER O WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00F8"), "o");	// LATIN SMALL LETTER O WITH STROKE -- no decompo
                    seoCharacterTable.Add(ToUnichar("00F9"), "u");	// LATIN SMALL LETTER U WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("00FA"), "u");	// LATIN SMALL LETTER U WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00FB"), "u");	// LATIN SMALL LETTER U WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("00FC"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("00FD"), "y");	// LATIN SMALL LETTER Y WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("00FE"), "th");	// LATIN SMALL LETTER THORN -- no decomposition  // Small thorn
                    seoCharacterTable.Add(ToUnichar("00FF"), "y");	// LATIN SMALL LETTER Y WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("0100"), "A");	// LATIN CAPITAL LETTER A WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0101"), "a");	// LATIN SMALL LETTER A WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0102"), "A");	// LATIN CAPITAL LETTER A WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0103"), "a");	// LATIN SMALL LETTER A WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0104"), "A");	// LATIN CAPITAL LETTER A WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0105"), "a");	// LATIN SMALL LETTER A WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0106"), "C");	// LATIN CAPITAL LETTER C WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0107"), "c");	// LATIN SMALL LETTER C WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0108"), "C");	// LATIN CAPITAL LETTER C WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0109"), "c");	// LATIN SMALL LETTER C WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("010A"), "C");	// LATIN CAPITAL LETTER C WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("010B"), "c");	// LATIN SMALL LETTER C WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("010C"), "C");	// LATIN CAPITAL LETTER C WITH CARON
                    seoCharacterTable.Add(ToUnichar("010D"), "c");	// LATIN SMALL LETTER C WITH CARON
                    seoCharacterTable.Add(ToUnichar("010E"), "D");	// LATIN CAPITAL LETTER D WITH CARON
                    seoCharacterTable.Add(ToUnichar("010F"), "d");	// LATIN SMALL LETTER D WITH CARON
                    seoCharacterTable.Add(ToUnichar("0110"), "D");	// LATIN CAPITAL LETTER D WITH STROKE -- no decomposition                     // Capital D with stroke
                    seoCharacterTable.Add(ToUnichar("0111"), "d");	// LATIN SMALL LETTER D WITH STROKE -- no decomposition                       // small D with stroke
                    seoCharacterTable.Add(ToUnichar("0112"), "E");	// LATIN CAPITAL LETTER E WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0113"), "e");	// LATIN SMALL LETTER E WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0114"), "E");	// LATIN CAPITAL LETTER E WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0115"), "e");	// LATIN SMALL LETTER E WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0116"), "E");	// LATIN CAPITAL LETTER E WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0117"), "e");	// LATIN SMALL LETTER E WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0118"), "E");	// LATIN CAPITAL LETTER E WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0119"), "e");	// LATIN SMALL LETTER E WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("011A"), "E");	// LATIN CAPITAL LETTER E WITH CARON
                    seoCharacterTable.Add(ToUnichar("011B"), "e");	// LATIN SMALL LETTER E WITH CARON
                    seoCharacterTable.Add(ToUnichar("011C"), "G");	// LATIN CAPITAL LETTER G WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("011D"), "g");	// LATIN SMALL LETTER G WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("011E"), "G");	// LATIN CAPITAL LETTER G WITH BREVE
                    seoCharacterTable.Add(ToUnichar("011F"), "g");	// LATIN SMALL LETTER G WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0120"), "G");	// LATIN CAPITAL LETTER G WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0121"), "g");	// LATIN SMALL LETTER G WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0122"), "G");	// LATIN CAPITAL LETTER G WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0123"), "g");	// LATIN SMALL LETTER G WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0124"), "H");	// LATIN CAPITAL LETTER H WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0125"), "h");	// LATIN SMALL LETTER H WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0126"), "H");	// LATIN CAPITAL LETTER H WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0127"), "h");	// LATIN SMALL LETTER H WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0128"), "I");	// LATIN CAPITAL LETTER I WITH TILDE
                    seoCharacterTable.Add(ToUnichar("0129"), "i");	// LATIN SMALL LETTER I WITH TILDE
                    seoCharacterTable.Add(ToUnichar("012A"), "I");	// LATIN CAPITAL LETTER I WITH MACRON
                    seoCharacterTable.Add(ToUnichar("012B"), "i");	// LATIN SMALL LETTER I WITH MACRON
                    seoCharacterTable.Add(ToUnichar("012C"), "I");	// LATIN CAPITAL LETTER I WITH BREVE
                    seoCharacterTable.Add(ToUnichar("012D"), "i");	// LATIN SMALL LETTER I WITH BREVE
                    seoCharacterTable.Add(ToUnichar("012E"), "I");	// LATIN CAPITAL LETTER I WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("012F"), "i");	// LATIN SMALL LETTER I WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0130"), "I");	// LATIN CAPITAL LETTER I WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0131"), "i");	// LATIN SMALL LETTER DOTLESS I -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0132"), "I");	// LATIN CAPITAL LIGATURE IJ    
                    seoCharacterTable.Add(ToUnichar("0133"), "i");	// LATIN SMALL LIGATURE IJ      
                    seoCharacterTable.Add(ToUnichar("0134"), "J");	// LATIN CAPITAL LETTER J WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0135"), "j");	// LATIN SMALL LETTER J WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0136"), "K");	// LATIN CAPITAL LETTER K WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0137"), "k");	// LATIN SMALL LETTER K WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0138"), "k");	// LATIN SMALL LETTER KRA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0139"), "L");	// LATIN CAPITAL LETTER L WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("013A"), "l");	// LATIN SMALL LETTER L WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("013B"), "L");	// LATIN CAPITAL LETTER L WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("013C"), "l");	// LATIN SMALL LETTER L WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("013D"), "L");	// LATIN CAPITAL LETTER L WITH CARON
                    seoCharacterTable.Add(ToUnichar("013E"), "l");	// LATIN SMALL LETTER L WITH CARON
                    seoCharacterTable.Add(ToUnichar("013F"), "L");	// LATIN CAPITAL LETTER L WITH MIDDLE DOT
                    seoCharacterTable.Add(ToUnichar("0140"), "l");	// LATIN SMALL LETTER L WITH MIDDLE DOT
                    seoCharacterTable.Add(ToUnichar("0141"), "L");	// LATIN CAPITAL LETTER L WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0142"), "l");	// LATIN SMALL LETTER L WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0143"), "N");	// LATIN CAPITAL LETTER N WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0144"), "n");	// LATIN SMALL LETTER N WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0145"), "N");	// LATIN CAPITAL LETTER N WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0146"), "n");	// LATIN SMALL LETTER N WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0147"), "N");	// LATIN CAPITAL LETTER N WITH CARON
                    seoCharacterTable.Add(ToUnichar("0148"), "n");	// LATIN SMALL LETTER N WITH CARON
                    seoCharacterTable.Add(ToUnichar("0149"), "'n");	// LATIN SMALL LETTER N PRECEDED BY APOSTROPHE                              ;
                    seoCharacterTable.Add(ToUnichar("014A"), "NG");	// LATIN CAPITAL LETTER ENG -- no decomposition                             ;
                    seoCharacterTable.Add(ToUnichar("014B"), "ng");	// LATIN SMALL LETTER ENG -- no decomposition                               ;
                    seoCharacterTable.Add(ToUnichar("014C"), "O");	// LATIN CAPITAL LETTER O WITH MACRON
                    seoCharacterTable.Add(ToUnichar("014D"), "o");	// LATIN SMALL LETTER O WITH MACRON
                    seoCharacterTable.Add(ToUnichar("014E"), "O");	// LATIN CAPITAL LETTER O WITH BREVE
                    seoCharacterTable.Add(ToUnichar("014F"), "o");	// LATIN SMALL LETTER O WITH BREVE
                    seoCharacterTable.Add(ToUnichar("0150"), "O");	// LATIN CAPITAL LETTER O WITH DOUBLE ACUTE
                    seoCharacterTable.Add(ToUnichar("0151"), "o");	// LATIN SMALL LETTER O WITH DOUBLE ACUTE
                    seoCharacterTable.Add(ToUnichar("0152"), "OE");	// LATIN CAPITAL LIGATURE OE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0153"), "oe");	// LATIN SMALL LIGATURE OE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0154"), "R");	// LATIN CAPITAL LETTER R WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0155"), "r");	// LATIN SMALL LETTER R WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("0156"), "R");	// LATIN CAPITAL LETTER R WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0157"), "r");	// LATIN SMALL LETTER R WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0158"), "R");	// LATIN CAPITAL LETTER R WITH CARON
                    seoCharacterTable.Add(ToUnichar("0159"), "r");	// LATIN SMALL LETTER R WITH CARON
                    seoCharacterTable.Add(ToUnichar("015A"), "S");	// LATIN CAPITAL LETTER S WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("015B"), "s");	// LATIN SMALL LETTER S WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("015C"), "S");	// LATIN CAPITAL LETTER S WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("015D"), "s");	// LATIN SMALL LETTER S WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("015E"), "S");	// LATIN CAPITAL LETTER S WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("015F"), "s");	// LATIN SMALL LETTER S WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0160"), "S");	// LATIN CAPITAL LETTER S WITH CARON
                    seoCharacterTable.Add(ToUnichar("0161"), "s");	// LATIN SMALL LETTER S WITH CARON
                    seoCharacterTable.Add(ToUnichar("0162"), "T");	// LATIN CAPITAL LETTER T WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0163"), "t");	// LATIN SMALL LETTER T WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0164"), "T");	// LATIN CAPITAL LETTER T WITH CARON
                    seoCharacterTable.Add(ToUnichar("0165"), "t");	// LATIN SMALL LETTER T WITH CARON
                    seoCharacterTable.Add(ToUnichar("0166"), "T");	// LATIN CAPITAL LETTER T WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0167"), "t");	// LATIN SMALL LETTER T WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0168"), "U");	// LATIN CAPITAL LETTER U WITH TILDE
                    seoCharacterTable.Add(ToUnichar("0169"), "u");	// LATIN SMALL LETTER U WITH TILDE
                    seoCharacterTable.Add(ToUnichar("016A"), "U");	// LATIN CAPITAL LETTER U WITH MACRON
                    seoCharacterTable.Add(ToUnichar("016B"), "u");	// LATIN SMALL LETTER U WITH MACRON
                    seoCharacterTable.Add(ToUnichar("016C"), "U");	// LATIN CAPITAL LETTER U WITH BREVE
                    seoCharacterTable.Add(ToUnichar("016D"), "u");	// LATIN SMALL LETTER U WITH BREVE
                    seoCharacterTable.Add(ToUnichar("016E"), "U");	// LATIN CAPITAL LETTER U WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("016F"), "u");	// LATIN SMALL LETTER U WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("0170"), "U");	// LATIN CAPITAL LETTER U WITH DOUBLE ACUTE
                    seoCharacterTable.Add(ToUnichar("0171"), "u");	// LATIN SMALL LETTER U WITH DOUBLE ACUTE
                    seoCharacterTable.Add(ToUnichar("0172"), "U");	// LATIN CAPITAL LETTER U WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0173"), "u");	// LATIN SMALL LETTER U WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("0174"), "W");	// LATIN CAPITAL LETTER W WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0175"), "w");	// LATIN SMALL LETTER W WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0176"), "Y");	// LATIN CAPITAL LETTER Y WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0177"), "y");	// LATIN SMALL LETTER Y WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("0178"), "Y");	// LATIN CAPITAL LETTER Y WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("0179"), "Z");	// LATIN CAPITAL LETTER Z WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("017A"), "z");	// LATIN SMALL LETTER Z WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("017B"), "Z");	// LATIN CAPITAL LETTER Z WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("017C"), "z");	// LATIN SMALL LETTER Z WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("017D"), "Z");	// LATIN CAPITAL LETTER Z WITH CARON
                    seoCharacterTable.Add(ToUnichar("017E"), "z");	// LATIN SMALL LETTER Z WITH CARON
                    seoCharacterTable.Add(ToUnichar("017F"), "s");	// LATIN SMALL LETTER LONG S    
                    seoCharacterTable.Add(ToUnichar("0180"), "b");	// LATIN SMALL LETTER B WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0181"), "B");	// LATIN CAPITAL LETTER B WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0182"), "B");	// LATIN CAPITAL LETTER B WITH TOPBAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0183"), "b");	// LATIN SMALL LETTER B WITH TOPBAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0184"), "6");	// LATIN CAPITAL LETTER TONE SIX -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0185"), "6");	// LATIN SMALL LETTER TONE SIX -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0186"), "O");	// LATIN CAPITAL LETTER OPEN O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0187"), "C");	// LATIN CAPITAL LETTER C WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0188"), "c");	// LATIN SMALL LETTER C WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0189"), "D");	// LATIN CAPITAL LETTER AFRICAN D -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018A"), "D");	// LATIN CAPITAL LETTER D WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018B"), "D");	// LATIN CAPITAL LETTER D WITH TOPBAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018C"), "d");	// LATIN SMALL LETTER D WITH TOPBAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018D"), "d");	// LATIN SMALL LETTER TURNED DELTA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018E"), "E");	// LATIN CAPITAL LETTER REVERSED E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("018F"), "E");	// LATIN CAPITAL LETTER SCHWA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0190"), "E");	// LATIN CAPITAL LETTER OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0191"), "F");	// LATIN CAPITAL LETTER F WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0192"), "f");	// LATIN SMALL LETTER F WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0193"), "G");	// LATIN CAPITAL LETTER G WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0194"), "G");	// LATIN CAPITAL LETTER GAMMA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0195"), "hv");	// LATIN SMALL LETTER HV -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0196"), "I");	// LATIN CAPITAL LETTER IOTA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0197"), "I");	// LATIN CAPITAL LETTER I WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0198"), "K");	// LATIN CAPITAL LETTER K WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0199"), "k");	// LATIN SMALL LETTER K WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019A"), "l");	// LATIN SMALL LETTER L WITH BAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019B"), "l");	// LATIN SMALL LETTER LAMBDA WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019C"), "M");	// LATIN CAPITAL LETTER TURNED M -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019D"), "N");	// LATIN CAPITAL LETTER N WITH LEFT HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019E"), "n");	// LATIN SMALL LETTER N WITH LONG RIGHT LEG -- no decomposition
                    seoCharacterTable.Add(ToUnichar("019F"), "O");	// LATIN CAPITAL LETTER O WITH MIDDLE TILDE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A0"), "O");	// LATIN CAPITAL LETTER O WITH HORN
                    seoCharacterTable.Add(ToUnichar("01A1"), "o");	// LATIN SMALL LETTER O WITH HORN
                    seoCharacterTable.Add(ToUnichar("01A2"), "OI");	// LATIN CAPITAL LETTER OI -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A3"), "oi");	// LATIN SMALL LETTER OI -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A4"), "P");	// LATIN CAPITAL LETTER P WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A5"), "p");	// LATIN SMALL LETTER P WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A6"), "YR");	// LATIN LETTER YR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A7"), "2");	// LATIN CAPITAL LETTER TONE TWO -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A8"), "2");	// LATIN SMALL LETTER TONE TWO -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01A9"), "S");	// LATIN CAPITAL LETTER ESH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AA"), "s");	// LATIN LETTER REVERSED ESH LOOP -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AB"), "t");	// LATIN SMALL LETTER T WITH PALATAL HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AC"), "T");	// LATIN CAPITAL LETTER T WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AD"), "t");	// LATIN SMALL LETTER T WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AE"), "T");	// LATIN CAPITAL LETTER T WITH RETROFLEX HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01AF"), "U");	// LATIN CAPITAL LETTER U WITH HORN
                    seoCharacterTable.Add(ToUnichar("01B0"), "u");	// LATIN SMALL LETTER U WITH HORN
                    seoCharacterTable.Add(ToUnichar("01B1"), "u");	// LATIN CAPITAL LETTER UPSILON -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B2"), "V");	// LATIN CAPITAL LETTER V WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B3"), "Y");	// LATIN CAPITAL LETTER Y WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B4"), "y");	// LATIN SMALL LETTER Y WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B5"), "Z");	// LATIN CAPITAL LETTER Z WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B6"), "z");	// LATIN SMALL LETTER Z WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B7"), "Z");	// LATIN CAPITAL LETTER EZH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B8"), "Z");	// LATIN CAPITAL LETTER EZH REVERSED -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01B9"), "Z");	// LATIN SMALL LETTER EZH REVERSED -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01BA"), "z");	// LATIN SMALL LETTER EZH WITH TAIL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01BB"), "2");	// LATIN LETTER TWO WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01BC"), "5");	// LATIN CAPITAL LETTER TONE FIVE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01BD"), "5");	// LATIN SMALL LETTER TONE FIVE -- no decomposition
                    //seoCharacterTable.Add(ToUnichar("01BE"), "´");	// LATIN LETTER INVERTED GLOTTAL STOP WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01BF"), "w");	// LATIN LETTER WYNN -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01C0"), "!");	// LATIN LETTER DENTAL CLICK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01C1"), "!");	// LATIN LETTER LATERAL CLICK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01C2"), "!");	// LATIN LETTER ALVEOLAR CLICK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01C3"), "!");	// LATIN LETTER RETROFLEX CLICK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01C4"), "DZ");	// LATIN CAPITAL LETTER DZ WITH CARON
                    seoCharacterTable.Add(ToUnichar("01C5"), "DZ");	// LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON
                    seoCharacterTable.Add(ToUnichar("01C6"), "d");	// LATIN SMALL LETTER DZ WITH CARON
                    seoCharacterTable.Add(ToUnichar("01C7"), "Lj");	// LATIN CAPITAL LETTER LJ
                    seoCharacterTable.Add(ToUnichar("01C8"), "Lj");	// LATIN CAPITAL LETTER L WITH SMALL LETTER J
                    seoCharacterTable.Add(ToUnichar("01C9"), "lj");	// LATIN SMALL LETTER LJ
                    seoCharacterTable.Add(ToUnichar("01CA"), "NJ");	// LATIN CAPITAL LETTER NJ
                    seoCharacterTable.Add(ToUnichar("01CB"), "NJ");	// LATIN CAPITAL LETTER N WITH SMALL LETTER J
                    seoCharacterTable.Add(ToUnichar("01CC"), "nj");	// LATIN SMALL LETTER NJ
                    seoCharacterTable.Add(ToUnichar("01CD"), "A");	// LATIN CAPITAL LETTER A WITH CARON
                    seoCharacterTable.Add(ToUnichar("01CE"), "a");	// LATIN SMALL LETTER A WITH CARON
                    seoCharacterTable.Add(ToUnichar("01CF"), "I");	// LATIN CAPITAL LETTER I WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D0"), "i");	// LATIN SMALL LETTER I WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D1"), "O");	// LATIN CAPITAL LETTER O WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D2"), "o");	// LATIN SMALL LETTER O WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D3"), "U");	// LATIN CAPITAL LETTER U WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D4"), "u");	// LATIN SMALL LETTER U WITH CARON
                    seoCharacterTable.Add(ToUnichar("01D5"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("01D6"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("01D7"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS AND ACUTE
                    seoCharacterTable.Add(ToUnichar("01D8"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS AND ACUTE
                    seoCharacterTable.Add(ToUnichar("01D9"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS AND CARON
                    seoCharacterTable.Add(ToUnichar("01DA"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS AND CARON
                    seoCharacterTable.Add(ToUnichar("01DB"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS AND GRAVE
                    seoCharacterTable.Add(ToUnichar("01DC"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS AND GRAVE
                    seoCharacterTable.Add(ToUnichar("01DD"), "e");	// LATIN SMALL LETTER TURNED E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01DE"), "A");	// LATIN CAPITAL LETTER A WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("01DF"), "a");	// LATIN SMALL LETTER A WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("01E0"), "A");	// LATIN CAPITAL LETTER A WITH DOT ABOVE AND MACRON
                    seoCharacterTable.Add(ToUnichar("01E1"), "a");	// LATIN SMALL LETTER A WITH DOT ABOVE AND MACRON
                    seoCharacterTable.Add(ToUnichar("01E2"), "AE");	// LATIN CAPITAL LETTER AE WITH MACRON
                    seoCharacterTable.Add(ToUnichar("01E3"), "ae");	// LATIN SMALL LETTER AE WITH MACRON
                    seoCharacterTable.Add(ToUnichar("01E4"), "G");	// LATIN CAPITAL LETTER G WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01E5"), "g");	// LATIN SMALL LETTER G WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01E6"), "G");	// LATIN CAPITAL LETTER G WITH CARON
                    seoCharacterTable.Add(ToUnichar("01E7"), "g");	// LATIN SMALL LETTER G WITH CARON
                    seoCharacterTable.Add(ToUnichar("01E8"), "K");	// LATIN CAPITAL LETTER K WITH CARON
                    seoCharacterTable.Add(ToUnichar("01E9"), "k");	// LATIN SMALL LETTER K WITH CARON
                    seoCharacterTable.Add(ToUnichar("01EA"), "O");	// LATIN CAPITAL LETTER O WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("01EB"), "o");	// LATIN SMALL LETTER O WITH OGONEK
                    seoCharacterTable.Add(ToUnichar("01EC"), "O");	// LATIN CAPITAL LETTER O WITH OGONEK AND MACRON
                    seoCharacterTable.Add(ToUnichar("01ED"), "o");	// LATIN SMALL LETTER O WITH OGONEK AND MACRON
                    seoCharacterTable.Add(ToUnichar("01EE"), "Z");	// LATIN CAPITAL LETTER EZH WITH CARON
                    seoCharacterTable.Add(ToUnichar("01EF"), "Z");	// LATIN SMALL LETTER EZH WITH CARON
                    seoCharacterTable.Add(ToUnichar("01F0"), "j");	// LATIN SMALL LETTER J WITH CARON
                    seoCharacterTable.Add(ToUnichar("01F1"), "DZ");	// LATIN CAPITAL LETTER DZ
                    seoCharacterTable.Add(ToUnichar("01F2"), "DZ");	// LATIN CAPITAL LETTER D WITH SMALL LETTER Z
                    seoCharacterTable.Add(ToUnichar("01F3"), "dz");	// LATIN SMALL LETTER DZ
                    seoCharacterTable.Add(ToUnichar("01F4"), "G");	// LATIN CAPITAL LETTER G WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("01F5"), "g");	// LATIN SMALL LETTER G WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("01F6"), "hv");	// LATIN CAPITAL LETTER HWAIR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01F7"), "w");	// LATIN CAPITAL LETTER WYNN -- no decomposition
                    seoCharacterTable.Add(ToUnichar("01F8"), "N");	// LATIN CAPITAL LETTER N WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("01F9"), "n");	// LATIN SMALL LETTER N WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("01FA"), "A");	// LATIN CAPITAL LETTER A WITH RING ABOVE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("01FB"), "a");	// LATIN SMALL LETTER A WITH RING ABOVE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("01FC"), "AE");	// LATIN CAPITAL LETTER AE WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("01FD"), "ae");	// LATIN SMALL LETTER AE WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("01FE"), "O");	// LATIN CAPITAL LETTER O WITH STROKE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("01FF"), "o");	// LATIN SMALL LETTER O WITH STROKE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("0200"), "A");	// LATIN CAPITAL LETTER A WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0201"), "a");	// LATIN SMALL LETTER A WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0202"), "A");	// LATIN CAPITAL LETTER A WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0203"), "a");	// LATIN SMALL LETTER A WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0204"), "E");	// LATIN CAPITAL LETTER E WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0205"), "e");	// LATIN SMALL LETTER E WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0206"), "E");	// LATIN CAPITAL LETTER E WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0207"), "e");	// LATIN SMALL LETTER E WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0208"), "I");	// LATIN CAPITAL LETTER I WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0209"), "i");	// LATIN SMALL LETTER I WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("020A"), "I");	// LATIN CAPITAL LETTER I WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("020B"), "i");	// LATIN SMALL LETTER I WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("020C"), "O");	// LATIN CAPITAL LETTER O WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("020D"), "o");	// LATIN SMALL LETTER O WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("020E"), "O");	// LATIN CAPITAL LETTER O WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("020F"), "o");	// LATIN SMALL LETTER O WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0210"), "R");	// LATIN CAPITAL LETTER R WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0211"), "r");	// LATIN SMALL LETTER R WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0212"), "R");	// LATIN CAPITAL LETTER R WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0213"), "r");	// LATIN SMALL LETTER R WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0214"), "U");	// LATIN CAPITAL LETTER U WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0215"), "u");	// LATIN SMALL LETTER U WITH DOUBLE GRAVE
                    seoCharacterTable.Add(ToUnichar("0216"), "U");	// LATIN CAPITAL LETTER U WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0217"), "u");	// LATIN SMALL LETTER U WITH INVERTED BREVE
                    seoCharacterTable.Add(ToUnichar("0218"), "S");	// LATIN CAPITAL LETTER S WITH COMMA BELOW
                    seoCharacterTable.Add(ToUnichar("0219"), "s");	// LATIN SMALL LETTER S WITH COMMA BELOW
                    seoCharacterTable.Add(ToUnichar("021A"), "T");	// LATIN CAPITAL LETTER T WITH COMMA BELOW
                    seoCharacterTable.Add(ToUnichar("021B"), "t");	// LATIN SMALL LETTER T WITH COMMA BELOW
                    seoCharacterTable.Add(ToUnichar("021C"), "Z");	// LATIN CAPITAL LETTER YOGH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("021D"), "z");	// LATIN SMALL LETTER YOGH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("021E"), "H");	// LATIN CAPITAL LETTER H WITH CARON
                    seoCharacterTable.Add(ToUnichar("021F"), "h");	// LATIN SMALL LETTER H WITH CARON
                    seoCharacterTable.Add(ToUnichar("0220"), "N");	// LATIN CAPITAL LETTER N WITH LONG RIGHT LEG -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0221"), "d");	// LATIN SMALL LETTER D WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0222"), "OU");	// LATIN CAPITAL LETTER OU -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0223"), "ou");	// LATIN SMALL LETTER OU -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0224"), "Z");	// LATIN CAPITAL LETTER Z WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0225"), "z");	// LATIN SMALL LETTER Z WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0226"), "A");	// LATIN CAPITAL LETTER A WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0227"), "a");	// LATIN SMALL LETTER A WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0228"), "E");	// LATIN CAPITAL LETTER E WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("0229"), "e");	// LATIN SMALL LETTER E WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("022A"), "O");	// LATIN CAPITAL LETTER O WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("022B"), "o");	// LATIN SMALL LETTER O WITH DIAERESIS AND MACRON
                    seoCharacterTable.Add(ToUnichar("022C"), "O");	// LATIN CAPITAL LETTER O WITH TILDE AND MACRON
                    seoCharacterTable.Add(ToUnichar("022D"), "o");	// LATIN SMALL LETTER O WITH TILDE AND MACRON
                    seoCharacterTable.Add(ToUnichar("022E"), "O");	// LATIN CAPITAL LETTER O WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("022F"), "o");	// LATIN SMALL LETTER O WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("0230"), "O");	// LATIN CAPITAL LETTER O WITH DOT ABOVE AND MACRON
                    seoCharacterTable.Add(ToUnichar("0231"), "o");	// LATIN SMALL LETTER O WITH DOT ABOVE AND MACRON
                    seoCharacterTable.Add(ToUnichar("0232"), "Y");	// LATIN CAPITAL LETTER Y WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0233"), "y");	// LATIN SMALL LETTER Y WITH MACRON
                    seoCharacterTable.Add(ToUnichar("0234"), "l");	// LATIN SMALL LETTER L WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0235"), "n");	// LATIN SMALL LETTER N WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0236"), "t");	// LATIN SMALL LETTER T WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0250"), "a");	// LATIN SMALL LETTER TURNED A -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0251"), "a");	// LATIN SMALL LETTER ALPHA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0252"), "a");	// LATIN SMALL LETTER TURNED ALPHA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0253"), "b");	// LATIN SMALL LETTER B WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0254"), "o");	// LATIN SMALL LETTER OPEN O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0255"), "c");	// LATIN SMALL LETTER C WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0256"), "d");	// LATIN SMALL LETTER D WITH TAIL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0257"), "d");	// LATIN SMALL LETTER D WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0258"), "e");	// LATIN SMALL LETTER REVERSED E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0259"), "e");	// LATIN SMALL LETTER SCHWA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025A"), "e");	// LATIN SMALL LETTER SCHWA WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025B"), "e");	// LATIN SMALL LETTER OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025C"), "e");	// LATIN SMALL LETTER REVERSED OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025D"), "e");	// LATIN SMALL LETTER REVERSED OPEN E WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025E"), "e");	// LATIN SMALL LETTER CLOSED REVERSED OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("025F"), "j");	// LATIN SMALL LETTER DOTLESS J WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0260"), "g");	// LATIN SMALL LETTER G WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0261"), "g");	// LATIN SMALL LETTER SCRIPT G -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0262"), "G");	// LATIN LETTER SMALL CAPITAL G -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0263"), "g");	// LATIN SMALL LETTER GAMMA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0264"), "y");	// LATIN SMALL LETTER RAMS HORN -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0265"), "h");	// LATIN SMALL LETTER TURNED H -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0266"), "h");	// LATIN SMALL LETTER H WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0267"), "h");	// LATIN SMALL LETTER HENG WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0268"), "i");	// LATIN SMALL LETTER I WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0269"), "i");	// LATIN SMALL LETTER IOTA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026A"), "I");	// LATIN LETTER SMALL CAPITAL I -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026B"), "l");	// LATIN SMALL LETTER L WITH MIDDLE TILDE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026C"), "l");	// LATIN SMALL LETTER L WITH BELT -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026D"), "l");	// LATIN SMALL LETTER L WITH RETROFLEX HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026E"), "lz");	// LATIN SMALL LETTER LEZH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("026F"), "m");	// LATIN SMALL LETTER TURNED M -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0270"), "m");	// LATIN SMALL LETTER TURNED M WITH LONG LEG -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0271"), "m");	// LATIN SMALL LETTER M WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0272"), "n");	// LATIN SMALL LETTER N WITH LEFT HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0273"), "n");	// LATIN SMALL LETTER N WITH RETROFLEX HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0274"), "N");	// LATIN LETTER SMALL CAPITAL N -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0275"), "o");	// LATIN SMALL LETTER BARRED O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0276"), "OE");	// LATIN LETTER SMALL CAPITAL OE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0277"), "o");	// LATIN SMALL LETTER CLOSED OMEGA -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0278"), "ph");	// LATIN SMALL LETTER PHI -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0279"), "r");	// LATIN SMALL LETTER TURNED R -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027A"), "r");	// LATIN SMALL LETTER TURNED R WITH LONG LEG -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027B"), "r");	// LATIN SMALL LETTER TURNED R WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027C"), "r");	// LATIN SMALL LETTER R WITH LONG LEG -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027D"), "r");	// LATIN SMALL LETTER R WITH TAIL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027E"), "r");	// LATIN SMALL LETTER R WITH FISHHOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("027F"), "r");	// LATIN SMALL LETTER REVERSED R WITH FISHHOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0280"), "R");	// LATIN LETTER SMALL CAPITAL R -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0281"), "r");	// LATIN LETTER SMALL CAPITAL INVERTED R -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0282"), "s");	// LATIN SMALL LETTER S WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0283"), "s");	// LATIN SMALL LETTER ESH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0284"), "j");	// LATIN SMALL LETTER DOTLESS J WITH STROKE AND HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0285"), "s");	// LATIN SMALL LETTER SQUAT REVERSED ESH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0286"), "s");	// LATIN SMALL LETTER ESH WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0287"), "y");	// LATIN SMALL LETTER TURNED T -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0288"), "t");	// LATIN SMALL LETTER T WITH RETROFLEX HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0289"), "u");	// LATIN SMALL LETTER U BAR -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028A"), "u");	// LATIN SMALL LETTER UPSILON -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028B"), "u");	// LATIN SMALL LETTER V WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028C"), "v");	// LATIN SMALL LETTER TURNED V -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028D"), "w");	// LATIN SMALL LETTER TURNED W -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028E"), "y");	// LATIN SMALL LETTER TURNED Y -- no decomposition
                    seoCharacterTable.Add(ToUnichar("028F"), "Y");	// LATIN LETTER SMALL CAPITAL Y -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0290"), "z");	// LATIN SMALL LETTER Z WITH RETROFLEX HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0291"), "z");	// LATIN SMALL LETTER Z WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0292"), "z");	// LATIN SMALL LETTER EZH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0293"), "z");	// LATIN SMALL LETTER EZH WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0294"), "'");	// LATIN LETTER GLOTTAL STOP -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0295"), "'");	// LATIN LETTER PHARYNGEAL VOICED FRICATIVE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0296"), "'");	// LATIN LETTER INVERTED GLOTTAL STOP -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0297"), "C");	// LATIN LETTER STRETCHED C -- no decomposition
                    //seoCharacterTable.Add(ToUnichar("0298"), "O˜");	// LATIN LETTER BILABIAL CLICK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("0299"), "B");	// LATIN LETTER SMALL CAPITAL B -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029A"), "e");	// LATIN SMALL LETTER CLOSED OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029B"), "G");	// LATIN LETTER SMALL CAPITAL G WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029C"), "H");	// LATIN LETTER SMALL CAPITAL H -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029D"), "j");	// LATIN SMALL LETTER J WITH CROSSED-TAIL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029E"), "k");	// LATIN SMALL LETTER TURNED K -- no decomposition
                    seoCharacterTable.Add(ToUnichar("029F"), "L");	// LATIN LETTER SMALL CAPITAL L -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A0"), "q");	// LATIN SMALL LETTER Q WITH HOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A1"), "'");	// LATIN LETTER GLOTTAL STOP WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A2"), "'");	// LATIN LETTER REVERSED GLOTTAL STOP WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A3"), "dz");	// LATIN SMALL LETTER DZ DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A4"), "dz");	// LATIN SMALL LETTER DEZH DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A5"), "dz");	// LATIN SMALL LETTER DZ DIGRAPH WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A6"), "ts");	// LATIN SMALL LETTER TS DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A7"), "ts");	// LATIN SMALL LETTER TESH DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A8"), ""); // LATIN SMALL LETTER TC DIGRAPH WITH CURL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02A9"), "fn");	// LATIN SMALL LETTER FENG DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AA"), "ls");	// LATIN SMALL LETTER LS DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AB"), "lz");	// LATIN SMALL LETTER LZ DIGRAPH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AC"), "w");	// LATIN LETTER BILABIAL PERCUSSIVE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AD"), "t");	// LATIN LETTER BIDENTAL PERCUSSIVE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AE"), "h");	// LATIN SMALL LETTER TURNED H WITH FISHHOOK -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02AF"), "h");	// LATIN SMALL LETTER TURNED H WITH FISHHOOK AND TAIL -- no decomposition
                    seoCharacterTable.Add(ToUnichar("02B0"), "h");	// MODIFIER LETTER SMALL H
                    seoCharacterTable.Add(ToUnichar("02B1"), "h");	// MODIFIER LETTER SMALL H WITH HOOK
                    seoCharacterTable.Add(ToUnichar("02B2"), "j");	// MODIFIER LETTER SMALL J
                    seoCharacterTable.Add(ToUnichar("02B3"), "r");	// MODIFIER LETTER SMALL R
                    seoCharacterTable.Add(ToUnichar("02B4"), "r");	// MODIFIER LETTER SMALL TURNED R
                    seoCharacterTable.Add(ToUnichar("02B5"), "r");	// MODIFIER LETTER SMALL TURNED R WITH HOOK
                    seoCharacterTable.Add(ToUnichar("02B6"), "R");	// MODIFIER LETTER SMALL CAPITAL INVERTED R
                    seoCharacterTable.Add(ToUnichar("02B7"), "w");	// MODIFIER LETTER SMALL W
                    seoCharacterTable.Add(ToUnichar("02B8"), "y");	// MODIFIER LETTER SMALL Y
                    seoCharacterTable.Add(ToUnichar("02E1"), "l");	// MODIFIER LETTER SMALL L
                    seoCharacterTable.Add(ToUnichar("02E2"), "s");	// MODIFIER LETTER SMALL S
                    seoCharacterTable.Add(ToUnichar("02E3"), "x");	// MODIFIER LETTER SMALL X
                    seoCharacterTable.Add(ToUnichar("02E4"), "'");	// MODIFIER LETTER SMALL REVERSED GLOTTAL STOP
                    seoCharacterTable.Add(ToUnichar("1D00"), "A");	// LATIN LETTER SMALL CAPITAL A -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D01"), "AE");	// LATIN LETTER SMALL CAPITAL AE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D02"), "ae");	// LATIN SMALL LETTER TURNED AE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D03"), "B");	// LATIN LETTER SMALL CAPITAL BARRED B -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D04"), "C");	// LATIN LETTER SMALL CAPITAL C -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D05"), "D");	// LATIN LETTER SMALL CAPITAL D -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D06"), "TH");	// LATIN LETTER SMALL CAPITAL ETH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D07"), "E");	// LATIN LETTER SMALL CAPITAL E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D08"), "e");	// LATIN SMALL LETTER TURNED OPEN E -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D09"), "i");	// LATIN SMALL LETTER TURNED I -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0A"), "J");	// LATIN LETTER SMALL CAPITAL J -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0B"), "K");	// LATIN LETTER SMALL CAPITAL K -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0C"), "L");	// LATIN LETTER SMALL CAPITAL L WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0D"), "M");	// LATIN LETTER SMALL CAPITAL M -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0E"), "N");	// LATIN LETTER SMALL CAPITAL REVERSED N -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D0F"), "O");	// LATIN LETTER SMALL CAPITAL O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D10"), "O");	// LATIN LETTER SMALL CAPITAL OPEN O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D11"), "o");	// LATIN SMALL LETTER SIDEWAYS O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D12"), "o");	// LATIN SMALL LETTER SIDEWAYS OPEN O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D13"), "o");	// LATIN SMALL LETTER SIDEWAYS O WITH STROKE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D14"), "oe");	// LATIN SMALL LETTER TURNED OE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D15"), "ou");	// LATIN LETTER SMALL CAPITAL OU -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D16"), "o");	// LATIN SMALL LETTER TOP HALF O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D17"), "o");	// LATIN SMALL LETTER BOTTOM HALF O -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D18"), "P");	// LATIN LETTER SMALL CAPITAL P -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D19"), "R");	// LATIN LETTER SMALL CAPITAL REVERSED R -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1A"), "R");	// LATIN LETTER SMALL CAPITAL TURNED R -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1B"), "T");	// LATIN LETTER SMALL CAPITAL T -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1C"), "U");	// LATIN LETTER SMALL CAPITAL U -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1D"), "u");	// LATIN SMALL LETTER SIDEWAYS U -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1E"), "u");	// LATIN SMALL LETTER SIDEWAYS DIAERESIZED U -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D1F"), "m");	// LATIN SMALL LETTER SIDEWAYS TURNED M -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D20"), "V");	// LATIN LETTER SMALL CAPITAL V -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D21"), "W");	// LATIN LETTER SMALL CAPITAL W -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D22"), "Z");	// LATIN LETTER SMALL CAPITAL Z -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D23"), "EZH");	// LATIN LETTER SMALL CAPITAL EZH -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D24"), "'");	// LATIN LETTER VOICED LARYNGEAL SPIRANT -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D25"), "L");	// LATIN LETTER AIN -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D2C"), "A");	// MODIFIER LETTER CAPITAL A
                    seoCharacterTable.Add(ToUnichar("1D2D"), "AE");	// MODIFIER LETTER CAPITAL AE
                    seoCharacterTable.Add(ToUnichar("1D2E"), "B");	// MODIFIER LETTER CAPITAL B
                    seoCharacterTable.Add(ToUnichar("1D2F"), "B");	// MODIFIER LETTER CAPITAL BARRED B -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D30"), "D");	// MODIFIER LETTER CAPITAL D
                    seoCharacterTable.Add(ToUnichar("1D31"), "E");	// MODIFIER LETTER CAPITAL E
                    seoCharacterTable.Add(ToUnichar("1D32"), "E");	// MODIFIER LETTER CAPITAL REVERSED E
                    seoCharacterTable.Add(ToUnichar("1D33"), "G");	// MODIFIER LETTER CAPITAL G
                    seoCharacterTable.Add(ToUnichar("1D34"), "H");	// MODIFIER LETTER CAPITAL H
                    seoCharacterTable.Add(ToUnichar("1D35"), "I");	// MODIFIER LETTER CAPITAL I
                    seoCharacterTable.Add(ToUnichar("1D36"), "J");	// MODIFIER LETTER CAPITAL J
                    seoCharacterTable.Add(ToUnichar("1D37"), "K");	// MODIFIER LETTER CAPITAL K
                    seoCharacterTable.Add(ToUnichar("1D38"), "L");	// MODIFIER LETTER CAPITAL L
                    seoCharacterTable.Add(ToUnichar("1D39"), "M");	// MODIFIER LETTER CAPITAL M
                    seoCharacterTable.Add(ToUnichar("1D3A"), "N");	// MODIFIER LETTER CAPITAL N
                    seoCharacterTable.Add(ToUnichar("1D3B"), "N");	// MODIFIER LETTER CAPITAL REVERSED N -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D3C"), "O");	// MODIFIER LETTER CAPITAL O
                    seoCharacterTable.Add(ToUnichar("1D3D"), "OU");	// MODIFIER LETTER CAPITAL OU
                    seoCharacterTable.Add(ToUnichar("1D3E"), "P");	// MODIFIER LETTER CAPITAL P
                    seoCharacterTable.Add(ToUnichar("1D3F"), "R");	// MODIFIER LETTER CAPITAL R
                    seoCharacterTable.Add(ToUnichar("1D40"), "T");	// MODIFIER LETTER CAPITAL T
                    seoCharacterTable.Add(ToUnichar("1D41"), "U");	// MODIFIER LETTER CAPITAL U
                    seoCharacterTable.Add(ToUnichar("1D42"), "W");	// MODIFIER LETTER CAPITAL W
                    seoCharacterTable.Add(ToUnichar("1D43"), "a");	// MODIFIER LETTER SMALL A
                    seoCharacterTable.Add(ToUnichar("1D44"), "a");	// MODIFIER LETTER SMALL TURNED A
                    seoCharacterTable.Add(ToUnichar("1D46"), "ae");	// MODIFIER LETTER SMALL TURNED AE
                    seoCharacterTable.Add(ToUnichar("1D47"), "b");    // MODIFIER LETTER SMALL B
                    seoCharacterTable.Add(ToUnichar("1D48"), "d");    // MODIFIER LETTER SMALL D
                    seoCharacterTable.Add(ToUnichar("1D49"), "e");    // MODIFIER LETTER SMALL E
                    seoCharacterTable.Add(ToUnichar("1D4A"), "e");    // MODIFIER LETTER SMALL SCHWA
                    seoCharacterTable.Add(ToUnichar("1D4B"), "e");    // MODIFIER LETTER SMALL OPEN E
                    seoCharacterTable.Add(ToUnichar("1D4C"), "e");    // MODIFIER LETTER SMALL TURNED OPEN E
                    seoCharacterTable.Add(ToUnichar("1D4D"), "g");    // MODIFIER LETTER SMALL G
                    seoCharacterTable.Add(ToUnichar("1D4E"), "i");    // MODIFIER LETTER SMALL TURNED I -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1D4F"), "k");    // MODIFIER LETTER SMALL K
                    seoCharacterTable.Add(ToUnichar("1D50"), "m");	// MODIFIER LETTER SMALL M
                    seoCharacterTable.Add(ToUnichar("1D51"), "g");	// MODIFIER LETTER SMALL ENG
                    seoCharacterTable.Add(ToUnichar("1D52"), "o");	// MODIFIER LETTER SMALL O
                    seoCharacterTable.Add(ToUnichar("1D53"), "o");	// MODIFIER LETTER SMALL OPEN O
                    seoCharacterTable.Add(ToUnichar("1D54"), "o");	// MODIFIER LETTER SMALL TOP HALF O
                    seoCharacterTable.Add(ToUnichar("1D55"), "o");	// MODIFIER LETTER SMALL BOTTOM HALF O
                    seoCharacterTable.Add(ToUnichar("1D56"), "p");	// MODIFIER LETTER SMALL P
                    seoCharacterTable.Add(ToUnichar("1D57"), "t");	// MODIFIER LETTER SMALL T
                    seoCharacterTable.Add(ToUnichar("1D58"), "u");	// MODIFIER LETTER SMALL U
                    seoCharacterTable.Add(ToUnichar("1D59"), "u");	// MODIFIER LETTER SMALL SIDEWAYS U
                    seoCharacterTable.Add(ToUnichar("1D5A"), "m");	// MODIFIER LETTER SMALL TURNED M
                    seoCharacterTable.Add(ToUnichar("1D5B"), "v");	// MODIFIER LETTER SMALL V
                    seoCharacterTable.Add(ToUnichar("1D62"), "i");	// LATIN SUBSCRIPT SMALL LETTER I
                    seoCharacterTable.Add(ToUnichar("1D63"), "r");	// LATIN SUBSCRIPT SMALL LETTER R
                    seoCharacterTable.Add(ToUnichar("1D64"), "u");	// LATIN SUBSCRIPT SMALL LETTER U
                    seoCharacterTable.Add(ToUnichar("1D65"), "v");	// LATIN SUBSCRIPT SMALL LETTER V
                    seoCharacterTable.Add(ToUnichar("1D6B"), "ue");	// LATIN SMALL LETTER UE -- no decomposition
                    seoCharacterTable.Add(ToUnichar("1E00"), "A");	// LATIN CAPITAL LETTER A WITH RING BELOW
                    seoCharacterTable.Add(ToUnichar("1E01"), "a");	// LATIN SMALL LETTER A WITH RING BELOW
                    seoCharacterTable.Add(ToUnichar("1E02"), "B");	// LATIN CAPITAL LETTER B WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E03"), "b");	// LATIN SMALL LETTER B WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E04"), "B");	// LATIN CAPITAL LETTER B WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E05"), "b");	// LATIN SMALL LETTER B WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E06"), "B");	// LATIN CAPITAL LETTER B WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E07"), "b");	// LATIN SMALL LETTER B WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E08"), "C");	// LATIN CAPITAL LETTER C WITH CEDILLA AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E09"), "c");	// LATIN SMALL LETTER C WITH CEDILLA AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E0A"), "D");	// LATIN CAPITAL LETTER D WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E0B"), "d");	// LATIN SMALL LETTER D WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E0C"), "D");	// LATIN CAPITAL LETTER D WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E0D"), "d");	// LATIN SMALL LETTER D WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E0E"), "D");	// LATIN CAPITAL LETTER D WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E0F"), "d");	// LATIN SMALL LETTER D WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E10"), "D");	// LATIN CAPITAL LETTER D WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("1E11"), "d");	// LATIN SMALL LETTER D WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("1E12"), "D");	// LATIN CAPITAL LETTER D WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E13"), "d");	// LATIN SMALL LETTER D WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E14"), "E");	// LATIN CAPITAL LETTER E WITH MACRON AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1E15"), "e");	// LATIN SMALL LETTER E WITH MACRON AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1E16"), "E");	// LATIN CAPITAL LETTER E WITH MACRON AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E17"), "e");	// LATIN SMALL LETTER E WITH MACRON AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E18"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E19"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E1A"), "E");	// LATIN CAPITAL LETTER E WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E1B"), "e");	// LATIN SMALL LETTER E WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E1C"), "E");	// LATIN CAPITAL LETTER E WITH CEDILLA AND BREVE
                    seoCharacterTable.Add(ToUnichar("1E1D"), "e");	// LATIN SMALL LETTER E WITH CEDILLA AND BREVE
                    seoCharacterTable.Add(ToUnichar("1E1E"), "F");	// LATIN CAPITAL LETTER F WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E1F"), "f");	// LATIN SMALL LETTER F WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E20"), "G");	// LATIN CAPITAL LETTER G WITH MACRON
                    seoCharacterTable.Add(ToUnichar("1E21"), "g");	// LATIN SMALL LETTER G WITH MACRON
                    seoCharacterTable.Add(ToUnichar("1E22"), "H");	// LATIN CAPITAL LETTER H WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E23"), "h");	// LATIN SMALL LETTER H WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E24"), "H");	// LATIN CAPITAL LETTER H WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E25"), "h");	// LATIN SMALL LETTER H WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E26"), "H");	// LATIN CAPITAL LETTER H WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E27"), "h");	// LATIN SMALL LETTER H WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E28"), "H");	// LATIN CAPITAL LETTER H WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("1E29"), "h");	// LATIN SMALL LETTER H WITH CEDILLA
                    seoCharacterTable.Add(ToUnichar("1E2A"), "H");	// LATIN CAPITAL LETTER H WITH BREVE BELOW
                    seoCharacterTable.Add(ToUnichar("1E2B"), "h");	// LATIN SMALL LETTER H WITH BREVE BELOW
                    seoCharacterTable.Add(ToUnichar("1E2C"), "I");	// LATIN CAPITAL LETTER I WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E2D"), "i");	// LATIN SMALL LETTER I WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E2E"), "I");	// LATIN CAPITAL LETTER I WITH DIAERESIS AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E2F"), "i");	// LATIN SMALL LETTER I WITH DIAERESIS AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E30"), "K");	// LATIN CAPITAL LETTER K WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E31"), "k");	// LATIN SMALL LETTER K WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E32"), "K");	// LATIN CAPITAL LETTER K WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E33"), "k");	// LATIN SMALL LETTER K WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E34"), "K");	// LATIN CAPITAL LETTER K WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E35"), "k");	// LATIN SMALL LETTER K WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E36"), "L");	// LATIN CAPITAL LETTER L WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E37"), "l");	// LATIN SMALL LETTER L WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E38"), "L");	// LATIN CAPITAL LETTER L WITH DOT BELOW AND MACRON
                    seoCharacterTable.Add(ToUnichar("1E39"), "l");	// LATIN SMALL LETTER L WITH DOT BELOW AND MACRON
                    seoCharacterTable.Add(ToUnichar("1E3A"), "L");	// LATIN CAPITAL LETTER L WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E3B"), "l");	// LATIN SMALL LETTER L WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E3C"), "L");	// LATIN CAPITAL LETTER L WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E3D"), "l");	// LATIN SMALL LETTER L WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E3E"), "M");	// LATIN CAPITAL LETTER M WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E3F"), "m");	// LATIN SMALL LETTER M WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E40"), "M");	// LATIN CAPITAL LETTER M WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E41"), "m");	// LATIN SMALL LETTER M WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E42"), "M");	// LATIN CAPITAL LETTER M WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E43"), "m");	// LATIN SMALL LETTER M WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E44"), "N");	// LATIN CAPITAL LETTER N WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E45"), "n");	// LATIN SMALL LETTER N WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E46"), "N");	// LATIN CAPITAL LETTER N WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E47"), "n");	// LATIN SMALL LETTER N WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E48"), "N");	// LATIN CAPITAL LETTER N WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E49"), "n");	// LATIN SMALL LETTER N WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E4A"), "N");	// LATIN CAPITAL LETTER N WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E4B"), "n");	// LATIN SMALL LETTER N WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E4C"), "O");	// LATIN CAPITAL LETTER O WITH TILDE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E4D"), "o");	// LATIN SMALL LETTER O WITH TILDE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E4E"), "O");	// LATIN CAPITAL LETTER O WITH TILDE AND DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E4F"), "o");	// LATIN SMALL LETTER O WITH TILDE AND DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E50"), "O");	// LATIN CAPITAL LETTER O WITH MACRON AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1E51"), "o");	// LATIN SMALL LETTER O WITH MACRON AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1E52"), "O");	// LATIN CAPITAL LETTER O WITH MACRON AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E53"), "o");	// LATIN SMALL LETTER O WITH MACRON AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E54"), "P");	// LATIN CAPITAL LETTER P WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E55"), "p");	// LATIN SMALL LETTER P WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E56"), "P");	// LATIN CAPITAL LETTER P WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E57"), "p");	// LATIN SMALL LETTER P WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E58"), "R");	// LATIN CAPITAL LETTER R WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E59"), "r");	// LATIN SMALL LETTER R WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E5A"), "R");	// LATIN CAPITAL LETTER R WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E5B"), "r");	// LATIN SMALL LETTER R WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E5C"), "R");	// LATIN CAPITAL LETTER R WITH DOT BELOW AND MACRON
                    seoCharacterTable.Add(ToUnichar("1E5D"), "r");	// LATIN SMALL LETTER R WITH DOT BELOW AND MACRON
                    seoCharacterTable.Add(ToUnichar("1E5E"), "R");	// LATIN CAPITAL LETTER R WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E5F"), "r");	// LATIN SMALL LETTER R WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E60"), "S");	// LATIN CAPITAL LETTER S WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E61"), "s");	// LATIN SMALL LETTER S WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E62"), "S");	// LATIN CAPITAL LETTER S WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E63"), "s");	// LATIN SMALL LETTER S WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E64"), "S");	// LATIN CAPITAL LETTER S WITH ACUTE AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E65"), "s");	// LATIN SMALL LETTER S WITH ACUTE AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E66"), "S");	// LATIN CAPITAL LETTER S WITH CARON AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E67"), "s");	// LATIN SMALL LETTER S WITH CARON AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E68"), "S");	// LATIN CAPITAL LETTER S WITH DOT BELOW AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E69"), "s");	// LATIN SMALL LETTER S WITH DOT BELOW AND DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E6A"), "T");	// LATIN CAPITAL LETTER T WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E6B"), "t");	// LATIN SMALL LETTER T WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E6C"), "T");	// LATIN CAPITAL LETTER T WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E6D"), "t");	// LATIN SMALL LETTER T WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E6E"), "T");	// LATIN CAPITAL LETTER T WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E6F"), "t");	// LATIN SMALL LETTER T WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E70"), "T");	// LATIN CAPITAL LETTER T WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E71"), "t");	// LATIN SMALL LETTER T WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E72"), "U");	// LATIN CAPITAL LETTER U WITH DIAERESIS BELOW
                    seoCharacterTable.Add(ToUnichar("1E73"), "u");	// LATIN SMALL LETTER U WITH DIAERESIS BELOW
                    seoCharacterTable.Add(ToUnichar("1E74"), "U");	// LATIN CAPITAL LETTER U WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E75"), "u");	// LATIN SMALL LETTER U WITH TILDE BELOW
                    seoCharacterTable.Add(ToUnichar("1E76"), "U");	// LATIN CAPITAL LETTER U WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E77"), "u");	// LATIN SMALL LETTER U WITH CIRCUMFLEX BELOW
                    seoCharacterTable.Add(ToUnichar("1E78"), "U");	// LATIN CAPITAL LETTER U WITH TILDE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E79"), "u");	// LATIN SMALL LETTER U WITH TILDE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1E7A"), "U");	// LATIN CAPITAL LETTER U WITH MACRON AND DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E7B"), "u");	// LATIN SMALL LETTER U WITH MACRON AND DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E7C"), "V");	// LATIN CAPITAL LETTER V WITH TILDE
                    seoCharacterTable.Add(ToUnichar("1E7D"), "v");	// LATIN SMALL LETTER V WITH TILDE
                    seoCharacterTable.Add(ToUnichar("1E7E"), "V");	// LATIN CAPITAL LETTER V WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E7F"), "v");	// LATIN SMALL LETTER V WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E80"), "W");	// LATIN CAPITAL LETTER W WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("1E81"), "w");	// LATIN SMALL LETTER W WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("1E82"), "W");	// LATIN CAPITAL LETTER W WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E83"), "w");	// LATIN SMALL LETTER W WITH ACUTE
                    seoCharacterTable.Add(ToUnichar("1E84"), "W");	// LATIN CAPITAL LETTER W WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E85"), "w");	// LATIN SMALL LETTER W WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E86"), "W");	// LATIN CAPITAL LETTER W WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E87"), "w");	// LATIN SMALL LETTER W WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E88"), "W");	// LATIN CAPITAL LETTER W WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E89"), "w");	// LATIN SMALL LETTER W WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E8A"), "X");	// LATIN CAPITAL LETTER X WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E8B"), "x");	// LATIN SMALL LETTER X WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E8C"), "X");	// LATIN CAPITAL LETTER X WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E8D"), "x");	// LATIN SMALL LETTER X WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E8E"), "Y");	// LATIN CAPITAL LETTER Y WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E8F"), "y");	// LATIN SMALL LETTER Y WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1E90"), "Z");	// LATIN CAPITAL LETTER Z WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("1E91"), "z");	// LATIN SMALL LETTER Z WITH CIRCUMFLEX
                    seoCharacterTable.Add(ToUnichar("1E92"), "Z");	// LATIN CAPITAL LETTER Z WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E93"), "z");	// LATIN SMALL LETTER Z WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1E94"), "Z");	// LATIN CAPITAL LETTER Z WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E95"), "z");	// LATIN SMALL LETTER Z WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E96"), "h");	// LATIN SMALL LETTER H WITH LINE BELOW
                    seoCharacterTable.Add(ToUnichar("1E97"), "t");	// LATIN SMALL LETTER T WITH DIAERESIS
                    seoCharacterTable.Add(ToUnichar("1E98"), "w");	// LATIN SMALL LETTER W WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("1E99"), "y");	// LATIN SMALL LETTER Y WITH RING ABOVE
                    seoCharacterTable.Add(ToUnichar("1E9A"), "a");	// LATIN SMALL LETTER A WITH RIGHT HALF RING
                    seoCharacterTable.Add(ToUnichar("1E9B"), "s");	// LATIN SMALL LETTER LONG S WITH DOT ABOVE
                    seoCharacterTable.Add(ToUnichar("1EA0"), "A");	// LATIN CAPITAL LETTER A WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EA1"), "a");	// LATIN SMALL LETTER A WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EA2"), "A");	// LATIN CAPITAL LETTER A WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EA3"), "a");	// LATIN SMALL LETTER A WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EA4"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EA5"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EA6"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EA7"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EA8"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EA9"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EAA"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EAB"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EAC"), "A");	// LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EAD"), "a");	// LATIN SMALL LETTER A WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EAE"), "A");	// LATIN CAPITAL LETTER A WITH BREVE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EAF"), "a");	// LATIN SMALL LETTER A WITH BREVE AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EB0"), "A");	// LATIN CAPITAL LETTER A WITH BREVE AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EB1"), "a");	// LATIN SMALL LETTER A WITH BREVE AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EB2"), "A");	// LATIN CAPITAL LETTER A WITH BREVE AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EB3"), "a");	// LATIN SMALL LETTER A WITH BREVE AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EB4"), "A");	// LATIN CAPITAL LETTER A WITH BREVE AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EB5"), "a");	// LATIN SMALL LETTER A WITH BREVE AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EB6"), "A");	// LATIN CAPITAL LETTER A WITH BREVE AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EB7"), "a");	// LATIN SMALL LETTER A WITH BREVE AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EB8"), "E");	// LATIN CAPITAL LETTER E WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EB9"), "e");	// LATIN SMALL LETTER E WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EBA"), "E");	// LATIN CAPITAL LETTER E WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EBB"), "e");	// LATIN SMALL LETTER E WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EBC"), "E");	// LATIN CAPITAL LETTER E WITH TILDE
                    seoCharacterTable.Add(ToUnichar("1EBD"), "e");	// LATIN SMALL LETTER E WITH TILDE
                    seoCharacterTable.Add(ToUnichar("1EBE"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EBF"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EC0"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EC1"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EC2"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EC3"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EC4"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EC5"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EC6"), "E");	// LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EC7"), "e");	// LATIN SMALL LETTER E WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EC8"), "I");	// LATIN CAPITAL LETTER I WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EC9"), "i");	// LATIN SMALL LETTER I WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1ECA"), "I");	// LATIN CAPITAL LETTER I WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1ECB"), "i");	// LATIN SMALL LETTER I WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1ECC"), "O");	// LATIN CAPITAL LETTER O WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1ECD"), "o");	// LATIN SMALL LETTER O WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1ECE"), "O");	// LATIN CAPITAL LETTER O WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1ECF"), "o");	// LATIN SMALL LETTER O WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1ED0"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1ED1"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1ED2"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1ED3"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1ED4"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1ED5"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1ED6"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1ED7"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX AND TILDE
                    seoCharacterTable.Add(ToUnichar("1ED8"), "O");	// LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1ED9"), "o");	// LATIN SMALL LETTER O WITH CIRCUMFLEX AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EDA"), "O");	// LATIN CAPITAL LETTER O WITH HORN AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EDB"), "o");	// LATIN SMALL LETTER O WITH HORN AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EDC"), "O");	// LATIN CAPITAL LETTER O WITH HORN AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EDD"), "o");	// LATIN SMALL LETTER O WITH HORN AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EDE"), "O");	// LATIN CAPITAL LETTER O WITH HORN AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EDF"), "o");	// LATIN SMALL LETTER O WITH HORN AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EE0"), "O");	// LATIN CAPITAL LETTER O WITH HORN AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EE1"), "o");	// LATIN SMALL LETTER O WITH HORN AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EE2"), "O");	// LATIN CAPITAL LETTER O WITH HORN AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EE3"), "o");	// LATIN SMALL LETTER O WITH HORN AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EE4"), "U");	// LATIN CAPITAL LETTER U WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EE5"), "u");	// LATIN SMALL LETTER U WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EE6"), "U");	// LATIN CAPITAL LETTER U WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EE7"), "u");	// LATIN SMALL LETTER U WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EE8"), "U");	// LATIN CAPITAL LETTER U WITH HORN AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EE9"), "u");	// LATIN SMALL LETTER U WITH HORN AND ACUTE
                    seoCharacterTable.Add(ToUnichar("1EEA"), "U");	// LATIN CAPITAL LETTER U WITH HORN AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EEB"), "u");	// LATIN SMALL LETTER U WITH HORN AND GRAVE
                    seoCharacterTable.Add(ToUnichar("1EEC"), "U");	// LATIN CAPITAL LETTER U WITH HORN AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EED"), "u");	// LATIN SMALL LETTER U WITH HORN AND HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EEE"), "U");	// LATIN CAPITAL LETTER U WITH HORN AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EEF"), "u");	// LATIN SMALL LETTER U WITH HORN AND TILDE
                    seoCharacterTable.Add(ToUnichar("1EF0"), "U");	// LATIN CAPITAL LETTER U WITH HORN AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EF1"), "u");	// LATIN SMALL LETTER U WITH HORN AND DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EF2"), "Y");	// LATIN CAPITAL LETTER Y WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("1EF3"), "y");	// LATIN SMALL LETTER Y WITH GRAVE
                    seoCharacterTable.Add(ToUnichar("1EF4"), "Y");	// LATIN CAPITAL LETTER Y WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EF5"), "y");	// LATIN SMALL LETTER Y WITH DOT BELOW
                    seoCharacterTable.Add(ToUnichar("1EF6"), "Y");	// LATIN CAPITAL LETTER Y WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EF7"), "y");	// LATIN SMALL LETTER Y WITH HOOK ABOVE
                    seoCharacterTable.Add(ToUnichar("1EF8"), "Y");	// LATIN CAPITAL LETTER Y WITH TILDE
                    seoCharacterTable.Add(ToUnichar("1EF9"), "y");	// LATIN SMALL LETTER Y WITH TILDE
                    seoCharacterTable.Add(ToUnichar("2071"), "i");	// SUPERSCRIPT LATIN SMALL LETTER I
                    seoCharacterTable.Add(ToUnichar("207F"), "n");	// SUPERSCRIPT LATIN SMALL LETTER N
                    seoCharacterTable.Add(ToUnichar("212A"), "K");	// KELVIN SIGN
                    seoCharacterTable.Add(ToUnichar("212B"), "A");	// ANGSTROM SIGN
                    seoCharacterTable.Add(ToUnichar("212C"), "B");	// SCRIPT CAPITAL B
                    seoCharacterTable.Add(ToUnichar("212D"), "C");	// BLACK-LETTER CAPITAL C
                    seoCharacterTable.Add(ToUnichar("212F"), "e");	// SCRIPT SMALL E
                    seoCharacterTable.Add(ToUnichar("2130"), "E");	// SCRIPT CAPITAL E
                    seoCharacterTable.Add(ToUnichar("2131"), "F");	// SCRIPT CAPITAL F
                    seoCharacterTable.Add(ToUnichar("2132"), "F");	// TURNED CAPITAL F -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2133"), "M");	// SCRIPT CAPITAL M
                    seoCharacterTable.Add(ToUnichar("2134"), "0");	// SCRIPT SMALL O
                    seoCharacterTable.Add(ToUnichar("213A"), "0");	// ROTATED CAPITAL Q -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2141"), "G");	// TURNED SANS-SERIF CAPITAL G -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2142"), "L");	// TURNED SANS-SERIF CAPITAL L -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2143"), "L");	// REVERSED SANS-SERIF CAPITAL L -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2144"), "Y");	// TURNED SANS-SERIF CAPITAL Y -- no decomposition
                    seoCharacterTable.Add(ToUnichar("2145"), "D");	// DOUBLE-STRUCK ITALIC CAPITAL D
                    seoCharacterTable.Add(ToUnichar("2146"), "d");	// DOUBLE-STRUCK ITALIC SMALL D
                    seoCharacterTable.Add(ToUnichar("2147"), "e");	// DOUBLE-STRUCK ITALIC SMALL E
                    seoCharacterTable.Add(ToUnichar("2148"), "i");	// DOUBLE-STRUCK ITALIC SMALL I
                    seoCharacterTable.Add(ToUnichar("2149"), "j");	// DOUBLE-STRUCK ITALIC SMALL J
                    seoCharacterTable.Add(ToUnichar("FB00"), "ff");	// LATIN SMALL LIGATURE FF
                    seoCharacterTable.Add(ToUnichar("FB01"), "fi");	// LATIN SMALL LIGATURE FI
                    seoCharacterTable.Add(ToUnichar("FB02"), "fl");	// LATIN SMALL LIGATURE FL
                    seoCharacterTable.Add(ToUnichar("FB03"), "ffi");	// LATIN SMALL LIGATURE FFI
                    seoCharacterTable.Add(ToUnichar("FB04"), "ffl");	// LATIN SMALL LIGATURE FFL
                    seoCharacterTable.Add(ToUnichar("FB05"), "st");	// LATIN SMALL LIGATURE LONG S T
                    seoCharacterTable.Add(ToUnichar("FB06"), "st");	// LATIN SMALL LIGATURE ST
                    seoCharacterTable.Add(ToUnichar("FF21"), "A");	// FULLWIDTH LATIN CAPITAL LETTER B
                    seoCharacterTable.Add(ToUnichar("FF22"), "B");	// FULLWIDTH LATIN CAPITAL LETTER B
                    seoCharacterTable.Add(ToUnichar("FF23"), "C");	// FULLWIDTH LATIN CAPITAL LETTER C
                    seoCharacterTable.Add(ToUnichar("FF24"), "D");	// FULLWIDTH LATIN CAPITAL LETTER D
                    seoCharacterTable.Add(ToUnichar("FF25"), "E");	// FULLWIDTH LATIN CAPITAL LETTER E
                    seoCharacterTable.Add(ToUnichar("FF26"), "F");	// FULLWIDTH LATIN CAPITAL LETTER F
                    seoCharacterTable.Add(ToUnichar("FF27"), "G");	// FULLWIDTH LATIN CAPITAL LETTER G
                    seoCharacterTable.Add(ToUnichar("FF28"), "H");	// FULLWIDTH LATIN CAPITAL LETTER H
                    seoCharacterTable.Add(ToUnichar("FF29"), "I");	// FULLWIDTH LATIN CAPITAL LETTER I
                    seoCharacterTable.Add(ToUnichar("FF2A"), "J");	// FULLWIDTH LATIN CAPITAL LETTER J
                    seoCharacterTable.Add(ToUnichar("FF2B"), "K");	// FULLWIDTH LATIN CAPITAL LETTER K
                    seoCharacterTable.Add(ToUnichar("FF2C"), "L");	// FULLWIDTH LATIN CAPITAL LETTER L
                    seoCharacterTable.Add(ToUnichar("FF2D"), "M");	// FULLWIDTH LATIN CAPITAL LETTER M
                    seoCharacterTable.Add(ToUnichar("FF2E"), "N");	// FULLWIDTH LATIN CAPITAL LETTER N
                    seoCharacterTable.Add(ToUnichar("FF2F"), "O");	// FULLWIDTH LATIN CAPITAL LETTER O
                    seoCharacterTable.Add(ToUnichar("FF30"), "P");	// FULLWIDTH LATIN CAPITAL LETTER P
                    seoCharacterTable.Add(ToUnichar("FF31"), "Q");	// FULLWIDTH LATIN CAPITAL LETTER Q
                    seoCharacterTable.Add(ToUnichar("FF32"), "R");	// FULLWIDTH LATIN CAPITAL LETTER R
                    seoCharacterTable.Add(ToUnichar("FF33"), "S");	// FULLWIDTH LATIN CAPITAL LETTER S
                    seoCharacterTable.Add(ToUnichar("FF34"), "T");	// FULLWIDTH LATIN CAPITAL LETTER T
                    seoCharacterTable.Add(ToUnichar("FF35"), "U");	// FULLWIDTH LATIN CAPITAL LETTER U
                    seoCharacterTable.Add(ToUnichar("FF36"), "V");	// FULLWIDTH LATIN CAPITAL LETTER V
                    seoCharacterTable.Add(ToUnichar("FF37"), "W");	// FULLWIDTH LATIN CAPITAL LETTER W
                    seoCharacterTable.Add(ToUnichar("FF38"), "X");	// FULLWIDTH LATIN CAPITAL LETTER X
                    seoCharacterTable.Add(ToUnichar("FF39"), "Y");	// FULLWIDTH LATIN CAPITAL LETTER Y
                    seoCharacterTable.Add(ToUnichar("FF3A"), "Z");	// FULLWIDTH LATIN CAPITAL LETTER Z
                    seoCharacterTable.Add(ToUnichar("FF41"), "a");	// FULLWIDTH LATIN SMALL LETTER A
                    seoCharacterTable.Add(ToUnichar("FF42"), "b");	// FULLWIDTH LATIN SMALL LETTER B
                    seoCharacterTable.Add(ToUnichar("FF43"), "c");	// FULLWIDTH LATIN SMALL LETTER C
                    seoCharacterTable.Add(ToUnichar("FF44"), "d");	// FULLWIDTH LATIN SMALL LETTER D
                    seoCharacterTable.Add(ToUnichar("FF45"), "e");	// FULLWIDTH LATIN SMALL LETTER E
                    seoCharacterTable.Add(ToUnichar("FF46"), "f");	// FULLWIDTH LATIN SMALL LETTER F
                    seoCharacterTable.Add(ToUnichar("FF47"), "g");	// FULLWIDTH LATIN SMALL LETTER G
                    seoCharacterTable.Add(ToUnichar("FF48"), "h");	// FULLWIDTH LATIN SMALL LETTER H
                    seoCharacterTable.Add(ToUnichar("FF49"), "i");	// FULLWIDTH LATIN SMALL LETTER I
                    seoCharacterTable.Add(ToUnichar("FF4A"), "j");	// FULLWIDTH LATIN SMALL LETTER J
                    seoCharacterTable.Add(ToUnichar("FF4B"), "k");	// FULLWIDTH LATIN SMALL LETTER K
                    seoCharacterTable.Add(ToUnichar("FF4C"), "l");	// FULLWIDTH LATIN SMALL LETTER L
                    seoCharacterTable.Add(ToUnichar("FF4D"), "m");	// FULLWIDTH LATIN SMALL LETTER M
                    seoCharacterTable.Add(ToUnichar("FF4E"), "n");	// FULLWIDTH LATIN SMALL LETTER N
                    seoCharacterTable.Add(ToUnichar("FF4F"), "o");	// FULLWIDTH LATIN SMALL LETTER O
                    seoCharacterTable.Add(ToUnichar("FF50"), "p");	// FULLWIDTH LATIN SMALL LETTER P
                    seoCharacterTable.Add(ToUnichar("FF51"), "q");	// FULLWIDTH LATIN SMALL LETTER Q
                    seoCharacterTable.Add(ToUnichar("FF52"), "r");	// FULLWIDTH LATIN SMALL LETTER R
                    seoCharacterTable.Add(ToUnichar("FF53"), "s");	// FULLWIDTH LATIN SMALL LETTER S
                    seoCharacterTable.Add(ToUnichar("FF54"), "t");	// FULLWIDTH LATIN SMALL LETTER T
                    seoCharacterTable.Add(ToUnichar("FF55"), "u");	// FULLWIDTH LATIN SMALL LETTER U
                    seoCharacterTable.Add(ToUnichar("FF56"), "v");	// FULLWIDTH LATIN SMALL LETTER V
                    seoCharacterTable.Add(ToUnichar("FF57"), "w");	// FULLWIDTH LATIN SMALL LETTER W
                    seoCharacterTable.Add(ToUnichar("FF58"), "x");	// FULLWIDTH LATIN SMALL LETTER X
                    seoCharacterTable.Add(ToUnichar("FF59"), "y");	// FULLWIDTH LATIN SMALL LETTER Y
                    seoCharacterTable.Add(ToUnichar("FF5A"), "z");	// FULLWIDTH LATIN SMALL LETTER Z
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders page meta tag
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="name">Meta name</param>
        /// <param name="content">Content</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderMetaTag(Page page, string name,
            string content, bool overwriteExisting)
        {
            if (page == null || page.Header == null)
                return;

            if (content == null)
                content = string.Empty;

            HtmlMeta control = page.Header.Controls.OfType<HtmlMeta>().FirstOrDefault(
                meta => string.Equals(meta.Name, name, StringComparison.OrdinalIgnoreCase));
            if (control == null)
            {
                control = new HtmlMeta();
                control.Name = name;
                control.Content = content;
                page.Header.Controls.Add(control);
            }
            else
            {
                if (overwriteExisting)
                    control.Content = content;
                else
                {
                    if (String.IsNullOrEmpty(control.Content))
                        control.Content = content;
                }
            }
        }

        /// <summary>
        /// Renders page title
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">Page title</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderTitle(Page page, string title, bool overwriteExisting)
        {
            bool includeStoreNameInTitle = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.IncludeStoreNameInTitle");
            RenderTitle(page, title, includeStoreNameInTitle, overwriteExisting);
        }

        /// <summary>
        /// Renders page title
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">Page title</param>
        /// <param name="includeStoreNameInTitle">Include store name in title</param>
        /// <param name="overwriteExisting">Overwrite existing content if exists</param>
        public static void RenderTitle(Page page, string title, 
            bool includeStoreNameInTitle, bool overwriteExisting)
        {
            if (page == null || page.Header == null)
                return;

            if (includeStoreNameInTitle)
                title = IoC.Resolve<ISettingManager>().StoreName + ". " + title;

            if (String.IsNullOrEmpty(title))
                return;

            if (overwriteExisting)
                page.Title = HttpUtility.HtmlEncode(title);
            else
            {
                if (String.IsNullOrEmpty(page.Title))
                    page.Title = HttpUtility.HtmlEncode(title);
            }
        }

        /// <summary>
        /// Renders an RSS link to the page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="title">RSS Title</param>
        /// <param name="href">Path to the RSS feed</param>
        public static void RenderHeaderRssLink(Page page, string title, string href)
        {
            if (page == null || page.Header == null)
                return;

            var link = new HtmlGenericControl("link");
            link.Attributes.Add("href", href);
            link.Attributes.Add("type", "application/rss+xml");
            link.Attributes.Add("rel", "alternate");
            link.Attributes.Add("title", title);
            page.Header.Controls.Add(link);
        }

        /// <summary>
        /// Renders 'canonical' tag
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="url">URL</param>
        public static void RenderCanonicalTag(Page page, string url)
        {
            if (page == null || page.Header == null)
                return;

            if (String.IsNullOrEmpty(url))
                return;

            Literal canonical = new Literal();
            canonical.Text = string.Format("<link rel=\"canonical\" href=\"{0}\" />", url);
            page.Header.Controls.Add(canonical);

            //HtmlLink canonical = new HtmlLink();
            //canonical.Attributes["rel"] = "canonical";
            //canonical.Href = url;
            //page.Header.Controls.Add(canonical);
        }

        /// <summary>
        /// Get SE name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public static string GetSEName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return string.Empty;
            string OKChars = "abcdefghijklmnopqrstuvwxyz1234567890 _-";
            name = name.Trim().ToLowerInvariant();

            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEONames.ConvertNonWesternChars"))
            {
                if (seoCharacterTable == null)
                    InitializeSeoCharacterTable();
            }

            var sb = new StringBuilder();
            foreach (char c in name.ToCharArray())
            {
                string c2 = c.ToString();
                if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEONames.ConvertNonWesternChars"))
                {
                    if (seoCharacterTable.ContainsKey(c2))
                        c2 = seoCharacterTable[c2];
                }

                if (OKChars.Contains(c2))
                    sb.Append(c2);
            }
            string name2 = sb.ToString();
            name2 = name2.Replace(" ", "-");
            while (name2.Contains("--"))
                name2 = name2.Replace("--", "-");
            while (name2.Contains("__"))
                name2 = name2.Replace("__", "_");
            return HttpContext.Current.Server.UrlEncode(name2);
        }

        /// <summary>
        /// Gets access denied page URL for admin area
        /// </summary>
        /// <returns>Result URL</returns>
        public static string GetAdminAreaAccessDeniedUrl()
        {
            string url = CommonHelper.GetStoreAdminLocation() + "AccessDenied.aspx";
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets my account URL
        /// </summary>
        /// <returns>My account URL</returns>
        public static string GetMyAccountUrl()
        {
            string url = string.Format("{0}Account.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets shopping cart URL
        /// </summary>
        /// <returns>Shopping cart URL</returns>
        public static string GetShoppingCartUrl()
        {
            string url = string.Format("{0}ShoppingCart.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets wishlistt URL
        /// </summary>
        /// <returns>Wishlist URL</returns>
        public static string GetWishlistUrl()
        {
            string url = string.Format("{0}Wishlist.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets wishlistt URL
        /// </summary>
        /// <param name="customerGuid">Customer Guid</param>
        /// <returns>Wishlist URL</returns>
        public static string GetWishlistUrl(Guid customerGuid)
        {
            string url = SEOHelper.GetWishlistUrl();
            url = CommonHelper.ModifyQueryString(url, "CustomerGUID=" + customerGuid.ToString(), null);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl()
        {
            return GetLoginPageUrl(string.Empty);
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="returnUrl">Return url</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(string returnUrl)
        {
            string loginUrl = string.Empty;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx?ReturnUrl={1}",
                    CommonHelper.GetStoreLocation(),
                    HttpUtility.UrlEncode(returnUrl));
            }
            else
            {
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx",
                    CommonHelper.GetStoreLocation());
            }
            return loginUrl.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="addCurrentPageUrl">A value indicating whether add current page url as "ReturnURL" parameter</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(bool addCurrentPageUrl)
        {
            return GetLoginPageUrl(addCurrentPageUrl, false);
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="addCurrentPageUrl">A value indicating whether add current page url as "ReturnURL" parameter</param>
        /// <param name="checkoutAsGuest">A value indicating whether login page will show "Checkout as a guest or Register" message</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageUrl(bool addCurrentPageUrl, bool checkoutAsGuest)
        {
            string loginUrl = string.Empty;
            if (addCurrentPageUrl)
            {
                string rawUrl = string.Empty;
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    rawUrl = HttpContext.Current.Request.RawUrl;
                loginUrl = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx?ReturnUrl={1}",
                    CommonHelper.GetStoreLocation(),
                    HttpUtility.UrlEncode(rawUrl));
            }
            else
            {
                loginUrl = GetLoginPageUrl();
            }

            if (checkoutAsGuest)
            {
                loginUrl = CommonHelper.ModifyQueryString(loginUrl, "CheckoutAsGuest=true", string.Empty);
            }
            return loginUrl.ToLowerInvariant();
        }

        /// <summary>
        /// Gets login page URL of admin area
        /// </summary>
        /// <returns>Login page URL</returns>
        public static string GetAdminAreaLoginPageUrl()
        {
            string url = string.Format(CultureInfo.InvariantCulture, "{0}Login.aspx",
                      CommonHelper.GetStoreAdminLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets product SE (search engine) name
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product SE (search engine) name</returns>
        public static string GetProductSEName(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            string seName = GetSEName(product.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(product.Name);
            }
            return seName;
        }

        /// <summary>
        /// Gets product URL
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product URL</returns>
        public static string GetProductUrl(int productId)
        {
            var product = IoC.Resolve<IProductService>().GetProductById(productId);
            return GetProductUrl(product);
        }

        /// <summary>
        /// Gets product URL
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Product URL</returns>
        public static string GetProductUrl(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            string seName = GetProductSEName(product);

            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Product.UrlRewriteFormat") : "{0}Product.aspx?ProductID={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), product.ProductId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets product email a friend URL
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>Product email a friend URL</returns>
        public static string GetProductEmailAFriendUrl(int productId)
        {
            string url = string.Format("{0}ProductEmailAFriend.aspx?ProductId={1}", CommonHelper.GetStoreLocation(), productId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets wishlist email a friend URL
        /// </summary>
        /// <returns>Wishlist email a friend URL</returns>
        public static string GetWishlistEmailAFriendUrl()
        {
            string url = string.Format("{0}WishlistEmailAFriend.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets manufacturer SE (search engine) name
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Manufacturer SE (search engine) name</returns>
        public static string GetManufacturerSEName(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");
            string seName = GetSEName(manufacturer.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(manufacturer.Name);
            }
            return seName;
        }

        /// <summary>
        /// Gets manufacturer URL
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <returns>Manufacturer URL</returns>
        public static string GetManufacturerUrl(int manufacturerId)
        {
            var manufacturer = IoC.Resolve<IManufacturerService>().GetManufacturerById(manufacturerId);
            return GetManufacturerUrl(manufacturer);
        }

        /// <summary>
        /// Gets manufacturer URL
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <returns>Manufacturer URL</returns>
        public static string GetManufacturerUrl(Manufacturer manufacturer)
        {
            if (manufacturer == null)
                throw new ArgumentNullException("manufacturer");
            string seName = GetManufacturerSEName(manufacturer);

            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Manufacturer.UrlRewriteFormat") : "{0}Manufacturer.aspx?ManufacturerID={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), manufacturer.ManufacturerId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets category SE (search engine) name
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category SE (search engine) name</returns>
        public static string GetCategorySEName(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            string seName = GetSEName(category.SEName);
            if (String.IsNullOrEmpty(seName))
            {
                seName = GetSEName(category.Name);
            }
            return seName;
        }

        /// <summary>
        /// Gets category URL
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category URL</returns>
        public static string GetCategoryUrl(int categoryId)
        {
            var category = IoC.Resolve<ICategoryService>().GetCategoryById(categoryId);
            return GetCategoryUrl(category);
        }

        /// <summary>
        /// Gets category URL
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category URL</returns>
        public static string GetCategoryUrl(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            string seName = GetCategorySEName(category);

            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Category.UrlRewriteFormat") : "{0}Category.aspx?CategoryID={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), category.CategoryId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets product tag URL
        /// </summary>
        /// <param name="productTagId">Product tag identifier</param>
        /// <returns>product tag URL</returns>
        public static string GetProductTagUrl(int productTagId)
        {
            var productTag = IoC.Resolve<IProductService>().GetProductTagById(productTagId);
            return GetProductTagUrl(productTag);
        }

        /// <summary>
        /// Gets product tag URL
        /// </summary>
        /// <param name="productTag">Product tag</param>
        /// <returns>Product tag URL</returns>
        public static string GetProductTagUrl(ProductTag productTag)
        {
            if (productTag == null)
                throw new ArgumentNullException("productTag");
            string seName = GetSEName(productTag.Name);

            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ProductTags.UrlRewriteFormat") : "{0}ProductTag.aspx?tagid={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), productTag.ProductTagId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog URL
        /// </summary>
        /// <returns>Blog URL</returns>
        public static string GetBlogUrl()
        {
            string url = string.Format("{0}blog.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog URL
        /// </summary>
        /// <param name="tag">Blog tag</param>
        /// <returns>Blog URL</returns>
        public static string GetBlogUrlForTag(string tag)
        {
            tag = tag.Trim();
            string url = string.Format("{0}blog.aspx?tag={1}", CommonHelper.GetStoreLocation(), tag);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog URL
        /// </summary>
        /// <param name="month">Month</param>
        /// <returns>Blog URL</returns>
        public static string GetBlogUrlByMonth(DateTime month)
        {
            string url = string.Format("{0}blog.aspx?month={1}-{2}", CommonHelper.GetStoreLocation(), month.Year, month.Month);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog post URL
        /// </summary>
        /// <param name="blogPostId">Blog post identifier</param>
        /// <returns>Blog post URL</returns>
        public static string GetBlogPostUrl(int blogPostId)
        {
            var blogPost = IoC.Resolve<IBlogService>().GetBlogPostById(blogPostId);
            return GetBlogPostUrl(blogPost);
        }

        /// <summary>
        /// Gets blog post URL
        /// </summary>
        /// <param name="blogPost">Blog post</param>
        /// <returns>Blog post URL</returns>
        public static string GetBlogPostUrl(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost"); 
            string seName = GetSEName(blogPost.BlogPostTitle);
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Blog.UrlRewriteFormat") : "{0}BlogPost.aspx?BlogPostID={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), blogPost.BlogPostId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets news URL
        /// </summary>
        /// <param name="newsId">News identifier</param>
        /// <returns>News URL</returns>
        public static string GetNewsUrl(int newsId)
        {
            var news = IoC.Resolve<INewsService>().GetNewsById(newsId);
            return GetNewsUrl(news);
        }

        /// <summary>
        /// Gets news URL
        /// </summary>
        /// <param name="news">News item</param>
        /// <returns>News URL</returns>
        public static string GetNewsUrl(News news)
        {
            if (news == null)
                throw new ArgumentNullException("news");
            string seName = GetSEName(news.Title);
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.News.UrlRewriteFormat") : "{0}News.aspx?NewsId={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), news.NewsId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets news Rss URL
        /// </summary>
        /// <returns>News Rss URL</returns>
        public static string GetNewsRssUrl()
        {
            return GetNewsRssUrl(NopContext.Current.WorkingLanguage.LanguageId);
        }

        /// <summary>
        /// Gets news Rss URL
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>News Rss URL</returns>
        public static string GetNewsRssUrl(int languageId)
        {
            string url = CommonHelper.GetStoreLocation() + "NewsRSS.aspx?LanguageId=" + languageId.ToString();
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets blog Rss URL
        /// </summary>
        /// <returns>Blog Rss URL</returns>
        public static string GetBlogRssUrl()
        {
            return GetBlogRssUrl(NopContext.Current.WorkingLanguage.LanguageId);
        }

        /// <summary>
        /// Gets blog Rss URL
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Blog Rss URL</returns>
        public static string GetBlogRssUrl(int languageId)
        {
            string url = CommonHelper.GetStoreLocation() + "BlogRSS.aspx?LanguageId=" + languageId.ToString();
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <returns>Forum URL</returns>
        public static string GetForumMainUrl()
        {
            string url = string.Format("{0}Boards/", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <returns>Forum URL</returns>
        public static string GetForumActiveDiscussionsUrl()
        {
            string url = string.Format("{0}Boards/ActiveDiscussions.aspx", CommonHelper.GetStoreLocation());
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum group URL
        /// </summary>
        /// <param name="forumGroupId">Forum group identifier</param>
        /// <returns>Forum group URL</returns>
        public static string GetForumGroupUrl(int forumGroupId)
        {
            var forumGroup = IoC.Resolve<IForumService>().GetForumGroupById(forumGroupId);
            return GetForumGroupUrl(forumGroup);
        }

        /// <summary>
        /// Gets forum group URL
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        /// <returns>Forum group URL</returns>
        public static string GetForumGroupUrl(ForumGroup forumGroup)
        {
            if(forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }
            string seName = GetSEName(forumGroup.Name);
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ForumGroup.UrlRewriteFormat") : "{0}Boards/ForumGroup.aspx?ForumGroupId={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), forumGroup.ForumGroupId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <returns>Forum URL</returns>
        public static string GetForumUrl(int forumId)
        {
            var forum = IoC.Resolve<IForumService>().GetForumById(forumId);
            return GetForumUrl(forum);
        }

        /// <summary>
        /// Gets forum URL
        /// </summary>
        /// <param name="forum">Forum</param>
        /// <returns>Forum URL</returns>
        public static string GetForumUrl(Forum forum)
        {
            if(forum == null)
            {
                throw new ArgumentNullException("Forum");
            }
            string seName = GetSEName(forum.Name);
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Forum.UrlRewriteFormat") : "{0}Boards/Forum.aspx?ForumId={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), forum.ForumId, seName);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets move topic URL
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <returns>Forum URL</returns>
        public static string GetMoveForumTopicUrl(ForumTopic forumTopic)
        {
            if (forumTopic == null)
                throw new ArgumentNullException("forumTopic");

            string url = string.Format("{0}Boards/MoveTopic.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), forumTopic.ForumTopicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum search URL
        /// </summary>
        /// <param name="searchTerms">Search terms</param>
        /// <returns>Forum URL</returns>
        public static string GetForumSearchUrl(string searchTerms)
        {
            string url = string.Format("{0}Boards/Search.aspx?searchTerms={1}", CommonHelper.GetStoreLocation(), HttpUtility.UrlEncode(searchTerms));
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum advanced search URL
        /// </summary>
        /// <returns>Forum URL</returns>
        public static string GetForumAdvancedSearchUrl()
        {
            string url = string.Format("{0}Boards/Search.aspx", CommonHelper.GetStoreLocation());
            url = CommonHelper.ModifyQueryString(url, "adv=true", string.Empty);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId)
        {
            var topic = IoC.Resolve<IForumService>().GetTopicById(topicId);
            return GetForumTopicUrl(topic); 
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic)
        {
            return GetForumTopicUrl(topic, "p", null);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId, 
            string queryStringProperty, int? pageIndex)
        {
            var topic = IoC.Resolve<IForumService>().GetTopicById(topicId);
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic,
            string queryStringProperty, int? pageIndex)
        {
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex, null);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="postId">Post identifier (anchor)</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(int topicId, string queryStringProperty, 
            int? pageIndex, int? postId)
        {
            var topic = IoC.Resolve<IForumService>().GetTopicById(topicId);
            return GetForumTopicUrl(topic, queryStringProperty, pageIndex, postId);
        }

        /// <summary>
        /// Gets forum topic URL
        /// </summary>
        /// <param name="topic">Forum topic</param>
        /// <param name="queryStringProperty">Query string property</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="postId">Post identifier (anchor)</param>
        /// <returns>Forum topic URL</returns>
        public static string GetForumTopicUrl(ForumTopic topic, string queryStringProperty,
            int? pageIndex, int? postId)
        {
            if(topic == null)
            {
                throw new ArgumentNullException("topic");
            }
            string seName = GetSEName(topic.Subject);
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.ForumTopic.UrlRewriteFormat") : "{0}Boards/Topic.aspx?TopicId={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(), topic.ForumTopicId, seName);
            if(pageIndex.HasValue && pageIndex.Value > 1)
            {
                url = CommonHelper.ModifyQueryString(url, queryStringProperty + "=" + pageIndex.Value.ToString(), null);
            }
            if(postId.HasValue)
            {
                url += string.Format("#{0}", postId.Value);
            }
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum topic URL
        /// </summary>
        /// <param name="forumId">Forum identifier</param>
        /// <returns>New forum topic URL</returns>
        public static string GetNewForumTopicUrl(int forumId)
        {
            string url = string.Format("{0}Boards/TopicNew.aspx?ForumId={1}", CommonHelper.GetStoreLocation(), forumId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets edit topic URL
        /// </summary>
        /// <param name="topicId">Forum post identifier</param>
        /// <returns>Edit forum post URL</returns>
        public static string GetEditForumTopicUrl(int topicId)
        {
            string url = string.Format("{0}Boards/TopicEdit.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), topicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum post URL
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <returns>New forum post URL</returns>
        public static string GetNewForumPostUrl(int topicId)
        {
            string url = string.Format("{0}Boards/PostNew.aspx?TopicId={1}", CommonHelper.GetStoreLocation(), topicId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets new forum post URL with quoting post
        /// </summary>
        /// <param name="topicId">Forum topic identifier</param>
        /// <param name="quotePostId">Quoting post identifier</param>
        /// <returns>New forum post URL</returns>
        public static string GetNewForumPostUrl(int topicId, int quotePostId)
        {
            string url = string.Format("{0}Boards/PostNew.aspx?TopicId={1}&QuotePostId={2}", CommonHelper.GetStoreLocation(), topicId, quotePostId);
            return url.ToLowerInvariant();
        }
        
        /// <summary>
        /// Gets edit post URL
        /// </summary>
        /// <param name="forumPostId">Forum post identifier</param>
        /// <returns>Edit forum post URL</returns>
        public static string GetEditForumPostUrl(int forumPostId)
        {
            string url = string.Format("{0}Boards/PostEdit.aspx?PostId={1}", CommonHelper.GetStoreLocation(), forumPostId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets forum user profile URL
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Forum topic URL</returns>
        public static string GetUserProfileUrl(int userId)
        {
            string url = string.Format("{0}Profile.aspx?UserId={1}", CommonHelper.GetStoreLocation(), userId);
            return url.ToLowerInvariant();
        }

        /// <summary>
        /// Gets Topic page URL
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <param name="title">Localized topic title</param>
        /// <returns>Topic page URL</returns>
        public static string GetTopicUrl(int topicId, string title)
        {
            return GetTopicUrl(topicId, title, CommonHelper.IsCurrentConnectionSecured());
        }

        /// <summary>
        /// Gets Topic page URL
        /// </summary>
        /// <param name="topicId">Topic identifier</param>
        /// <param name="title">Localized topic title</param>
        /// <param name="httpSecured">HTTP secured URL</param>
        /// <returns>Topic page URL</returns>
        public static string GetTopicUrl(int topicId, string title, bool httpSecured)
        {
            string url2 = SEOHelper.EnableUrlRewriting ? IoC.Resolve<ISettingManager>().GetSettingValue("SEO.Topic.UrlRewriteFormat") : "{0}Topic.aspx?TopicId={1}";
            string url = string.Format(url2, CommonHelper.GetStoreLocation(httpSecured), topicId, GetSEName(title));
            return url.ToLowerInvariant();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets a value indicating whether "Url Rewriting enabled" feature is enabled
        /// </summary>
        public static bool EnableUrlRewriting
        {
            get
            {
                bool enableUrlRewriting = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("SEO.EnableUrlRewriting", true);
                return enableUrlRewriting;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("SEO.EnableUrlRewriting", value.ToString());
            }
        }

        #endregion
    }
}
