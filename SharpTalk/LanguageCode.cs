using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTalk
{
    /// <summary>
    /// Common language codes used for loading DECtalk dictionaries.
    /// </summary>
    public static class LanguageCode
    {
        /// <summary>
        /// United States English
        /// </summary>
        public const string EnglishUS = "US";
        /// <summary>
        /// United Kingdom English
        /// </summary>
        public const string EnglishUK = "UK";
        /// <summary>
        /// Castilian Spanish
        /// </summary>
        public const string SpanishCastilian = "SP";
        /// <summary>
        /// Latin-American Spanish
        /// </summary>
        public const string SpanishLatinAmerican = "LA";
        /// <summary>
        /// German
        /// </summary>
        public const string German = "GR";
        /// <summary>
        /// French
        /// </summary>
        public const string French = "FR";
        /// <summary>
        /// A special language code that tells SharpTalk not to load any language files.
        /// </summary>
        public const string None = "XX";
    }
}
