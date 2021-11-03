using System;
using System.Text.RegularExpressions;

namespace CodeNameK.DataAccess
{
    /// <summary>
    /// Escape / unescape special characters for folders.
    ///
    /// The following reserved characters:
    /// < (less than)
    /// > (greater than)
    /// : (colon)
    /// " (double quote)
    /// / (forward slash)
    /// \ (backslash)
    /// | (vertical bar or pipe)
    /// ? (question mark)
    /// * (asterisk)
    /// Refer to stack overflow: https://stackoverflow.com/questions/15087444/c-sharp-file-path-encoding-and-decoding
    /// /// </summary>
    public class FileNameEscaper
    {
        private const string _invalidChars = @"<>:""/\|?*";
        private const string _escapeChar = "%";

        private static readonly Regex escaper = new Regex("[" + Regex.Escape(_escapeChar + _invalidChars) + "]", RegexOptions.Compiled);
        private static readonly Regex unescaper = new Regex(Regex.Escape(_escapeChar) + "([0-9A-Z]{4})", RegexOptions.Compiled);

        public string Escape(string path)
        {
            return escaper.Replace(path, m => _escapeChar + ((short)(m.Value[0])).ToString("X4"));
        }

        public string Unescape(string path)
        {
            return unescaper.Replace(path,
                m => ((char)Convert.ToInt16(m.Groups[1].Value, 16)).ToString());
        }
    }
}