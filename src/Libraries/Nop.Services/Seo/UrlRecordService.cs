using System.Text;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Services.Seo;

/// <summary>
/// Provides information about URL records
/// </summary>
public partial class UrlRecordService : IUrlRecordService
{
    #region Fields

    protected static readonly Dictionary<char, string> _seoCharacterTable;
    protected static readonly HashSet<char> _okChars;

    protected readonly ILanguageService _languageService;
    protected readonly IRepository<UrlRecord> _urlRecordRepository;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly SeoSettings _seoSettings;

    #endregion

    #region Ctor

    public UrlRecordService(ILanguageService languageService,
        IRepository<UrlRecord> urlRecordRepository,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        SeoSettings seoSettings)
    {
        _languageService = languageService;
        _urlRecordRepository = urlRecordRepository;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        _seoSettings = seoSettings;
    }

    static UrlRecordService()
    {
        _okChars = new HashSet<char>("abcdefghijklmnopqrstuvwxyz1234567890 _-");

        // stores unicode characters and their "normalized"
        // values to a hash table. Character codes are referenced
        // by hex numbers because that's the most common way to
        // refer to them.
        // 
        // upper-case comments are identifiers from the Unicode database. 
        // lower- and mixed-case comments are the author's
        _seoCharacterTable = new Dictionary<char, string>
        {
            { '\u0041', "A" },
            { '\u0042', "B" },
            { '\u0043', "C" },
            { '\u0044', "D" },
            { '\u0045', "E" },
            { '\u0046', "F" },
            { '\u0047', "G" },
            { '\u0048', "H" },
            { '\u0049', "I" },
            { '\u004A', "J" },
            { '\u004B', "K" },
            { '\u004C', "L" },
            { '\u004D', "M" },
            { '\u004E', "N" },
            { '\u004F', "O" },
            { '\u0050', "P" },
            { '\u0051', "Q" },
            { '\u0052', "R" },
            { '\u0053', "S" },
            { '\u0054', "T" },
            { '\u0055', "U" },
            { '\u0056', "V" },
            { '\u0057', "W" },
            { '\u0058', "X" },
            { '\u0059', "Y" },
            { '\u005A', "Z" },
            { '\u0061', "a" },
            { '\u0062', "b" },
            { '\u0063', "c" },
            { '\u0064', "d" },
            { '\u0065', "e" },
            { '\u0066', "f" },
            { '\u0067', "g" },
            { '\u0068', "h" },
            { '\u0069', "i" },
            { '\u006A', "j" },
            { '\u006B', "k" },
            { '\u006C', "l" },
            { '\u006D', "m" },
            { '\u006E', "n" },
            { '\u006F', "o" },
            { '\u0070', "p" },
            { '\u0071', "q" },
            { '\u0072', "r" },
            { '\u0073', "s" },
            { '\u0074', "t" },
            { '\u0075', "u" },
            { '\u0076', "v" },
            { '\u0077', "w" },
            { '\u0078', "x" },
            { '\u0079', "y" },
            { '\u007A', "z" },
            { '\u00AA', "a" }, // FEMININE ORDINAL INDICATOR
            { '\u00BA', "o" }, // MASCULINE ORDINAL INDICATOR
            { '\u00C0', "A" }, // LATIN CAPITAL LETTER A WITH GRAVE
            { '\u00C1', "A" }, // LATIN CAPITAL LETTER A WITH ACUTE
            { '\u00C2', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX
            { '\u00C3', "A" }, // LATIN CAPITAL LETTER A WITH TILDE
            { '\u00C4', "A" }, // LATIN CAPITAL LETTER A WITH DIAERESIS
            { '\u00C5', "A" }, // LATIN CAPITAL LETTER A WITH RING ABOVE
            { '\u00C6', "AE" }, // LATIN CAPITAL LETTER AE -- no decomposition
            { '\u00C7', "C" }, // LATIN CAPITAL LETTER C WITH CEDILLA
            { '\u00C8', "E" }, // LATIN CAPITAL LETTER E WITH GRAVE
            { '\u00C9', "E" }, // LATIN CAPITAL LETTER E WITH ACUTE
            { '\u00CA', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX
            { '\u00CB', "E" }, // LATIN CAPITAL LETTER E WITH DIAERESIS
            { '\u00CC', "I" }, // LATIN CAPITAL LETTER I WITH GRAVE
            { '\u00CD', "I" }, // LATIN CAPITAL LETTER I WITH ACUTE
            { '\u00CE', "I" }, // LATIN CAPITAL LETTER I WITH CIRCUMFLEX
            { '\u00CF', "I" }, // LATIN CAPITAL LETTER I WITH DIAERESIS
            { '\u00D0', "D" }, // LATIN CAPITAL LETTER ETH -- no decomposition // Eth [D for Vietnamese]
            { '\u00D1', "N" }, // LATIN CAPITAL LETTER N WITH TILDE
            { '\u00D2', "O" }, // LATIN CAPITAL LETTER O WITH GRAVE
            { '\u00D3', "O" }, // LATIN CAPITAL LETTER O WITH ACUTE
            { '\u00D4', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX
            { '\u00D5', "O" }, // LATIN CAPITAL LETTER O WITH TILDE
            { '\u00D6', "O" }, // LATIN CAPITAL LETTER O WITH DIAERESIS
            { '\u00D8', "O" }, // LATIN CAPITAL LETTER O WITH STROKE -- no decom
            { '\u00D9', "U" }, // LATIN CAPITAL LETTER U WITH GRAVE
            { '\u00DA', "U" }, // LATIN CAPITAL LETTER U WITH ACUTE
            { '\u00DB', "U" }, // LATIN CAPITAL LETTER U WITH CIRCUMFLEX
            { '\u00DC', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS
            { '\u00DD', "Y" }, // LATIN CAPITAL LETTER Y WITH ACUTE
            { '\u00DE', "Th" }, // LATIN CAPITAL LETTER THORN -- no decomposition; // Thorn - Could be nothing other than thorn
            { '\u00DF', "s" }, // LATIN SMALL LETTER SHARP S -- no decomposition
            { '\u00E0', "a" }, // LATIN SMALL LETTER A WITH GRAVE
            { '\u00E1', "a" }, // LATIN SMALL LETTER A WITH ACUTE
            { '\u00E2', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX
            { '\u00E3', "a" }, // LATIN SMALL LETTER A WITH TILDE
            { '\u00E4', "a" }, // LATIN SMALL LETTER A WITH DIAERESIS
            { '\u00E5', "a" }, // LATIN SMALL LETTER A WITH RING ABOVE
            { '\u00E6', "ae" }, // LATIN SMALL LETTER AE -- no decomposition
            { '\u00E7', "c" }, // LATIN SMALL LETTER C WITH CEDILLA
            { '\u00E8', "e" }, // LATIN SMALL LETTER E WITH GRAVE
            { '\u00E9', "e" }, // LATIN SMALL LETTER E WITH ACUTE
            { '\u00EA', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX
            { '\u00EB', "e" }, // LATIN SMALL LETTER E WITH DIAERESIS
            { '\u00EC', "i" }, // LATIN SMALL LETTER I WITH GRAVE
            { '\u00ED', "i" }, // LATIN SMALL LETTER I WITH ACUTE
            { '\u00EE', "i" }, // LATIN SMALL LETTER I WITH CIRCUMFLEX
            { '\u00EF', "i" }, // LATIN SMALL LETTER I WITH DIAERESIS
            { '\u00F0', "d" }, // LATIN SMALL LETTER ETH -- no decomposition         // small eth, "d" for benefit of Vietnamese
            { '\u00F1', "n" }, // LATIN SMALL LETTER N WITH TILDE
            { '\u00F2', "o" }, // LATIN SMALL LETTER O WITH GRAVE
            { '\u00F3', "o" }, // LATIN SMALL LETTER O WITH ACUTE
            { '\u00F4', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX
            { '\u00F5', "o" }, // LATIN SMALL LETTER O WITH TILDE
            { '\u00F6', "o" }, // LATIN SMALL LETTER O WITH DIAERESIS
            { '\u00F8', "o" }, // LATIN SMALL LETTER O WITH STROKE -- no decompo
            { '\u00F9', "u" }, // LATIN SMALL LETTER U WITH GRAVE
            { '\u00FA', "u" }, // LATIN SMALL LETTER U WITH ACUTE
            { '\u00FB', "u" }, // LATIN SMALL LETTER U WITH CIRCUMFLEX
            { '\u00FC', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS
            { '\u00FD', "y" }, // LATIN SMALL LETTER Y WITH ACUTE
            { '\u00FE', "th" }, // LATIN SMALL LETTER THORN -- no decomposition  // Small thorn
            { '\u00FF', "y" }, // LATIN SMALL LETTER Y WITH DIAERESIS
            { '\u0100', "A" }, // LATIN CAPITAL LETTER A WITH MACRON
            { '\u0101', "a" }, // LATIN SMALL LETTER A WITH MACRON
            { '\u0102', "A" }, // LATIN CAPITAL LETTER A WITH BREVE
            { '\u0103', "a" }, // LATIN SMALL LETTER A WITH BREVE
            { '\u0104', "A" }, // LATIN CAPITAL LETTER A WITH OGONEK
            { '\u0105', "a" }, // LATIN SMALL LETTER A WITH OGONEK
            { '\u0106', "C" }, // LATIN CAPITAL LETTER C WITH ACUTE
            { '\u0107', "c" }, // LATIN SMALL LETTER C WITH ACUTE
            { '\u0108', "C" }, // LATIN CAPITAL LETTER C WITH CIRCUMFLEX
            { '\u0109', "c" }, // LATIN SMALL LETTER C WITH CIRCUMFLEX
            { '\u010A', "C" }, // LATIN CAPITAL LETTER C WITH DOT ABOVE
            { '\u010B', "c" }, // LATIN SMALL LETTER C WITH DOT ABOVE
            { '\u010C', "C" }, // LATIN CAPITAL LETTER C WITH CARON
            { '\u010D', "c" }, // LATIN SMALL LETTER C WITH CARON
            { '\u010E', "D" }, // LATIN CAPITAL LETTER D WITH CARON
            { '\u010F', "d" }, // LATIN SMALL LETTER D WITH CARON
            { '\u0110', "D" }, // LATIN CAPITAL LETTER D WITH STROKE -- no decomposition                     // Capital D with stroke
            { '\u0111', "d" }, // LATIN SMALL LETTER D WITH STROKE -- no decomposition                       // small D with stroke
            { '\u0112', "E" }, // LATIN CAPITAL LETTER E WITH MACRON
            { '\u0113', "e" }, // LATIN SMALL LETTER E WITH MACRON
            { '\u0114', "E" }, // LATIN CAPITAL LETTER E WITH BREVE
            { '\u0115', "e" }, // LATIN SMALL LETTER E WITH BREVE
            { '\u0116', "E" }, // LATIN CAPITAL LETTER E WITH DOT ABOVE
            { '\u0117', "e" }, // LATIN SMALL LETTER E WITH DOT ABOVE
            { '\u0118', "E" }, // LATIN CAPITAL LETTER E WITH OGONEK
            { '\u0119', "e" }, // LATIN SMALL LETTER E WITH OGONEK
            { '\u011A', "E" }, // LATIN CAPITAL LETTER E WITH CARON
            { '\u011B', "e" }, // LATIN SMALL LETTER E WITH CARON
            { '\u011C', "G" }, // LATIN CAPITAL LETTER G WITH CIRCUMFLEX
            { '\u011D', "g" }, // LATIN SMALL LETTER G WITH CIRCUMFLEX
            { '\u011E', "G" }, // LATIN CAPITAL LETTER G WITH BREVE
            { '\u011F', "g" }, // LATIN SMALL LETTER G WITH BREVE
            { '\u0120', "G" }, // LATIN CAPITAL LETTER G WITH DOT ABOVE
            { '\u0121', "g" }, // LATIN SMALL LETTER G WITH DOT ABOVE
            { '\u0122', "G" }, // LATIN CAPITAL LETTER G WITH CEDILLA
            { '\u0123', "g" }, // LATIN SMALL LETTER G WITH CEDILLA
            { '\u0124', "H" }, // LATIN CAPITAL LETTER H WITH CIRCUMFLEX
            { '\u0125', "h" }, // LATIN SMALL LETTER H WITH CIRCUMFLEX
            { '\u0126', "H" }, // LATIN CAPITAL LETTER H WITH STROKE -- no decomposition
            { '\u0127', "h" }, // LATIN SMALL LETTER H WITH STROKE -- no decomposition
            { '\u0128', "I" }, // LATIN CAPITAL LETTER I WITH TILDE
            { '\u0129', "i" }, // LATIN SMALL LETTER I WITH TILDE
            { '\u012A', "I" }, // LATIN CAPITAL LETTER I WITH MACRON
            { '\u012B', "i" }, // LATIN SMALL LETTER I WITH MACRON
            { '\u012C', "I" }, // LATIN CAPITAL LETTER I WITH BREVE
            { '\u012D', "i" }, // LATIN SMALL LETTER I WITH BREVE
            { '\u012E', "I" }, // LATIN CAPITAL LETTER I WITH OGONEK
            { '\u012F', "i" }, // LATIN SMALL LETTER I WITH OGONEK
            { '\u0130', "I" }, // LATIN CAPITAL LETTER I WITH DOT ABOVE
            { '\u0131', "i" }, // LATIN SMALL LETTER DOTLESS I -- no decomposition
            { '\u0132', "I" }, // LATIN CAPITAL LIGATURE IJ    
            { '\u0133', "i" }, // LATIN SMALL LIGATURE IJ      
            { '\u0134', "J" }, // LATIN CAPITAL LETTER J WITH CIRCUMFLEX
            { '\u0135', "j" }, // LATIN SMALL LETTER J WITH CIRCUMFLEX
            { '\u0136', "K" }, // LATIN CAPITAL LETTER K WITH CEDILLA
            { '\u0137', "k" }, // LATIN SMALL LETTER K WITH CEDILLA
            { '\u0138', "k" }, // LATIN SMALL LETTER KRA -- no decomposition
            { '\u0139', "L" }, // LATIN CAPITAL LETTER L WITH ACUTE
            { '\u013A', "l" }, // LATIN SMALL LETTER L WITH ACUTE
            { '\u013B', "L" }, // LATIN CAPITAL LETTER L WITH CEDILLA
            { '\u013C', "l" }, // LATIN SMALL LETTER L WITH CEDILLA
            { '\u013D', "L" }, // LATIN CAPITAL LETTER L WITH CARON
            { '\u013E', "l" }, // LATIN SMALL LETTER L WITH CARON
            { '\u013F', "L" }, // LATIN CAPITAL LETTER L WITH MIDDLE DOT
            { '\u0140', "l" }, // LATIN SMALL LETTER L WITH MIDDLE DOT
            { '\u0141', "L" }, // LATIN CAPITAL LETTER L WITH STROKE -- no decomposition
            { '\u0142', "l" }, // LATIN SMALL LETTER L WITH STROKE -- no decomposition
            { '\u0143', "N" }, // LATIN CAPITAL LETTER N WITH ACUTE
            { '\u0144', "n" }, // LATIN SMALL LETTER N WITH ACUTE
            { '\u0145', "N" }, // LATIN CAPITAL LETTER N WITH CEDILLA
            { '\u0146', "n" }, // LATIN SMALL LETTER N WITH CEDILLA
            { '\u0147', "N" }, // LATIN CAPITAL LETTER N WITH CARON
            { '\u0148', "n" }, // LATIN SMALL LETTER N WITH CARON
            { '\u0149', "'n" }, // LATIN SMALL LETTER N PRECEDED BY APOSTROPHE
            { '\u014A', "NG" }, // LATIN CAPITAL LETTER ENG -- no decomposition
            { '\u014B', "ng" }, // LATIN SMALL LETTER ENG -- no decomposition
            { '\u014C', "O" }, // LATIN CAPITAL LETTER O WITH MACRON
            { '\u014D', "o" }, // LATIN SMALL LETTER O WITH MACRON
            { '\u014E', "O" }, // LATIN CAPITAL LETTER O WITH BREVE
            { '\u014F', "o" }, // LATIN SMALL LETTER O WITH BREVE
            { '\u0150', "O" }, // LATIN CAPITAL LETTER O WITH DOUBLE ACUTE
            { '\u0151', "o" }, // LATIN SMALL LETTER O WITH DOUBLE ACUTE
            { '\u0152', "OE" }, // LATIN CAPITAL LIGATURE OE -- no decomposition
            { '\u0153', "oe" }, // LATIN SMALL LIGATURE OE -- no decomposition
            { '\u0154', "R" }, // LATIN CAPITAL LETTER R WITH ACUTE
            { '\u0155', "r" }, // LATIN SMALL LETTER R WITH ACUTE
            { '\u0156', "R" }, // LATIN CAPITAL LETTER R WITH CEDILLA
            { '\u0157', "r" }, // LATIN SMALL LETTER R WITH CEDILLA
            { '\u0158', "R" }, // LATIN CAPITAL LETTER R WITH CARON
            { '\u0159', "r" }, // LATIN SMALL LETTER R WITH CARON
            { '\u015A', "S" }, // LATIN CAPITAL LETTER S WITH ACUTE
            { '\u015B', "s" }, // LATIN SMALL LETTER S WITH ACUTE
            { '\u015C', "S" }, // LATIN CAPITAL LETTER S WITH CIRCUMFLEX
            { '\u015D', "s" }, // LATIN SMALL LETTER S WITH CIRCUMFLEX
            { '\u015E', "S" }, // LATIN CAPITAL LETTER S WITH CEDILLA
            { '\u015F', "s" }, // LATIN SMALL LETTER S WITH CEDILLA
            { '\u0160', "S" }, // LATIN CAPITAL LETTER S WITH CARON
            { '\u0161', "s" }, // LATIN SMALL LETTER S WITH CARON
            { '\u0162', "T" }, // LATIN CAPITAL LETTER T WITH CEDILLA
            { '\u0163', "t" }, // LATIN SMALL LETTER T WITH CEDILLA
            { '\u0164', "T" }, // LATIN CAPITAL LETTER T WITH CARON
            { '\u0165', "t" }, // LATIN SMALL LETTER T WITH CARON
            { '\u0166', "T" }, // LATIN CAPITAL LETTER T WITH STROKE -- no decomposition
            { '\u0167', "t" }, // LATIN SMALL LETTER T WITH STROKE -- no decomposition
            { '\u0168', "U" }, // LATIN CAPITAL LETTER U WITH TILDE
            { '\u0169', "u" }, // LATIN SMALL LETTER U WITH TILDE
            { '\u016A', "U" }, // LATIN CAPITAL LETTER U WITH MACRON
            { '\u016B', "u" }, // LATIN SMALL LETTER U WITH MACRON
            { '\u016C', "U" }, // LATIN CAPITAL LETTER U WITH BREVE
            { '\u016D', "u" }, // LATIN SMALL LETTER U WITH BREVE
            { '\u016E', "U" }, // LATIN CAPITAL LETTER U WITH RING ABOVE
            { '\u016F', "u" }, // LATIN SMALL LETTER U WITH RING ABOVE
            { '\u0170', "U" }, // LATIN CAPITAL LETTER U WITH DOUBLE ACUTE
            { '\u0171', "u" }, // LATIN SMALL LETTER U WITH DOUBLE ACUTE
            { '\u0172', "U" }, // LATIN CAPITAL LETTER U WITH OGONEK
            { '\u0173', "u" }, // LATIN SMALL LETTER U WITH OGONEK
            { '\u0174', "W" }, // LATIN CAPITAL LETTER W WITH CIRCUMFLEX
            { '\u0175', "w" }, // LATIN SMALL LETTER W WITH CIRCUMFLEX
            { '\u0176', "Y" }, // LATIN CAPITAL LETTER Y WITH CIRCUMFLEX
            { '\u0177', "y" }, // LATIN SMALL LETTER Y WITH CIRCUMFLEX
            { '\u0178', "Y" }, // LATIN CAPITAL LETTER Y WITH DIAERESIS
            { '\u0179', "Z" }, // LATIN CAPITAL LETTER Z WITH ACUTE
            { '\u017A', "z" }, // LATIN SMALL LETTER Z WITH ACUTE
            { '\u017B', "Z" }, // LATIN CAPITAL LETTER Z WITH DOT ABOVE
            { '\u017C', "z" }, // LATIN SMALL LETTER Z WITH DOT ABOVE
            { '\u017D', "Z" }, // LATIN CAPITAL LETTER Z WITH CARON
            { '\u017E', "z" }, // LATIN SMALL LETTER Z WITH CARON
            { '\u017F', "s" }, // LATIN SMALL LETTER LONG S    
            { '\u0180', "b" }, // LATIN SMALL LETTER B WITH STROKE -- no decomposition
            { '\u0181', "B" }, // LATIN CAPITAL LETTER B WITH HOOK -- no decomposition
            { '\u0182', "B" }, // LATIN CAPITAL LETTER B WITH TOPBAR -- no decomposition
            { '\u0183', "b" }, // LATIN SMALL LETTER B WITH TOPBAR -- no decomposition
            { '\u0184', "6" }, // LATIN CAPITAL LETTER TONE SIX -- no decomposition
            { '\u0185', "6" }, // LATIN SMALL LETTER TONE SIX -- no decomposition
            { '\u0186', "O" }, // LATIN CAPITAL LETTER OPEN O -- no decomposition
            { '\u0187', "C" }, // LATIN CAPITAL LETTER C WITH HOOK -- no decomposition
            { '\u0188', "c" }, // LATIN SMALL LETTER C WITH HOOK -- no decomposition
            { '\u0189', "D" }, // LATIN CAPITAL LETTER AFRICAN D -- no decomposition
            { '\u018A', "D" }, // LATIN CAPITAL LETTER D WITH HOOK -- no decomposition
            { '\u018B', "D" }, // LATIN CAPITAL LETTER D WITH TOPBAR -- no decomposition
            { '\u018C', "d" }, // LATIN SMALL LETTER D WITH TOPBAR -- no decomposition
            { '\u018D', "d" }, // LATIN SMALL LETTER TURNED DELTA -- no decomposition
            { '\u018E', "E" }, // LATIN CAPITAL LETTER REVERSED E -- no decomposition
            { '\u018F', "E" }, // LATIN CAPITAL LETTER SCHWA -- no decomposition
            { '\u0190', "E" }, // LATIN CAPITAL LETTER OPEN E -- no decomposition
            { '\u0191', "F" }, // LATIN CAPITAL LETTER F WITH HOOK -- no decomposition
            { '\u0192', "f" }, // LATIN SMALL LETTER F WITH HOOK -- no decomposition
            { '\u0193', "G" }, // LATIN CAPITAL LETTER G WITH HOOK -- no decomposition
            { '\u0194', "G" }, // LATIN CAPITAL LETTER GAMMA -- no decomposition
            { '\u0195', "hv" }, // LATIN SMALL LETTER HV -- no decomposition
            { '\u0196', "I" }, // LATIN CAPITAL LETTER IOTA -- no decomposition
            { '\u0197', "I" }, // LATIN CAPITAL LETTER I WITH STROKE -- no decomposition
            { '\u0198', "K" }, // LATIN CAPITAL LETTER K WITH HOOK -- no decomposition
            { '\u0199', "k" }, // LATIN SMALL LETTER K WITH HOOK -- no decomposition
            { '\u019A', "l" }, // LATIN SMALL LETTER L WITH BAR -- no decomposition
            { '\u019B', "l" }, // LATIN SMALL LETTER LAMBDA WITH STROKE -- no decomposition
            { '\u019C', "M" }, // LATIN CAPITAL LETTER TURNED M -- no decomposition
            { '\u019D', "N" }, // LATIN CAPITAL LETTER N WITH LEFT HOOK -- no decomposition
            { '\u019E', "n" }, // LATIN SMALL LETTER N WITH LONG RIGHT LEG -- no decomposition
            { '\u019F', "O" }, // LATIN CAPITAL LETTER O WITH MIDDLE TILDE -- no decomposition
            { '\u01A0', "O" }, // LATIN CAPITAL LETTER O WITH HORN
            { '\u01A1', "o" }, // LATIN SMALL LETTER O WITH HORN
            { '\u01A2', "OI" }, // LATIN CAPITAL LETTER OI -- no decomposition
            { '\u01A3', "oi" }, // LATIN SMALL LETTER OI -- no decomposition
            { '\u01A4', "P" }, // LATIN CAPITAL LETTER P WITH HOOK -- no decomposition
            { '\u01A5', "p" }, // LATIN SMALL LETTER P WITH HOOK -- no decomposition
            { '\u01A6', "YR" }, // LATIN LETTER YR -- no decomposition
            { '\u01A7', "2" }, // LATIN CAPITAL LETTER TONE TWO -- no decomposition
            { '\u01A8', "2" }, // LATIN SMALL LETTER TONE TWO -- no decomposition
            { '\u01A9', "S" }, // LATIN CAPITAL LETTER ESH -- no decomposition
            { '\u01AA', "s" }, // LATIN LETTER REVERSED ESH LOOP -- no decomposition
            { '\u01AB', "t" }, // LATIN SMALL LETTER T WITH PALATAL HOOK -- no decomposition
            { '\u01AC', "T" }, // LATIN CAPITAL LETTER T WITH HOOK -- no decomposition
            { '\u01AD', "t" }, // LATIN SMALL LETTER T WITH HOOK -- no decomposition
            { '\u01AE', "T" }, // LATIN CAPITAL LETTER T WITH RETROFLEX HOOK -- no decomposition
            { '\u01AF', "U" }, // LATIN CAPITAL LETTER U WITH HORN
            { '\u01B0', "u" }, // LATIN SMALL LETTER U WITH HORN
            { '\u01B1', "u" }, // LATIN CAPITAL LETTER UPSILON -- no decomposition
            { '\u01B2', "V" }, // LATIN CAPITAL LETTER V WITH HOOK -- no decomposition
            { '\u01B3', "Y" }, // LATIN CAPITAL LETTER Y WITH HOOK -- no decomposition
            { '\u01B4', "y" }, // LATIN SMALL LETTER Y WITH HOOK -- no decomposition
            { '\u01B5', "Z" }, // LATIN CAPITAL LETTER Z WITH STROKE -- no decomposition
            { '\u01B6', "z" }, // LATIN SMALL LETTER Z WITH STROKE -- no decomposition
            { '\u01B7', "Z" }, // LATIN CAPITAL LETTER EZH -- no decomposition
            { '\u01B8', "Z" }, // LATIN CAPITAL LETTER EZH REVERSED -- no decomposition
            { '\u01B9', "Z" }, // LATIN SMALL LETTER EZH REVERSED -- no decomposition
            { '\u01BA', "z" }, // LATIN SMALL LETTER EZH WITH TAIL -- no decomposition
            { '\u01BB', "2" }, // LATIN LETTER TWO WITH STROKE -- no decomposition
            { '\u01BC', "5" }, // LATIN CAPITAL LETTER TONE FIVE -- no decomposition
            { '\u01BD', "5" }, // LATIN SMALL LETTER TONE FIVE -- no decomposition
            { '\u01BF', "w" }, // LATIN LETTER WYNN -- no decomposition
            { '\u01C0', "!" }, // LATIN LETTER DENTAL CLICK -- no decomposition
            { '\u01C1', "!" }, // LATIN LETTER LATERAL CLICK -- no decomposition
            { '\u01C2', "!" }, // LATIN LETTER ALVEOLAR CLICK -- no decomposition
            { '\u01C3', "!" }, // LATIN LETTER RETROFLEX CLICK -- no decomposition
            { '\u01C4', "DZ" }, // LATIN CAPITAL LETTER DZ WITH CARON
            { '\u01C5', "DZ" }, // LATIN CAPITAL LETTER D WITH SMALL LETTER Z WITH CARON
            { '\u01C6', "d" }, // LATIN SMALL LETTER DZ WITH CARON
            { '\u01C7', "Lj" }, // LATIN CAPITAL LETTER LJ
            { '\u01C8', "Lj" }, // LATIN CAPITAL LETTER L WITH SMALL LETTER J
            { '\u01C9', "lj" }, // LATIN SMALL LETTER LJ
            { '\u01CA', "NJ" }, // LATIN CAPITAL LETTER NJ
            { '\u01CB', "NJ" }, // LATIN CAPITAL LETTER N WITH SMALL LETTER J
            { '\u01CC', "nj" }, // LATIN SMALL LETTER NJ
            { '\u01CD', "A" }, // LATIN CAPITAL LETTER A WITH CARON
            { '\u01CE', "a" }, // LATIN SMALL LETTER A WITH CARON
            { '\u01CF', "I" }, // LATIN CAPITAL LETTER I WITH CARON
            { '\u01D0', "i" }, // LATIN SMALL LETTER I WITH CARON
            { '\u01D1', "O" }, // LATIN CAPITAL LETTER O WITH CARON
            { '\u01D2', "o" }, // LATIN SMALL LETTER O WITH CARON
            { '\u01D3', "U" }, // LATIN CAPITAL LETTER U WITH CARON
            { '\u01D4', "u" }, // LATIN SMALL LETTER U WITH CARON
            { '\u01D5', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS AND MACRON
            { '\u01D6', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS AND MACRON
            { '\u01D7', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS AND ACUTE
            { '\u01D8', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS AND ACUTE
            { '\u01D9', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS AND CARON
            { '\u01DA', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS AND CARON
            { '\u01DB', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS AND GRAVE
            { '\u01DC', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS AND GRAVE
            { '\u01DD', "e" }, // LATIN SMALL LETTER TURNED E -- no decomposition
            { '\u01DE', "A" }, // LATIN CAPITAL LETTER A WITH DIAERESIS AND MACRON
            { '\u01DF', "a" }, // LATIN SMALL LETTER A WITH DIAERESIS AND MACRON
            { '\u01E0', "A" }, // LATIN CAPITAL LETTER A WITH DOT ABOVE AND MACRON
            { '\u01E1', "a" }, // LATIN SMALL LETTER A WITH DOT ABOVE AND MACRON
            { '\u01E2', "AE" },// LATIN CAPITAL LETTER AE WITH MACRON
            { '\u01E3', "ae" },// LATIN SMALL LETTER AE WITH MACRON
            { '\u01E4', "G" }, // LATIN CAPITAL LETTER G WITH STROKE -- no decomposition
            { '\u01E5', "g" }, // LATIN SMALL LETTER G WITH STROKE -- no decomposition
            { '\u01E6', "G" }, // LATIN CAPITAL LETTER G WITH CARON
            { '\u01E7', "g" }, // LATIN SMALL LETTER G WITH CARON
            { '\u01E8', "K" }, // LATIN CAPITAL LETTER K WITH CARON
            { '\u01E9', "k" }, // LATIN SMALL LETTER K WITH CARON
            { '\u01EA', "O" }, // LATIN CAPITAL LETTER O WITH OGONEK
            { '\u01EB', "o" }, // LATIN SMALL LETTER O WITH OGONEK
            { '\u01EC', "O" }, // LATIN CAPITAL LETTER O WITH OGONEK AND MACRON
            { '\u01ED', "o" }, // LATIN SMALL LETTER O WITH OGONEK AND MACRON
            { '\u01EE', "Z" }, // LATIN CAPITAL LETTER EZH WITH CARON
            { '\u01EF', "Z" }, // LATIN SMALL LETTER EZH WITH CARON
            { '\u01F0', "j" }, // LATIN SMALL LETTER J WITH CARON
            { '\u01F1', "DZ" }, // LATIN CAPITAL LETTER DZ
            { '\u01F2', "DZ" }, // LATIN CAPITAL LETTER D WITH SMALL LETTER Z
            { '\u01F3', "dz" }, // LATIN SMALL LETTER DZ
            { '\u01F4', "G" }, // LATIN CAPITAL LETTER G WITH ACUTE
            { '\u01F5', "g" }, // LATIN SMALL LETTER G WITH ACUTE
            { '\u01F6', "hv" }, // LATIN CAPITAL LETTER HWAIR -- no decomposition
            { '\u01F7', "w" }, // LATIN CAPITAL LETTER WYNN -- no decomposition
            { '\u01F8', "N" }, // LATIN CAPITAL LETTER N WITH GRAVE
            { '\u01F9', "n" }, // LATIN SMALL LETTER N WITH GRAVE
            { '\u01FA', "A" }, // LATIN CAPITAL LETTER A WITH RING ABOVE AND ACUTE
            { '\u01FB', "a" }, // LATIN SMALL LETTER A WITH RING ABOVE AND ACUTE
            { '\u01FC', "AE" }, // LATIN CAPITAL LETTER AE WITH ACUTE
            { '\u01FD', "ae" }, // LATIN SMALL LETTER AE WITH ACUTE
            { '\u01FE', "O" }, // LATIN CAPITAL LETTER O WITH STROKE AND ACUTE
            { '\u01FF', "o" }, // LATIN SMALL LETTER O WITH STROKE AND ACUTE
            { '\u0200', "A" }, // LATIN CAPITAL LETTER A WITH DOUBLE GRAVE
            { '\u0201', "a" }, // LATIN SMALL LETTER A WITH DOUBLE GRAVE
            { '\u0202', "A" }, // LATIN CAPITAL LETTER A WITH INVERTED BREVE
            { '\u0203', "a" }, // LATIN SMALL LETTER A WITH INVERTED BREVE
            { '\u0204', "E" }, // LATIN CAPITAL LETTER E WITH DOUBLE GRAVE
            { '\u0205', "e" }, // LATIN SMALL LETTER E WITH DOUBLE GRAVE
            { '\u0206', "E" }, // LATIN CAPITAL LETTER E WITH INVERTED BREVE
            { '\u0207', "e" }, // LATIN SMALL LETTER E WITH INVERTED BREVE
            { '\u0208', "I" }, // LATIN CAPITAL LETTER I WITH DOUBLE GRAVE
            { '\u0209', "i" }, // LATIN SMALL LETTER I WITH DOUBLE GRAVE
            { '\u020A', "I" }, // LATIN CAPITAL LETTER I WITH INVERTED BREVE
            { '\u020B', "i" }, // LATIN SMALL LETTER I WITH INVERTED BREVE
            { '\u020C', "O" }, // LATIN CAPITAL LETTER O WITH DOUBLE GRAVE
            { '\u020D', "o" }, // LATIN SMALL LETTER O WITH DOUBLE GRAVE
            { '\u020E', "O" }, // LATIN CAPITAL LETTER O WITH INVERTED BREVE
            { '\u020F', "o" }, // LATIN SMALL LETTER O WITH INVERTED BREVE
            { '\u0210', "R" }, // LATIN CAPITAL LETTER R WITH DOUBLE GRAVE
            { '\u0211', "r" }, // LATIN SMALL LETTER R WITH DOUBLE GRAVE
            { '\u0212', "R" }, // LATIN CAPITAL LETTER R WITH INVERTED BREVE
            { '\u0213', "r" }, // LATIN SMALL LETTER R WITH INVERTED BREVE
            { '\u0214', "U" }, // LATIN CAPITAL LETTER U WITH DOUBLE GRAVE
            { '\u0215', "u" }, // LATIN SMALL LETTER U WITH DOUBLE GRAVE
            { '\u0216', "U" }, // LATIN CAPITAL LETTER U WITH INVERTED BREVE
            { '\u0217', "u" }, // LATIN SMALL LETTER U WITH INVERTED BREVE
            { '\u0218', "S" }, // LATIN CAPITAL LETTER S WITH COMMA BELOW
            { '\u0219', "s" }, // LATIN SMALL LETTER S WITH COMMA BELOW
            { '\u021A', "T" }, // LATIN CAPITAL LETTER T WITH COMMA BELOW
            { '\u021B', "t" }, // LATIN SMALL LETTER T WITH COMMA BELOW
            { '\u021C', "Z" }, // LATIN CAPITAL LETTER YOGH -- no decomposition
            { '\u021D', "z" }, // LATIN SMALL LETTER YOGH -- no decomposition
            { '\u021E', "H" }, // LATIN CAPITAL LETTER H WITH CARON
            { '\u021F', "h" }, // LATIN SMALL LETTER H WITH CARON
            { '\u0220', "N" }, // LATIN CAPITAL LETTER N WITH LONG RIGHT LEG -- no decomposition
            { '\u0221', "d" }, // LATIN SMALL LETTER D WITH CURL -- no decomposition
            { '\u0222', "OU" }, // LATIN CAPITAL LETTER OU -- no decomposition
            { '\u0223', "ou" }, // LATIN SMALL LETTER OU -- no decomposition
            { '\u0224', "Z" }, // LATIN CAPITAL LETTER Z WITH HOOK -- no decomposition
            { '\u0225', "z" }, // LATIN SMALL LETTER Z WITH HOOK -- no decomposition
            { '\u0226', "A" }, // LATIN CAPITAL LETTER A WITH DOT ABOVE
            { '\u0227', "a" }, // LATIN SMALL LETTER A WITH DOT ABOVE
            { '\u0228', "E" }, // LATIN CAPITAL LETTER E WITH CEDILLA
            { '\u0229', "e" }, // LATIN SMALL LETTER E WITH CEDILLA
            { '\u022A', "O" }, // LATIN CAPITAL LETTER O WITH DIAERESIS AND MACRON
            { '\u022B', "o" }, // LATIN SMALL LETTER O WITH DIAERESIS AND MACRON
            { '\u022C', "O" }, // LATIN CAPITAL LETTER O WITH TILDE AND MACRON
            { '\u022D', "o" }, // LATIN SMALL LETTER O WITH TILDE AND MACRON
            { '\u022E', "O" }, // LATIN CAPITAL LETTER O WITH DOT ABOVE
            { '\u022F', "o" }, // LATIN SMALL LETTER O WITH DOT ABOVE
            { '\u0230', "O" }, // LATIN CAPITAL LETTER O WITH DOT ABOVE AND MACRON
            { '\u0231', "o" }, // LATIN SMALL LETTER O WITH DOT ABOVE AND MACRON
            { '\u0232', "Y" }, // LATIN CAPITAL LETTER Y WITH MACRON
            { '\u0233', "y" }, // LATIN SMALL LETTER Y WITH MACRON
            { '\u0234', "l" }, // LATIN SMALL LETTER L WITH CURL -- no decomposition
            { '\u0235', "n" }, // LATIN SMALL LETTER N WITH CURL -- no decomposition
            { '\u0236', "t" }, // LATIN SMALL LETTER T WITH CURL -- no decomposition
            { '\u0250', "a" }, // LATIN SMALL LETTER TURNED A -- no decomposition
            { '\u0251', "a" }, // LATIN SMALL LETTER ALPHA -- no decomposition
            { '\u0252', "a" }, // LATIN SMALL LETTER TURNED ALPHA -- no decomposition
            { '\u0253', "b" }, // LATIN SMALL LETTER B WITH HOOK -- no decomposition
            { '\u0254', "o" }, // LATIN SMALL LETTER OPEN O -- no decomposition
            { '\u0255', "c" }, // LATIN SMALL LETTER C WITH CURL -- no decomposition
            { '\u0256', "d" }, // LATIN SMALL LETTER D WITH TAIL -- no decomposition
            { '\u0257', "d" }, // LATIN SMALL LETTER D WITH HOOK -- no decomposition
            { '\u0258', "e" }, // LATIN SMALL LETTER REVERSED E -- no decomposition
            { '\u0259', "e" }, // LATIN SMALL LETTER SCHWA -- no decomposition
            { '\u025A', "e" }, // LATIN SMALL LETTER SCHWA WITH HOOK -- no decomposition
            { '\u025B', "e" }, // LATIN SMALL LETTER OPEN E -- no decomposition
            { '\u025C', "e" }, // LATIN SMALL LETTER REVERSED OPEN E -- no decomposition
            { '\u025D', "e" }, // LATIN SMALL LETTER REVERSED OPEN E WITH HOOK -- no decomposition
            { '\u025E', "e" }, // LATIN SMALL LETTER CLOSED REVERSED OPEN E -- no decomposition
            { '\u025F', "j" }, // LATIN SMALL LETTER DOTLESS J WITH STROKE -- no decomposition
            { '\u0260', "g" }, // LATIN SMALL LETTER G WITH HOOK -- no decomposition
            { '\u0261', "g" }, // LATIN SMALL LETTER SCRIPT G -- no decomposition
            { '\u0262', "G" }, // LATIN LETTER SMALL CAPITAL G -- no decomposition
            { '\u0263', "g" }, // LATIN SMALL LETTER GAMMA -- no decomposition
            { '\u0264', "y" }, // LATIN SMALL LETTER RAMS HORN -- no decomposition
            { '\u0265', "h" }, // LATIN SMALL LETTER TURNED H -- no decomposition
            { '\u0266', "h" }, // LATIN SMALL LETTER H WITH HOOK -- no decomposition
            { '\u0267', "h" }, // LATIN SMALL LETTER HENG WITH HOOK -- no decomposition
            { '\u0268', "i" }, // LATIN SMALL LETTER I WITH STROKE -- no decomposition
            { '\u0269', "i" }, // LATIN SMALL LETTER IOTA -- no decomposition
            { '\u026A', "I" }, // LATIN LETTER SMALL CAPITAL I -- no decomposition
            { '\u026B', "l" }, // LATIN SMALL LETTER L WITH MIDDLE TILDE -- no decomposition
            { '\u026C', "l" }, // LATIN SMALL LETTER L WITH BELT -- no decomposition
            { '\u026D', "l" }, // LATIN SMALL LETTER L WITH RETROFLEX HOOK -- no decomposition
            { '\u026E', "lz" }, // LATIN SMALL LETTER LEZH -- no decomposition
            { '\u026F', "m" }, // LATIN SMALL LETTER TURNED M -- no decomposition
            { '\u0270', "m" }, // LATIN SMALL LETTER TURNED M WITH LONG LEG -- no decomposition
            { '\u0271', "m" }, // LATIN SMALL LETTER M WITH HOOK -- no decomposition
            { '\u0272', "n" }, // LATIN SMALL LETTER N WITH LEFT HOOK -- no decomposition
            { '\u0273', "n" }, // LATIN SMALL LETTER N WITH RETROFLEX HOOK -- no decomposition
            { '\u0274', "N" }, // LATIN LETTER SMALL CAPITAL N -- no decomposition
            { '\u0275', "o" }, // LATIN SMALL LETTER BARRED O -- no decomposition
            { '\u0276', "OE" }, // LATIN LETTER SMALL CAPITAL OE -- no decomposition
            { '\u0277', "o" }, // LATIN SMALL LETTER CLOSED OMEGA -- no decomposition
            { '\u0278', "ph" }, // LATIN SMALL LETTER PHI -- no decomposition
            { '\u0279', "r" }, // LATIN SMALL LETTER TURNED R -- no decomposition
            { '\u027A', "r" }, // LATIN SMALL LETTER TURNED R WITH LONG LEG -- no decomposition
            { '\u027B', "r" }, // LATIN SMALL LETTER TURNED R WITH HOOK -- no decomposition
            { '\u027C', "r" }, // LATIN SMALL LETTER R WITH LONG LEG -- no decomposition
            { '\u027D', "r" }, // LATIN SMALL LETTER R WITH TAIL -- no decomposition
            { '\u027E', "r" }, // LATIN SMALL LETTER R WITH FISHHOOK -- no decomposition
            { '\u027F', "r" }, // LATIN SMALL LETTER REVERSED R WITH FISHHOOK -- no decomposition
            { '\u0280', "R" }, // LATIN LETTER SMALL CAPITAL R -- no decomposition
            { '\u0281', "r" }, // LATIN LETTER SMALL CAPITAL INVERTED R -- no decomposition
            { '\u0282', "s" }, // LATIN SMALL LETTER S WITH HOOK -- no decomposition
            { '\u0283', "s" }, // LATIN SMALL LETTER ESH -- no decomposition
            { '\u0284', "j" }, // LATIN SMALL LETTER DOTLESS J WITH STROKE AND HOOK -- no decomposition
            { '\u0285', "s" }, // LATIN SMALL LETTER SQUAT REVERSED ESH -- no decomposition
            { '\u0286', "s" }, // LATIN SMALL LETTER ESH WITH CURL -- no decomposition
            { '\u0287', "y" }, // LATIN SMALL LETTER TURNED T -- no decomposition
            { '\u0288', "t" }, // LATIN SMALL LETTER T WITH RETROFLEX HOOK -- no decomposition
            { '\u0289', "u" }, // LATIN SMALL LETTER U BAR -- no decomposition
            { '\u028A', "u" }, // LATIN SMALL LETTER UPSILON -- no decomposition
            { '\u028B', "u" }, // LATIN SMALL LETTER V WITH HOOK -- no decomposition
            { '\u028C', "v" }, // LATIN SMALL LETTER TURNED V -- no decomposition
            { '\u028D', "w" }, // LATIN SMALL LETTER TURNED W -- no decomposition
            { '\u028E', "y" }, // LATIN SMALL LETTER TURNED Y -- no decomposition
            { '\u028F', "Y" }, // LATIN LETTER SMALL CAPITAL Y -- no decomposition
            { '\u0290', "z" }, // LATIN SMALL LETTER Z WITH RETROFLEX HOOK -- no decomposition
            { '\u0291', "z" }, // LATIN SMALL LETTER Z WITH CURL -- no decomposition
            { '\u0292', "z" }, // LATIN SMALL LETTER EZH -- no decomposition
            { '\u0293', "z" }, // LATIN SMALL LETTER EZH WITH CURL -- no decomposition
            { '\u0294', "'" }, // LATIN LETTER GLOTTAL STOP -- no decomposition
            { '\u0295', "'" }, // LATIN LETTER PHARYNGEAL VOICED FRICATIVE -- no decomposition
            { '\u0296', "'" }, // LATIN LETTER INVERTED GLOTTAL STOP -- no decomposition
            { '\u0297', "C" }, // LATIN LETTER STRETCHED C -- no decomposition
            { '\u0299', "B" }, // LATIN LETTER SMALL CAPITAL B -- no decomposition
            { '\u029A', "e" }, // LATIN SMALL LETTER CLOSED OPEN E -- no decomposition
            { '\u029B', "G" }, // LATIN LETTER SMALL CAPITAL G WITH HOOK -- no decomposition
            { '\u029C', "H" }, // LATIN LETTER SMALL CAPITAL H -- no decomposition
            { '\u029D', "j" }, // LATIN SMALL LETTER J WITH CROSSED-TAIL -- no decomposition
            { '\u029E', "k" }, // LATIN SMALL LETTER TURNED K -- no decomposition
            { '\u029F', "L" }, // LATIN LETTER SMALL CAPITAL L -- no decomposition
            { '\u02A0', "q" }, // LATIN SMALL LETTER Q WITH HOOK -- no decomposition
            { '\u02A1', "'" }, // LATIN LETTER GLOTTAL STOP WITH STROKE -- no decomposition
            { '\u02A2', "'" }, // LATIN LETTER REVERSED GLOTTAL STOP WITH STROKE -- no decomposition
            { '\u02A3', "dz" }, // LATIN SMALL LETTER DZ DIGRAPH -- no decomposition
            { '\u02A4', "dz" }, // LATIN SMALL LETTER DEZH DIGRAPH -- no decomposition
            { '\u02A5', "dz" }, // LATIN SMALL LETTER DZ DIGRAPH WITH CURL -- no decomposition
            { '\u02A6', "ts" }, // LATIN SMALL LETTER TS DIGRAPH -- no decomposition
            { '\u02A7', "ts" }, // LATIN SMALL LETTER TESH DIGRAPH -- no decomposition
            { '\u02A8', string.Empty }, // LATIN SMALL LETTER TC DIGRAPH WITH CURL -- no decomposition
            { '\u02A9', "fn" }, // LATIN SMALL LETTER FENG DIGRAPH -- no decomposition
            { '\u02AA', "ls" }, // LATIN SMALL LETTER LS DIGRAPH -- no decomposition
            { '\u02AB', "lz" }, // LATIN SMALL LETTER LZ DIGRAPH -- no decomposition
            { '\u02AC', "w" }, // LATIN LETTER BILABIAL PERCUSSIVE -- no decomposition
            { '\u02AD', "t" }, // LATIN LETTER BIDENTAL PERCUSSIVE -- no decomposition
            { '\u02AE', "h" }, // LATIN SMALL LETTER TURNED H WITH FISHHOOK -- no decomposition
            { '\u02AF', "h" }, // LATIN SMALL LETTER TURNED H WITH FISHHOOK AND TAIL -- no decomposition
            { '\u02B0', "h" }, // MODIFIER LETTER SMALL H
            { '\u02B1', "h" }, // MODIFIER LETTER SMALL H WITH HOOK
            { '\u02B2', "j" }, // MODIFIER LETTER SMALL J
            { '\u02B3', "r" }, // MODIFIER LETTER SMALL R
            { '\u02B4', "r" }, // MODIFIER LETTER SMALL TURNED R
            { '\u02B5', "r" }, // MODIFIER LETTER SMALL TURNED R WITH HOOK
            { '\u02B6', "R" }, // MODIFIER LETTER SMALL CAPITAL INVERTED R
            { '\u02B7', "w" }, // MODIFIER LETTER SMALL W
            { '\u02B8', "y" }, // MODIFIER LETTER SMALL Y
            { '\u02E1', "l" }, // MODIFIER LETTER SMALL L
            { '\u02E2', "s" }, // MODIFIER LETTER SMALL S
            { '\u02E3', "x" }, // MODIFIER LETTER SMALL X
            { '\u02E4', "'" }, // MODIFIER LETTER SMALL REVERSED GLOTTAL STOP
            { '\u1D00', "A" }, // LATIN LETTER SMALL CAPITAL A -- no decomposition
            { '\u1D01', "AE" }, // LATIN LETTER SMALL CAPITAL AE -- no decomposition
            { '\u1D02', "ae" }, // LATIN SMALL LETTER TURNED AE -- no decomposition
            { '\u1D03', "B" }, // LATIN LETTER SMALL CAPITAL BARRED B -- no decomposition
            { '\u1D04', "C" }, // LATIN LETTER SMALL CAPITAL C -- no decomposition
            { '\u1D05', "D" }, // LATIN LETTER SMALL CAPITAL D -- no decomposition
            { '\u1D06', "TH" }, // LATIN LETTER SMALL CAPITAL ETH -- no decomposition
            { '\u1D07', "E" }, // LATIN LETTER SMALL CAPITAL E -- no decomposition
            { '\u1D08', "e" }, // LATIN SMALL LETTER TURNED OPEN E -- no decomposition
            { '\u1D09', "i" }, // LATIN SMALL LETTER TURNED I -- no decomposition
            { '\u1D0A', "J" }, // LATIN LETTER SMALL CAPITAL J -- no decomposition
            { '\u1D0B', "K" }, // LATIN LETTER SMALL CAPITAL K -- no decomposition
            { '\u1D0C', "L" }, // LATIN LETTER SMALL CAPITAL L WITH STROKE -- no decomposition
            { '\u1D0D', "M" }, // LATIN LETTER SMALL CAPITAL M -- no decomposition
            { '\u1D0E', "N" }, // LATIN LETTER SMALL CAPITAL REVERSED N -- no decomposition
            { '\u1D0F', "O" }, // LATIN LETTER SMALL CAPITAL O -- no decomposition
            { '\u1D10', "O" }, // LATIN LETTER SMALL CAPITAL OPEN O -- no decomposition
            { '\u1D11', "o" }, // LATIN SMALL LETTER SIDEWAYS O -- no decomposition
            { '\u1D12', "o" }, // LATIN SMALL LETTER SIDEWAYS OPEN O -- no decomposition
            { '\u1D13', "o" }, // LATIN SMALL LETTER SIDEWAYS O WITH STROKE -- no decomposition
            { '\u1D14', "oe" }, // LATIN SMALL LETTER TURNED OE -- no decomposition
            { '\u1D15', "ou" }, // LATIN LETTER SMALL CAPITAL OU -- no decomposition
            { '\u1D16', "o" }, // LATIN SMALL LETTER TOP HALF O -- no decomposition
            { '\u1D17', "o" }, // LATIN SMALL LETTER BOTTOM HALF O -- no decomposition
            { '\u1D18', "P" }, // LATIN LETTER SMALL CAPITAL P -- no decomposition
            { '\u1D19', "R" }, // LATIN LETTER SMALL CAPITAL REVERSED R -- no decomposition
            { '\u1D1A', "R" }, // LATIN LETTER SMALL CAPITAL TURNED R -- no decomposition
            { '\u1D1B', "T" }, // LATIN LETTER SMALL CAPITAL T -- no decomposition
            { '\u1D1C', "U" }, // LATIN LETTER SMALL CAPITAL U -- no decomposition
            { '\u1D1D', "u" }, // LATIN SMALL LETTER SIDEWAYS U -- no decomposition
            { '\u1D1E', "u" }, // LATIN SMALL LETTER SIDEWAYS DIAERESIZED U -- no decomposition
            { '\u1D1F', "m" }, // LATIN SMALL LETTER SIDEWAYS TURNED M -- no decomposition
            { '\u1D20', "V" }, // LATIN LETTER SMALL CAPITAL V -- no decomposition
            { '\u1D21', "W" }, // LATIN LETTER SMALL CAPITAL W -- no decomposition
            { '\u1D22', "Z" }, // LATIN LETTER SMALL CAPITAL Z -- no decomposition
            { '\u1D23', "EZH" }, // LATIN LETTER SMALL CAPITAL EZH -- no decomposition
            { '\u1D24', "'" }, // LATIN LETTER VOICED LARYNGEAL SPIRANT -- no decomposition
            { '\u1D25', "L" }, // LATIN LETTER AIN -- no decomposition
            { '\u1D2C', "A" }, // MODIFIER LETTER CAPITAL A
            { '\u1D2D', "AE" }, // MODIFIER LETTER CAPITAL AE
            { '\u1D2E', "B" }, // MODIFIER LETTER CAPITAL B
            { '\u1D2F', "B" }, // MODIFIER LETTER CAPITAL BARRED B -- no decomposition
            { '\u1D30', "D" }, // MODIFIER LETTER CAPITAL D
            { '\u1D31', "E" }, // MODIFIER LETTER CAPITAL E
            { '\u1D32', "E" }, // MODIFIER LETTER CAPITAL REVERSED E
            { '\u1D33', "G" }, // MODIFIER LETTER CAPITAL G
            { '\u1D34', "H" }, // MODIFIER LETTER CAPITAL H
            { '\u1D35', "I" }, // MODIFIER LETTER CAPITAL I
            { '\u1D36', "J" }, // MODIFIER LETTER CAPITAL J
            { '\u1D37', "K" }, // MODIFIER LETTER CAPITAL K
            { '\u1D38', "L" }, // MODIFIER LETTER CAPITAL L
            { '\u1D39', "M" }, // MODIFIER LETTER CAPITAL M
            { '\u1D3A', "N" }, // MODIFIER LETTER CAPITAL N
            { '\u1D3B', "N" }, // MODIFIER LETTER CAPITAL REVERSED N -- no decomposition
            { '\u1D3C', "O" }, // MODIFIER LETTER CAPITAL O
            { '\u1D3D', "OU" }, // MODIFIER LETTER CAPITAL OU
            { '\u1D3E', "P" }, // MODIFIER LETTER CAPITAL P
            { '\u1D3F', "R" }, // MODIFIER LETTER CAPITAL R
            { '\u1D40', "T" }, // MODIFIER LETTER CAPITAL T
            { '\u1D41', "U" }, // MODIFIER LETTER CAPITAL U
            { '\u1D42', "W" }, // MODIFIER LETTER CAPITAL W
            { '\u1D43', "a" }, // MODIFIER LETTER SMALL A
            { '\u1D44', "a" }, // MODIFIER LETTER SMALL TURNED A
            { '\u1D46', "ae" }, // MODIFIER LETTER SMALL TURNED AE
            { '\u1D47', "b" }, // MODIFIER LETTER SMALL B
            { '\u1D48', "d" }, // MODIFIER LETTER SMALL D
            { '\u1D49', "e" }, // MODIFIER LETTER SMALL E
            { '\u1D4A', "e" }, // MODIFIER LETTER SMALL SCHWA
            { '\u1D4B', "e" }, // MODIFIER LETTER SMALL OPEN E
            { '\u1D4C', "e" }, // MODIFIER LETTER SMALL TURNED OPEN E
            { '\u1D4D', "g" }, // MODIFIER LETTER SMALL G
            { '\u1D4E', "i" }, // MODIFIER LETTER SMALL TURNED I -- no decomposition
            { '\u1D4F', "k" }, // MODIFIER LETTER SMALL K
            { '\u1D50', "m" }, // MODIFIER LETTER SMALL M
            { '\u1D51', "g" }, // MODIFIER LETTER SMALL ENG
            { '\u1D52', "o" }, // MODIFIER LETTER SMALL O
            { '\u1D53', "o" }, // MODIFIER LETTER SMALL OPEN O
            { '\u1D54', "o" }, // MODIFIER LETTER SMALL TOP HALF O
            { '\u1D55', "o" }, // MODIFIER LETTER SMALL BOTTOM HALF O
            { '\u1D56', "p" }, // MODIFIER LETTER SMALL P
            { '\u1D57', "t" }, // MODIFIER LETTER SMALL T
            { '\u1D58', "u" }, // MODIFIER LETTER SMALL U
            { '\u1D59', "u" }, // MODIFIER LETTER SMALL SIDEWAYS U
            { '\u1D5A', "m" }, // MODIFIER LETTER SMALL TURNED M
            { '\u1D5B', "v" }, // MODIFIER LETTER SMALL V
            { '\u1D62', "i" }, // LATIN SUBSCRIPT SMALL LETTER I
            { '\u1D63', "r" }, // LATIN SUBSCRIPT SMALL LETTER R
            { '\u1D64', "u" }, // LATIN SUBSCRIPT SMALL LETTER U
            { '\u1D65', "v" }, // LATIN SUBSCRIPT SMALL LETTER V
            { '\u1D6B', "ue" }, // LATIN SMALL LETTER UE -- no decomposition
            { '\u1E00', "A" }, // LATIN CAPITAL LETTER A WITH RING BELOW
            { '\u1E01', "a" }, // LATIN SMALL LETTER A WITH RING BELOW
            { '\u1E02', "B" }, // LATIN CAPITAL LETTER B WITH DOT ABOVE
            { '\u1E03', "b" }, // LATIN SMALL LETTER B WITH DOT ABOVE
            { '\u1E04', "B" }, // LATIN CAPITAL LETTER B WITH DOT BELOW
            { '\u1E05', "b" }, // LATIN SMALL LETTER B WITH DOT BELOW
            { '\u1E06', "B" }, // LATIN CAPITAL LETTER B WITH LINE BELOW
            { '\u1E07', "b" }, // LATIN SMALL LETTER B WITH LINE BELOW
            { '\u1E08', "C" }, // LATIN CAPITAL LETTER C WITH CEDILLA AND ACUTE
            { '\u1E09', "c" }, // LATIN SMALL LETTER C WITH CEDILLA AND ACUTE
            { '\u1E0A', "D" }, // LATIN CAPITAL LETTER D WITH DOT ABOVE
            { '\u1E0B', "d" }, // LATIN SMALL LETTER D WITH DOT ABOVE
            { '\u1E0C', "D" }, // LATIN CAPITAL LETTER D WITH DOT BELOW
            { '\u1E0D', "d" }, // LATIN SMALL LETTER D WITH DOT BELOW
            { '\u1E0E', "D" }, // LATIN CAPITAL LETTER D WITH LINE BELOW
            { '\u1E0F', "d" }, // LATIN SMALL LETTER D WITH LINE BELOW
            { '\u1E10', "D" }, // LATIN CAPITAL LETTER D WITH CEDILLA
            { '\u1E11', "d" }, // LATIN SMALL LETTER D WITH CEDILLA
            { '\u1E12', "D" }, // LATIN CAPITAL LETTER D WITH CIRCUMFLEX BELOW
            { '\u1E13', "d" }, // LATIN SMALL LETTER D WITH CIRCUMFLEX BELOW
            { '\u1E14', "E" }, // LATIN CAPITAL LETTER E WITH MACRON AND GRAVE
            { '\u1E15', "e" }, // LATIN SMALL LETTER E WITH MACRON AND GRAVE
            { '\u1E16', "E" }, // LATIN CAPITAL LETTER E WITH MACRON AND ACUTE
            { '\u1E17', "e" }, // LATIN SMALL LETTER E WITH MACRON AND ACUTE
            { '\u1E18', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX BELOW
            { '\u1E19', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX BELOW
            { '\u1E1A', "E" }, // LATIN CAPITAL LETTER E WITH TILDE BELOW
            { '\u1E1B', "e" }, // LATIN SMALL LETTER E WITH TILDE BELOW
            { '\u1E1C', "E" }, // LATIN CAPITAL LETTER E WITH CEDILLA AND BREVE
            { '\u1E1D', "e" }, // LATIN SMALL LETTER E WITH CEDILLA AND BREVE
            { '\u1E1E', "F" }, // LATIN CAPITAL LETTER F WITH DOT ABOVE
            { '\u1E1F', "f" }, // LATIN SMALL LETTER F WITH DOT ABOVE
            { '\u1E20', "G" }, // LATIN CAPITAL LETTER G WITH MACRON
            { '\u1E21', "g" }, // LATIN SMALL LETTER G WITH MACRON
            { '\u1E22', "H" }, // LATIN CAPITAL LETTER H WITH DOT ABOVE
            { '\u1E23', "h" }, // LATIN SMALL LETTER H WITH DOT ABOVE
            { '\u1E24', "H" }, // LATIN CAPITAL LETTER H WITH DOT BELOW
            { '\u1E25', "h" }, // LATIN SMALL LETTER H WITH DOT BELOW
            { '\u1E26', "H" }, // LATIN CAPITAL LETTER H WITH DIAERESIS
            { '\u1E27', "h" }, // LATIN SMALL LETTER H WITH DIAERESIS
            { '\u1E28', "H" }, // LATIN CAPITAL LETTER H WITH CEDILLA
            { '\u1E29', "h" }, // LATIN SMALL LETTER H WITH CEDILLA
            { '\u1E2A', "H" }, // LATIN CAPITAL LETTER H WITH BREVE BELOW
            { '\u1E2B', "h" }, // LATIN SMALL LETTER H WITH BREVE BELOW
            { '\u1E2C', "I" }, // LATIN CAPITAL LETTER I WITH TILDE BELOW
            { '\u1E2D', "i" }, // LATIN SMALL LETTER I WITH TILDE BELOW
            { '\u1E2E', "I" }, // LATIN CAPITAL LETTER I WITH DIAERESIS AND ACUTE
            { '\u1E2F', "i" }, // LATIN SMALL LETTER I WITH DIAERESIS AND ACUTE
            { '\u1E30', "K" }, // LATIN CAPITAL LETTER K WITH ACUTE
            { '\u1E31', "k" }, // LATIN SMALL LETTER K WITH ACUTE
            { '\u1E32', "K" }, // LATIN CAPITAL LETTER K WITH DOT BELOW
            { '\u1E33', "k" }, // LATIN SMALL LETTER K WITH DOT BELOW
            { '\u1E34', "K" }, // LATIN CAPITAL LETTER K WITH LINE BELOW
            { '\u1E35', "k" }, // LATIN SMALL LETTER K WITH LINE BELOW
            { '\u1E36', "L" }, // LATIN CAPITAL LETTER L WITH DOT BELOW
            { '\u1E37', "l" }, // LATIN SMALL LETTER L WITH DOT BELOW
            { '\u1E38', "L" }, // LATIN CAPITAL LETTER L WITH DOT BELOW AND MACRON
            { '\u1E39', "l" }, // LATIN SMALL LETTER L WITH DOT BELOW AND MACRON
            { '\u1E3A', "L" }, // LATIN CAPITAL LETTER L WITH LINE BELOW
            { '\u1E3B', "l" }, // LATIN SMALL LETTER L WITH LINE BELOW
            { '\u1E3C', "L" }, // LATIN CAPITAL LETTER L WITH CIRCUMFLEX BELOW
            { '\u1E3D', "l" }, // LATIN SMALL LETTER L WITH CIRCUMFLEX BELOW
            { '\u1E3E', "M" }, // LATIN CAPITAL LETTER M WITH ACUTE
            { '\u1E3F', "m" }, // LATIN SMALL LETTER M WITH ACUTE
            { '\u1E40', "M" }, // LATIN CAPITAL LETTER M WITH DOT ABOVE
            { '\u1E41', "m" }, // LATIN SMALL LETTER M WITH DOT ABOVE
            { '\u1E42', "M" }, // LATIN CAPITAL LETTER M WITH DOT BELOW
            { '\u1E43', "m" }, // LATIN SMALL LETTER M WITH DOT BELOW
            { '\u1E44', "N" }, // LATIN CAPITAL LETTER N WITH DOT ABOVE
            { '\u1E45', "n" }, // LATIN SMALL LETTER N WITH DOT ABOVE
            { '\u1E46', "N" }, // LATIN CAPITAL LETTER N WITH DOT BELOW
            { '\u1E47', "n" }, // LATIN SMALL LETTER N WITH DOT BELOW
            { '\u1E48', "N" }, // LATIN CAPITAL LETTER N WITH LINE BELOW
            { '\u1E49', "n" }, // LATIN SMALL LETTER N WITH LINE BELOW
            { '\u1E4A', "N" }, // LATIN CAPITAL LETTER N WITH CIRCUMFLEX BELOW
            { '\u1E4B', "n" }, // LATIN SMALL LETTER N WITH CIRCUMFLEX BELOW
            { '\u1E4C', "O" }, // LATIN CAPITAL LETTER O WITH TILDE AND ACUTE
            { '\u1E4D', "o" }, // LATIN SMALL LETTER O WITH TILDE AND ACUTE
            { '\u1E4E', "O" }, // LATIN CAPITAL LETTER O WITH TILDE AND DIAERESIS
            { '\u1E4F', "o" }, // LATIN SMALL LETTER O WITH TILDE AND DIAERESIS
            { '\u1E50', "O" }, // LATIN CAPITAL LETTER O WITH MACRON AND GRAVE
            { '\u1E51', "o" }, // LATIN SMALL LETTER O WITH MACRON AND GRAVE
            { '\u1E52', "O" }, // LATIN CAPITAL LETTER O WITH MACRON AND ACUTE
            { '\u1E53', "o" }, // LATIN SMALL LETTER O WITH MACRON AND ACUTE
            { '\u1E54', "P" }, // LATIN CAPITAL LETTER P WITH ACUTE
            { '\u1E55', "p" }, // LATIN SMALL LETTER P WITH ACUTE
            { '\u1E56', "P" }, // LATIN CAPITAL LETTER P WITH DOT ABOVE
            { '\u1E57', "p" }, // LATIN SMALL LETTER P WITH DOT ABOVE
            { '\u1E58', "R" }, // LATIN CAPITAL LETTER R WITH DOT ABOVE
            { '\u1E59', "r" }, // LATIN SMALL LETTER R WITH DOT ABOVE
            { '\u1E5A', "R" }, // LATIN CAPITAL LETTER R WITH DOT BELOW
            { '\u1E5B', "r" }, // LATIN SMALL LETTER R WITH DOT BELOW
            { '\u1E5C', "R" }, // LATIN CAPITAL LETTER R WITH DOT BELOW AND MACRON
            { '\u1E5D', "r" }, // LATIN SMALL LETTER R WITH DOT BELOW AND MACRON
            { '\u1E5E', "R" }, // LATIN CAPITAL LETTER R WITH LINE BELOW
            { '\u1E5F', "r" }, // LATIN SMALL LETTER R WITH LINE BELOW
            { '\u1E60', "S" }, // LATIN CAPITAL LETTER S WITH DOT ABOVE
            { '\u1E61', "s" }, // LATIN SMALL LETTER S WITH DOT ABOVE
            { '\u1E62', "S" }, // LATIN CAPITAL LETTER S WITH DOT BELOW
            { '\u1E63', "s" }, // LATIN SMALL LETTER S WITH DOT BELOW
            { '\u1E64', "S" }, // LATIN CAPITAL LETTER S WITH ACUTE AND DOT ABOVE
            { '\u1E65', "s" }, // LATIN SMALL LETTER S WITH ACUTE AND DOT ABOVE
            { '\u1E66', "S" }, // LATIN CAPITAL LETTER S WITH CARON AND DOT ABOVE
            { '\u1E67', "s" }, // LATIN SMALL LETTER S WITH CARON AND DOT ABOVE
            { '\u1E68', "S" }, // LATIN CAPITAL LETTER S WITH DOT BELOW AND DOT ABOVE
            { '\u1E69', "s" }, // LATIN SMALL LETTER S WITH DOT BELOW AND DOT ABOVE
            { '\u1E6A', "T" }, // LATIN CAPITAL LETTER T WITH DOT ABOVE
            { '\u1E6B', "t" }, // LATIN SMALL LETTER T WITH DOT ABOVE
            { '\u1E6C', "T" }, // LATIN CAPITAL LETTER T WITH DOT BELOW
            { '\u1E6D', "t" }, // LATIN SMALL LETTER T WITH DOT BELOW
            { '\u1E6E', "T" }, // LATIN CAPITAL LETTER T WITH LINE BELOW
            { '\u1E6F', "t" }, // LATIN SMALL LETTER T WITH LINE BELOW
            { '\u1E70', "T" }, // LATIN CAPITAL LETTER T WITH CIRCUMFLEX BELOW
            { '\u1E71', "t" }, // LATIN SMALL LETTER T WITH CIRCUMFLEX BELOW
            { '\u1E72', "U" }, // LATIN CAPITAL LETTER U WITH DIAERESIS BELOW
            { '\u1E73', "u" }, // LATIN SMALL LETTER U WITH DIAERESIS BELOW
            { '\u1E74', "U" }, // LATIN CAPITAL LETTER U WITH TILDE BELOW
            { '\u1E75', "u" }, // LATIN SMALL LETTER U WITH TILDE BELOW
            { '\u1E76', "U" }, // LATIN CAPITAL LETTER U WITH CIRCUMFLEX BELOW
            { '\u1E77', "u" }, // LATIN SMALL LETTER U WITH CIRCUMFLEX BELOW
            { '\u1E78', "U" }, // LATIN CAPITAL LETTER U WITH TILDE AND ACUTE
            { '\u1E79', "u" }, // LATIN SMALL LETTER U WITH TILDE AND ACUTE
            { '\u1E7A', "U" }, // LATIN CAPITAL LETTER U WITH MACRON AND DIAERESIS
            { '\u1E7B', "u" }, // LATIN SMALL LETTER U WITH MACRON AND DIAERESIS
            { '\u1E7C', "V" }, // LATIN CAPITAL LETTER V WITH TILDE
            { '\u1E7D', "v" }, // LATIN SMALL LETTER V WITH TILDE
            { '\u1E7E', "V" }, // LATIN CAPITAL LETTER V WITH DOT BELOW
            { '\u1E7F', "v" }, // LATIN SMALL LETTER V WITH DOT BELOW
            { '\u1E80', "W" }, // LATIN CAPITAL LETTER W WITH GRAVE
            { '\u1E81', "w" }, // LATIN SMALL LETTER W WITH GRAVE
            { '\u1E82', "W" }, // LATIN CAPITAL LETTER W WITH ACUTE
            { '\u1E83', "w" }, // LATIN SMALL LETTER W WITH ACUTE
            { '\u1E84', "W" }, // LATIN CAPITAL LETTER W WITH DIAERESIS
            { '\u1E85', "w" }, // LATIN SMALL LETTER W WITH DIAERESIS
            { '\u1E86', "W" }, // LATIN CAPITAL LETTER W WITH DOT ABOVE
            { '\u1E87', "w" }, // LATIN SMALL LETTER W WITH DOT ABOVE
            { '\u1E88', "W" }, // LATIN CAPITAL LETTER W WITH DOT BELOW
            { '\u1E89', "w" }, // LATIN SMALL LETTER W WITH DOT BELOW
            { '\u1E8A', "X" }, // LATIN CAPITAL LETTER X WITH DOT ABOVE
            { '\u1E8B', "x" }, // LATIN SMALL LETTER X WITH DOT ABOVE
            { '\u1E8C', "X" }, // LATIN CAPITAL LETTER X WITH DIAERESIS
            { '\u1E8D', "x" }, // LATIN SMALL LETTER X WITH DIAERESIS
            { '\u1E8E', "Y" }, // LATIN CAPITAL LETTER Y WITH DOT ABOVE
            { '\u1E8F', "y" }, // LATIN SMALL LETTER Y WITH DOT ABOVE
            { '\u1E90', "Z" }, // LATIN CAPITAL LETTER Z WITH CIRCUMFLEX
            { '\u1E91', "z" }, // LATIN SMALL LETTER Z WITH CIRCUMFLEX
            { '\u1E92', "Z" }, // LATIN CAPITAL LETTER Z WITH DOT BELOW
            { '\u1E93', "z" }, // LATIN SMALL LETTER Z WITH DOT BELOW
            { '\u1E94', "Z" }, // LATIN CAPITAL LETTER Z WITH LINE BELOW
            { '\u1E95', "z" }, // LATIN SMALL LETTER Z WITH LINE BELOW
            { '\u1E96', "h" }, // LATIN SMALL LETTER H WITH LINE BELOW
            { '\u1E97', "t" }, // LATIN SMALL LETTER T WITH DIAERESIS
            { '\u1E98', "w" }, // LATIN SMALL LETTER W WITH RING ABOVE
            { '\u1E99', "y" }, // LATIN SMALL LETTER Y WITH RING ABOVE
            { '\u1E9A', "a" }, // LATIN SMALL LETTER A WITH RIGHT HALF RING
            { '\u1E9B', "s" }, // LATIN SMALL LETTER LONG S WITH DOT ABOVE
            { '\u1EA0', "A" }, // LATIN CAPITAL LETTER A WITH DOT BELOW
            { '\u1EA1', "a" }, // LATIN SMALL LETTER A WITH DOT BELOW
            { '\u1EA2', "A" }, // LATIN CAPITAL LETTER A WITH HOOK ABOVE
            { '\u1EA3', "a" }, // LATIN SMALL LETTER A WITH HOOK ABOVE
            { '\u1EA4', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND ACUTE
            { '\u1EA5', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX AND ACUTE
            { '\u1EA6', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND GRAVE
            { '\u1EA7', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX AND GRAVE
            { '\u1EA8', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1EA9', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1EAA', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND TILDE
            { '\u1EAB', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX AND TILDE
            { '\u1EAC', "A" }, // LATIN CAPITAL LETTER A WITH CIRCUMFLEX AND DOT BELOW
            { '\u1EAD', "a" }, // LATIN SMALL LETTER A WITH CIRCUMFLEX AND DOT BELOW
            { '\u1EAE', "A" }, // LATIN CAPITAL LETTER A WITH BREVE AND ACUTE
            { '\u1EAF', "a" }, // LATIN SMALL LETTER A WITH BREVE AND ACUTE
            { '\u1EB0', "A" }, // LATIN CAPITAL LETTER A WITH BREVE AND GRAVE
            { '\u1EB1', "a" }, // LATIN SMALL LETTER A WITH BREVE AND GRAVE
            { '\u1EB2', "A" }, // LATIN CAPITAL LETTER A WITH BREVE AND HOOK ABOVE
            { '\u1EB3', "a" }, // LATIN SMALL LETTER A WITH BREVE AND HOOK ABOVE
            { '\u1EB4', "A" }, // LATIN CAPITAL LETTER A WITH BREVE AND TILDE
            { '\u1EB5', "a" }, // LATIN SMALL LETTER A WITH BREVE AND TILDE
            { '\u1EB6', "A" }, // LATIN CAPITAL LETTER A WITH BREVE AND DOT BELOW
            { '\u1EB7', "a" }, // LATIN SMALL LETTER A WITH BREVE AND DOT BELOW
            { '\u1EB8', "E" }, // LATIN CAPITAL LETTER E WITH DOT BELOW
            { '\u1EB9', "e" }, // LATIN SMALL LETTER E WITH DOT BELOW
            { '\u1EBA', "E" }, // LATIN CAPITAL LETTER E WITH HOOK ABOVE
            { '\u1EBB', "e" }, // LATIN SMALL LETTER E WITH HOOK ABOVE
            { '\u1EBC', "E" }, // LATIN CAPITAL LETTER E WITH TILDE
            { '\u1EBD', "e" }, // LATIN SMALL LETTER E WITH TILDE
            { '\u1EBE', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND ACUTE
            { '\u1EBF', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX AND ACUTE
            { '\u1EC0', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND GRAVE
            { '\u1EC1', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX AND GRAVE
            { '\u1EC2', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1EC3', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1EC4', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND TILDE
            { '\u1EC5', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX AND TILDE
            { '\u1EC6', "E" }, // LATIN CAPITAL LETTER E WITH CIRCUMFLEX AND DOT BELOW
            { '\u1EC7', "e" }, // LATIN SMALL LETTER E WITH CIRCUMFLEX AND DOT BELOW
            { '\u1EC8', "I" }, // LATIN CAPITAL LETTER I WITH HOOK ABOVE
            { '\u1EC9', "i" }, // LATIN SMALL LETTER I WITH HOOK ABOVE
            { '\u1ECA', "I" }, // LATIN CAPITAL LETTER I WITH DOT BELOW
            { '\u1ECB', "i" }, // LATIN SMALL LETTER I WITH DOT BELOW
            { '\u1ECC', "O" }, // LATIN CAPITAL LETTER O WITH DOT BELOW
            { '\u1ECD', "o" }, // LATIN SMALL LETTER O WITH DOT BELOW
            { '\u1ECE', "O" }, // LATIN CAPITAL LETTER O WITH HOOK ABOVE
            { '\u1ECF', "o" }, // LATIN SMALL LETTER O WITH HOOK ABOVE
            { '\u1ED0', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND ACUTE
            { '\u1ED1', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX AND ACUTE
            { '\u1ED2', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND GRAVE
            { '\u1ED3', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX AND GRAVE
            { '\u1ED4', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1ED5', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX AND HOOK ABOVE
            { '\u1ED6', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND TILDE
            { '\u1ED7', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX AND TILDE
            { '\u1ED8', "O" }, // LATIN CAPITAL LETTER O WITH CIRCUMFLEX AND DOT BELOW
            { '\u1ED9', "o" }, // LATIN SMALL LETTER O WITH CIRCUMFLEX AND DOT BELOW
            { '\u1EDA', "O" }, // LATIN CAPITAL LETTER O WITH HORN AND ACUTE
            { '\u1EDB', "o" }, // LATIN SMALL LETTER O WITH HORN AND ACUTE
            { '\u1EDC', "O" }, // LATIN CAPITAL LETTER O WITH HORN AND GRAVE
            { '\u1EDD', "o" }, // LATIN SMALL LETTER O WITH HORN AND GRAVE
            { '\u1EDE', "O" }, // LATIN CAPITAL LETTER O WITH HORN AND HOOK ABOVE
            { '\u1EDF', "o" }, // LATIN SMALL LETTER O WITH HORN AND HOOK ABOVE
            { '\u1EE0', "O" }, // LATIN CAPITAL LETTER O WITH HORN AND TILDE
            { '\u1EE1', "o" }, // LATIN SMALL LETTER O WITH HORN AND TILDE
            { '\u1EE2', "O" }, // LATIN CAPITAL LETTER O WITH HORN AND DOT BELOW
            { '\u1EE3', "o" }, // LATIN SMALL LETTER O WITH HORN AND DOT BELOW
            { '\u1EE4', "U" }, // LATIN CAPITAL LETTER U WITH DOT BELOW
            { '\u1EE5', "u" }, // LATIN SMALL LETTER U WITH DOT BELOW
            { '\u1EE6', "U" }, // LATIN CAPITAL LETTER U WITH HOOK ABOVE
            { '\u1EE7', "u" }, // LATIN SMALL LETTER U WITH HOOK ABOVE
            { '\u1EE8', "U" }, // LATIN CAPITAL LETTER U WITH HORN AND ACUTE
            { '\u1EE9', "u" }, // LATIN SMALL LETTER U WITH HORN AND ACUTE
            { '\u1EEA', "U" }, // LATIN CAPITAL LETTER U WITH HORN AND GRAVE
            { '\u1EEB', "u" }, // LATIN SMALL LETTER U WITH HORN AND GRAVE
            { '\u1EEC', "U" }, // LATIN CAPITAL LETTER U WITH HORN AND HOOK ABOVE
            { '\u1EED', "u" }, // LATIN SMALL LETTER U WITH HORN AND HOOK ABOVE
            { '\u1EEE', "U" }, // LATIN CAPITAL LETTER U WITH HORN AND TILDE
            { '\u1EEF', "u" }, // LATIN SMALL LETTER U WITH HORN AND TILDE
            { '\u1EF0', "U" }, // LATIN CAPITAL LETTER U WITH HORN AND DOT BELOW
            { '\u1EF1', "u" }, // LATIN SMALL LETTER U WITH HORN AND DOT BELOW
            { '\u1EF2', "Y" }, // LATIN CAPITAL LETTER Y WITH GRAVE
            { '\u1EF3', "y" }, // LATIN SMALL LETTER Y WITH GRAVE
            { '\u1EF4', "Y" }, // LATIN CAPITAL LETTER Y WITH DOT BELOW
            { '\u1EF5', "y" }, // LATIN SMALL LETTER Y WITH DOT BELOW
            { '\u1EF6', "Y" }, // LATIN CAPITAL LETTER Y WITH HOOK ABOVE
            { '\u1EF7', "y" }, // LATIN SMALL LETTER Y WITH HOOK ABOVE
            { '\u1EF8', "Y" }, // LATIN CAPITAL LETTER Y WITH TILDE
            { '\u1EF9', "y" }, // LATIN SMALL LETTER Y WITH TILDE
            { '\u2071', "i" }, // SUPERSCRIPT LATIN SMALL LETTER I
            { '\u207F', "n" }, // SUPERSCRIPT LATIN SMALL LETTER N
            { '\u212A', "K" }, // KELVIN SIGN
            { '\u212B', "A" }, // ANGSTROM SIGN
            { '\u212C', "B" }, // SCRIPT CAPITAL B
            { '\u212D', "C" }, // BLACK-LETTER CAPITAL C
            { '\u212F', "e" }, // SCRIPT SMALL E
            { '\u2130', "E" }, // SCRIPT CAPITAL E
            { '\u2131', "F" }, // SCRIPT CAPITAL F
            { '\u2132', "F" }, // TURNED CAPITAL F -- no decomposition
            { '\u2133', "M" }, // SCRIPT CAPITAL M
            { '\u2134', "0" }, // SCRIPT SMALL O
            { '\u213A', "0" }, // ROTATED CAPITAL Q -- no decomposition
            { '\u2141', "G" }, // TURNED SANS-SERIF CAPITAL G -- no decomposition
            { '\u2142', "L" }, // TURNED SANS-SERIF CAPITAL L -- no decomposition
            { '\u2143', "L" }, // REVERSED SANS-SERIF CAPITAL L -- no decomposition
            { '\u2144', "Y" }, // TURNED SANS-SERIF CAPITAL Y -- no decomposition
            { '\u2145', "D" }, // DOUBLE-STRUCK ITALIC CAPITAL D
            { '\u2146', "d" }, // DOUBLE-STRUCK ITALIC SMALL D
            { '\u2147', "e" }, // DOUBLE-STRUCK ITALIC SMALL E
            { '\u2148', "i" }, // DOUBLE-STRUCK ITALIC SMALL I
            { '\u2149', "j" }, // DOUBLE-STRUCK ITALIC SMALL J
            { '\uFB00', "ff" }, // LATIN SMALL LIGATURE FF
            { '\uFB01', "fi" }, // LATIN SMALL LIGATURE FI
            { '\uFB02', "fl" }, // LATIN SMALL LIGATURE FL
            { '\uFB03', "ffi" }, // LATIN SMALL LIGATURE FFI
            { '\uFB04', "ffl" }, // LATIN SMALL LIGATURE FFL
            { '\uFB05', "st" }, // LATIN SMALL LIGATURE LONG S T
            { '\uFB06', "st" }, // LATIN SMALL LIGATURE ST
            { '\uFF21', "A" }, // FULLWIDTH LATIN CAPITAL LETTER B
            { '\uFF22', "B" }, // FULLWIDTH LATIN CAPITAL LETTER B
            { '\uFF23', "C" }, // FULLWIDTH LATIN CAPITAL LETTER C
            { '\uFF24', "D" }, // FULLWIDTH LATIN CAPITAL LETTER D
            { '\uFF25', "E" }, // FULLWIDTH LATIN CAPITAL LETTER E
            { '\uFF26', "F" }, // FULLWIDTH LATIN CAPITAL LETTER F
            { '\uFF27', "G" }, // FULLWIDTH LATIN CAPITAL LETTER G
            { '\uFF28', "H" }, // FULLWIDTH LATIN CAPITAL LETTER H
            { '\uFF29', "I" }, // FULLWIDTH LATIN CAPITAL LETTER I
            { '\uFF2A', "J" }, // FULLWIDTH LATIN CAPITAL LETTER J
            { '\uFF2B', "K" }, // FULLWIDTH LATIN CAPITAL LETTER K
            { '\uFF2C', "L" }, // FULLWIDTH LATIN CAPITAL LETTER L
            { '\uFF2D', "M" }, // FULLWIDTH LATIN CAPITAL LETTER M
            { '\uFF2E', "N" }, // FULLWIDTH LATIN CAPITAL LETTER N
            { '\uFF2F', "O" }, // FULLWIDTH LATIN CAPITAL LETTER O
            { '\uFF30', "P" }, // FULLWIDTH LATIN CAPITAL LETTER P
            { '\uFF31', "Q" }, // FULLWIDTH LATIN CAPITAL LETTER Q
            { '\uFF32', "R" }, // FULLWIDTH LATIN CAPITAL LETTER R
            { '\uFF33', "S" }, // FULLWIDTH LATIN CAPITAL LETTER S
            { '\uFF34', "T" }, // FULLWIDTH LATIN CAPITAL LETTER T
            { '\uFF35', "U" }, // FULLWIDTH LATIN CAPITAL LETTER U
            { '\uFF36', "V" }, // FULLWIDTH LATIN CAPITAL LETTER V
            { '\uFF37', "W" }, // FULLWIDTH LATIN CAPITAL LETTER W
            { '\uFF38', "X" }, // FULLWIDTH LATIN CAPITAL LETTER X
            { '\uFF39', "Y" }, // FULLWIDTH LATIN CAPITAL LETTER Y
            { '\uFF3A', "Z" }, // FULLWIDTH LATIN CAPITAL LETTER Z
            { '\uFF41', "a" }, // FULLWIDTH LATIN SMALL LETTER A
            { '\uFF42', "b" }, // FULLWIDTH LATIN SMALL LETTER B
            { '\uFF43', "c" }, // FULLWIDTH LATIN SMALL LETTER C
            { '\uFF44', "d" }, // FULLWIDTH LATIN SMALL LETTER D
            { '\uFF45', "e" }, // FULLWIDTH LATIN SMALL LETTER E
            { '\uFF46', "f" }, // FULLWIDTH LATIN SMALL LETTER F
            { '\uFF47', "g" }, // FULLWIDTH LATIN SMALL LETTER G
            { '\uFF48', "h" }, // FULLWIDTH LATIN SMALL LETTER H
            { '\uFF49', "i" }, // FULLWIDTH LATIN SMALL LETTER I
            { '\uFF4A', "j" }, // FULLWIDTH LATIN SMALL LETTER J
            { '\uFF4B', "k" }, // FULLWIDTH LATIN SMALL LETTER K
            { '\uFF4C', "l" }, // FULLWIDTH LATIN SMALL LETTER L
            { '\uFF4D', "m" }, // FULLWIDTH LATIN SMALL LETTER M
            { '\uFF4E', "n" }, // FULLWIDTH LATIN SMALL LETTER N
            { '\uFF4F', "o" }, // FULLWIDTH LATIN SMALL LETTER O
            { '\uFF50', "p" }, // FULLWIDTH LATIN SMALL LETTER P
            { '\uFF51', "q" }, // FULLWIDTH LATIN SMALL LETTER Q
            { '\uFF52', "r" }, // FULLWIDTH LATIN SMALL LETTER R
            { '\uFF53', "s" }, // FULLWIDTH LATIN SMALL LETTER S
            { '\uFF54', "t" }, // FULLWIDTH LATIN SMALL LETTER T
            { '\uFF55', "u" }, // FULLWIDTH LATIN SMALL LETTER U
            { '\uFF56', "v" }, // FULLWIDTH LATIN SMALL LETTER V
            { '\uFF57', "w" }, // FULLWIDTH LATIN SMALL LETTER W
            { '\uFF58', "x" }, // FULLWIDTH LATIN SMALL LETTER X
            { '\uFF59', "y" }, // FULLWIDTH LATIN SMALL LETTER Y
            { '\uFF5A', "z" }, // FULLWIDTH LATIN SMALL LETTER Z
            { '\u0410', "A" }, // RUSSIAN CAPITAL LETTER А 
            { '\u0411', "B" }, // RUSSIAN CAPITAL LETTER Б
            { '\u0412', "V" }, // RUSSIAN CAPITAL LETTER В
            { '\u0413', "G" }, // RUSSIAN CAPITAL LETTER Г
            { '\u0414', "D" }, // RUSSIAN CAPITAL LETTER Д
            { '\u0415', "E" }, // RUSSIAN CAPITAL LETTER Е
            { '\u0401', "YO" }, // RUSSIAN CAPITAL LETTER Ё
            { '\u0416', "ZH" }, // RUSSIAN CAPITAL LETTER Ж
            { '\u0417', "Z" }, // RUSSIAN CAPITAL LETTER З
            { '\u0418', "I" }, // RUSSIAN CAPITAL LETTER И
            { '\u0419', "J" }, // RUSSIAN CAPITAL LETTER Й
            { '\u041A', "K" }, // RUSSIAN CAPITAL LETTER К
            { '\u041B', "L" }, // RUSSIAN CAPITAL LETTER Л
            { '\u041C', "M" }, // RUSSIAN CAPITAL LETTER М
            { '\u041D', "N" }, // RUSSIAN CAPITAL LETTER Н
            { '\u041E', "O" }, // RUSSIAN CAPITAL LETTER О
            { '\u041F', "P" }, // RUSSIAN CAPITAL LETTER П
            { '\u0420', "R" }, // RUSSIAN CAPITAL LETTER Р
            { '\u0421', "S" }, // RUSSIAN CAPITAL LETTER С
            { '\u0422', "T" }, // RUSSIAN CAPITAL LETTER Т
            { '\u0423', "U" }, // RUSSIAN CAPITAL LETTER У
            { '\u0424', "F" }, // RUSSIAN CAPITAL LETTER Ф
            { '\u0425', "H" }, // RUSSIAN CAPITAL LETTER Х
            { '\u0426', "C" }, // RUSSIAN CAPITAL LETTER Ц
            { '\u0427', "CH" }, // RUSSIAN CAPITAL LETTER Ч
            { '\u0428', "SH" }, // RUSSIAN CAPITAL LETTER Ш
            { '\u0429', "SHH" }, // RUSSIAN CAPITAL LETTER Щ
            { '\u042A', string.Empty }, // RUSSIAN CAPITAL LETTER Ъ
            { '\u042B', "Y" }, // RUSSIAN CAPITAL LETTER Ы
            { '\u042C', string.Empty }, // RUSSIAN CAPITAL LETTER Ь
            { '\u042D', "E" }, // RUSSIAN CAPITAL LETTER Э
            { '\u042E', "YU" }, // RUSSIAN CAPITAL LETTER Ю
            { '\u042F', "YA" }, // RUSSIAN CAPITAL LETTER Я
            { '\u0430', "a" }, // RUSSIAN SMALL LETTER а
            { '\u0431', "b" }, // RUSSIAN SMALL LETTER б
            { '\u0432', "v" }, // RUSSIAN SMALL LETTER в
            { '\u0433', "g" }, // RUSSIAN SMALL LETTER г
            { '\u0434', "d" }, // RUSSIAN SMALL LETTER д
            { '\u0435', "e" }, // RUSSIAN SMALL LETTER е
            { '\u0451', "yo" }, // RUSSIAN SMALL LETTER ё
            { '\u0436', "zh" }, // RUSSIAN SMALL LETTER ж
            { '\u0437', "z" }, // RUSSIAN SMALL LETTER з
            { '\u0438', "i" }, // RUSSIAN SMALL LETTER и
            { '\u0439', "j" }, // RUSSIAN SMALL LETTER й
            { '\u043A', "k" }, // RUSSIAN SMALL LETTER к
            { '\u043B', "l" }, // RUSSIAN SMALL LETTER л
            { '\u043C', "m" }, // RUSSIAN SMALL LETTER м
            { '\u043D', "n" }, // RUSSIAN SMALL LETTER н
            { '\u043E', "o" }, // RUSSIAN SMALL LETTER о
            { '\u043F', "p" }, // RUSSIAN SMALL LETTER п
            { '\u0440', "r" }, // RUSSIAN SMALL LETTER р
            { '\u0441', "s" }, // RUSSIAN SMALL LETTER с
            { '\u0442', "t" }, // RUSSIAN SMALL LETTER т
            { '\u0443', "u" }, // RUSSIAN SMALL LETTER у
            { '\u0444', "f" }, // RUSSIAN SMALL LETTER ф
            { '\u0445', "h" }, // RUSSIAN SMALL LETTER х
            { '\u0446', "c" }, // RUSSIAN SMALL LETTER ц
            { '\u0447', "ch" }, // RUSSIAN SMALL LETTER ч
            { '\u0448', "sh" }, // RUSSIAN SMALL LETTER ш
            { '\u0449', "shh" }, // RUSSIAN SMALL LETTER щ
            { '\u044A', string.Empty }, // RUSSIAN SMALL LETTER ъ
            { '\u044B', "y" }, // RUSSIAN SMALL LETTER ы
            { '\u044C', string.Empty }, // RUSSIAN SMALL LETTER ь
            { '\u044D', "e" }, // RUSSIAN SMALL LETTER э
            { '\u044E', "yu" }, // RUSSIAN SMALL LETTER ю
            { '\u044F', "ya" }, // RUSSIAN SMALL LETTER я
            { '\u0406', "I" }, // Ukraine-Byelorussian CAPITAL LETTER І
            { '\u0456', "i" }, // Ukraine-Byelorussian SMALL LETTER і
            { '\u0407', "I" }, // Ukraine CAPITAL LETTER Ї
            { '\u0457', "i" }, // Ukraine SMALL LETTER ї
            { '\u0404', "Ie" }, // Ukraine CAPITAL LETTER Є
            { '\u0454', "ie" }, // Ukraine SMALL LETTER є
            { '\u0490', "G" }, // Ukraine CAPITAL LETTER Ґ
            { '\u0491', "g" }, // Ukraine SMALL LETTER ґ
            { '\u040E', "U" }, // Byelorussian CAPITAL LETTER Ў
            { '\u045E', "u" } // Byelorussian SMALL LETTER ў
        };
    }

    #endregion

    #region Utilities

    [GeneratedRegex(@"[\s-]+")]
    protected static partial Regex WhitespaceHyphenRegex();
    [GeneratedRegex(@"_+")]
    protected static partial Regex UnderscoreRegex();

    #endregion

    #region Methods

    /// <summary>
    /// Deletes an URL records
    /// </summary>
    /// <param name="urlRecords">URL records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteUrlRecordsAsync(IList<UrlRecord> urlRecords)
    {
        await _urlRecordRepository.DeleteAsync(urlRecords);
    }

    /// <summary>
    /// Gets an URL records
    /// </summary>
    /// <param name="urlRecordIds">URL record identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL record
    /// </returns>
    public virtual async Task<IList<UrlRecord>> GetUrlRecordsByIdsAsync(int[] urlRecordIds)
    {
        return await _urlRecordRepository.GetByIdsAsync(urlRecordIds, _ => default);
    }

    /// <summary>
    /// Inserts an URL record
    /// </summary>
    /// <param name="urlRecord">URL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertUrlRecordAsync(UrlRecord urlRecord)
    {
        await _urlRecordRepository.InsertAsync(urlRecord);
    }

    /// <summary>
    /// Updates the URL record
    /// </summary>
    /// <param name="urlRecord">URL record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateUrlRecordAsync(UrlRecord urlRecord)
    {
        await _urlRecordRepository.UpdateAsync(urlRecord);
    }

    /// <summary>
    /// Find URL record
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found URL record
    /// </returns>
    public virtual async Task<UrlRecord> GetBySlugAsync(string slug)
    {
        if (string.IsNullOrEmpty(slug))
            return null;

        if (_localizationSettings.LoadAllUrlRecordsOnStartup)
        {
            var records = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopSeoDefaults.UrlRecordSlugLookupCacheKey),
                async () => (await GetAllUrlRecordsAsync())
                    .GroupBy(x => x.Slug.ToLower())
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id).First()));

            return records.TryGetValue(slug.ToLower(), out var record)
                ? record
                : null;
        }

        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSeoDefaults.UrlRecordBySlugCacheKey, slug);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            // gradual loading
            var query = from ur in _urlRecordRepository.Table
                        where ur.Slug == slug
                        //first, try to find an active record
                        orderby ur.IsActive descending, ur.Id
                        select ur;

            return await query.FirstOrDefaultAsync();
        });
    }

    /// <summary>
    /// Gets all URL records
    /// </summary>
    /// <param name="slug">Slug</param>
    /// <param name="languageId">Language ID; "null" to load records with any language; "0" to load records with standard language only; otherwise to load records with specify language ID only</param>
    /// <param name="isActive">A value indicating whether to get active records; "null" to load all records; "false" to load only inactive records; "true" to load only active records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the uRL records
    /// </returns>
    public virtual async Task<IPagedList<UrlRecord>> GetAllUrlRecordsAsync(
        string slug = "", int? languageId = null, bool? isActive = null, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var urlRecords = (await _urlRecordRepository.GetAllAsync(query => query.OrderBy(ur => ur.Slug), _ => default))
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(slug))
            urlRecords = urlRecords.Where(ur => ur.Slug.Contains(slug));

        if (languageId.HasValue)
            urlRecords = urlRecords.Where(ur => ur.LanguageId == languageId);

        if (isActive.HasValue)
            urlRecords = urlRecords.Where(ur => ur.IsActive == isActive);

        var result = urlRecords.ToList();

        return new PagedList<UrlRecord>(result, pageIndex, pageSize);
    }

    /// <summary>
    /// Find slug
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="languageId">Language identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the found slug
    /// </returns>
    public virtual async Task<string> GetActiveSlugAsync(int entityId, string entityName, int languageId)
    {
        if (_localizationSettings.LoadAllUrlRecordsOnStartup)
        {
            //value tuples aren't json-serializable by default, so we use a string key
            static string formatKey(string name, int id) => $"{name}:{id}";

            var activeSlugs = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopSeoDefaults.UrlRecordEntityIdLookupCacheKey, languageId),
                async () => (await GetAllUrlRecordsAsync())
                    .Where(x => x.IsActive && x.LanguageId == languageId)
                    .GroupBy(x => formatKey(x.EntityName, x.EntityId))
                    .ToDictionary(g => g.Key, g => g.MinBy(x => x.Id)!.Slug));

            return activeSlugs.TryGetValue(formatKey(entityName, entityId), out var slug)
                ? slug
                : string.Empty;
        }

        //gradual loading
        var key = _staticCacheManager.PrepareKeyForDefaultCache(NopSeoDefaults.UrlRecordCacheKey, entityId, entityName, languageId);

        return await _staticCacheManager.GetAsync(key, async () =>
        {
            var query = from ur in _urlRecordRepository.Table
                        where ur.EntityId == entityId &&
                            ur.EntityName == entityName &&
                            ur.LanguageId == languageId &&
                            ur.IsActive
                        orderby ur.Id descending
                        select ur.Slug;

            //little hack here. nulls aren't cacheable so set it to ""
            return await query.FirstOrDefaultAsync() ?? string.Empty;
        });
    }

    /// <summary>
    /// Save slug
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="slug">Slug</param>
    /// <param name="languageId">Language ID</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task SaveSlugAsync<T>(T entity, string slug, int languageId) where T : BaseEntity, ISlugSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = entity.Id;
        var entityName = entity.GetType().Name;

        var query = from ur in _urlRecordRepository.Table
                    where ur.EntityId == entityId &&
                        ur.EntityName == entityName &&
                        ur.LanguageId == languageId
                    orderby ur.Id descending
                    select ur;
        var allUrlRecords = await query.ToListAsync();
        var activeUrlRecord = allUrlRecords.FirstOrDefault(x => x.IsActive);
        UrlRecord nonActiveRecordWithSpecifiedSlug;

        if (activeUrlRecord == null && !string.IsNullOrWhiteSpace(slug))
        {
            //find in non-active records with the specified slug
            nonActiveRecordWithSpecifiedSlug = allUrlRecords
                .FirstOrDefault(
                    x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);
            if (nonActiveRecordWithSpecifiedSlug != null)
            {
                //mark non-active record as active
                nonActiveRecordWithSpecifiedSlug.IsActive = true;
                await UpdateUrlRecordAsync(nonActiveRecordWithSpecifiedSlug);
            }
            else
            {
                //new record
                var urlRecord = new UrlRecord
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    Slug = slug,
                    LanguageId = languageId,
                    IsActive = true
                };
                await InsertUrlRecordAsync(urlRecord);
            }
        }

        if (activeUrlRecord != null && string.IsNullOrWhiteSpace(slug))
        {
            //disable the previous active URL record
            activeUrlRecord.IsActive = false;
            await UpdateUrlRecordAsync(activeUrlRecord);
        }

        if (activeUrlRecord == null || string.IsNullOrWhiteSpace(slug))
            return;

        //it should not be the same slug as in active URL record
        if (activeUrlRecord.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            return;

        //find in non-active records with the specified slug
        nonActiveRecordWithSpecifiedSlug = allUrlRecords
            .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);
        if (nonActiveRecordWithSpecifiedSlug != null)
        {
            //mark non-active record as active
            nonActiveRecordWithSpecifiedSlug.IsActive = true;
            await UpdateUrlRecordAsync(nonActiveRecordWithSpecifiedSlug);

            //disable the previous active URL record
            activeUrlRecord.IsActive = false;
            await UpdateUrlRecordAsync(activeUrlRecord);
        }
        else
        {
            //insert new record
            //we do not update the existing record because we should track all previously entered slugs
            //to ensure that URLs will work fine
            var urlRecord = new UrlRecord
            {
                EntityId = entityId,
                EntityName = entityName,
                Slug = slug,
                LanguageId = languageId,
                IsActive = true
            };
            await InsertUrlRecordAsync(urlRecord);

            //disable the previous active URL record
            activeUrlRecord.IsActive = false;
            await UpdateUrlRecordAsync(activeUrlRecord);
        }
    }

    /// <summary>
    ///  Get search engine friendly name (slug)
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="entity">Entity</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search engine  name (slug)
    /// </returns>
    public virtual async Task<string> GetSeNameAsync<T>(T entity, int? languageId = null, bool returnDefaultValue = true,
        bool ensureTwoPublishedLanguages = true) where T : BaseEntity, ISlugSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityName = entity.GetType().Name;

        return await GetSeNameAsync(entity.Id, entityName, languageId ?? (await _workContext.GetWorkingLanguageAsync()).Id, returnDefaultValue, ensureTwoPublishedLanguages);
    }

    /// <summary>
    /// Get search engine friendly name (slug)
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="returnDefaultValue">A value indicating whether to return default value (if language specified one is not found)</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the search engine  name (slug)
    /// </returns>
    public virtual async Task<string> GetSeNameAsync(int entityId, string entityName, int? languageId = null,
        bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
    {
        languageId ??= (await _workContext.GetWorkingLanguageAsync()).Id;
        var result = string.Empty;

        if (languageId > 0)
        {
            //ensure that we have at least two published languages
            var loadLocalizedValue = true;
            if (ensureTwoPublishedLanguages)
            {
                var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                loadLocalizedValue = totalPublishedLanguages >= 2;
            }

            //localized value
            if (loadLocalizedValue)
                result = await GetActiveSlugAsync(entityId, entityName, languageId.Value);
        }

        //set default value if required
        if (string.IsNullOrEmpty(result) && returnDefaultValue)
            result = await GetActiveSlugAsync(entityId, entityName, 0);

        return result;
    }

    /// <summary>
    /// Get SE name
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="convertNonWesternChars">A value indicating whether non western chars should be converted</param>
    /// <param name="allowUnicodeCharsInUrls">A value indicating whether Unicode chars are allowed</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result
    /// </returns>
    public virtual Task<string> GetSeNameAsync(string name, bool convertNonWesternChars, bool allowUnicodeCharsInUrls)
    {
        if (string.IsNullOrEmpty(name))
            return Task.FromResult(name);

        var sb = new StringBuilder();
        foreach (var c in name.Trim().ToLowerInvariant())
            if (convertNonWesternChars && _seoCharacterTable.TryGetValue(c, out var transliteration))
                sb.Append(transliteration.ToLowerInvariant());
            else if (_okChars.Contains(c) || allowUnicodeCharsInUrls && char.IsLetterOrDigit(c))
                sb.Append(c);

        var seName = WhitespaceHyphenRegex().Replace(sb.ToString(), "-");
        seName = UnderscoreRegex().Replace(seName, "_");

        return Task.FromResult(seName);
    }

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <param name="name">User-friendly name used to generate seName</param>
    /// <param name="ensureNotEmpty">Ensure that seName is not empty</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
    public virtual async Task<string> ValidateSeNameAsync<T>(T entity, string seName, string name, bool ensureNotEmpty) where T : BaseEntity, ISlugSupported
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityName = entity.GetType().Name;

        return await ValidateSeNameAsync(entity.Id, entityName, seName, name, ensureNotEmpty);
    }

    /// <summary>
    /// Validate search engine name
    /// </summary>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="entityName">Entity name</param>
    /// <param name="seName">Search engine name to validate</param>
    /// <param name="name">User-friendly name used to generate seName</param>
    /// <param name="ensureNotEmpty">Ensure that seName is not empty</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the valid seName
    /// </returns>
    public virtual async Task<string> ValidateSeNameAsync(int entityId, string entityName, string seName, string name, bool ensureNotEmpty)
    {
        //use name if seName is not specified
        if (string.IsNullOrWhiteSpace(seName) && !string.IsNullOrWhiteSpace(name))
            seName = name;

        //validation
        seName = await GetSeNameAsync(seName, _seoSettings.ConvertNonWesternChars, _seoSettings.AllowUnicodeCharsInUrls);

        //max length
        seName = CommonHelper.EnsureMaximumLength(seName, NopSeoDefaults.SearchEngineNameLength);

        if (string.IsNullOrWhiteSpace(seName))
        {
            if (ensureNotEmpty)
                //use entity identifier as seName if empty
                seName = entityId.ToString();
            else
                //return. no need for further processing
                return seName;
        }

        //ensure this seName is not reserved yet
        var i = 2;
        var tempSeName = seName;
        while (true)
        {
            //check whether such slug already exists (and that is not the current entity)
            var urlRecord = await GetBySlugAsync(tempSeName);
            var reserved1 = urlRecord != null && !(urlRecord.EntityId == entityId && urlRecord.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            //and it's not in the list of reserved slugs
            var reserved2 = _seoSettings.ReservedUrlRecordSlugs.Contains(tempSeName, StringComparer.InvariantCultureIgnoreCase);
            //and it's not equal to a language code
            var reserved3 = (await _languageService.GetAllLanguagesAsync(true)).Any(language => language.UniqueSeoCode.Equals(tempSeName, StringComparison.InvariantCultureIgnoreCase));
            if (!reserved1 && !reserved2 && !reserved3)
                break;

            tempSeName = $"{seName}-{i}";
            i++;
        }

        seName = tempSeName;

        return seName;
    }

    #endregion
}