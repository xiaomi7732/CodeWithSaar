using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeWithSaar
{
    /// <summary>
    /// Do not use the following reserved names for the name of a file:
    /// CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, and LPT9.
    /// Also avoid these names followed immediately by an extension; for example, NUL.txt is not recommended.
    /// </summary>
    internal class ReservedFileNameProcessor : IFileNameProcessor
    {
        private const string _escapeChar = "%";

        private readonly List<string> _reservedNames = new List<string>(){
            "CON",
            "PRN",
            "AUX",
            "NUL",
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "LPT1",
            "LPT2",
            "LPT3",
            "LPT4",
            "LPT5",
            "LPT6",
            "LPT7",
            "LPT8",
            "LPT9",
        };

        private readonly Regex _decoder;
        private readonly Regex _encoder;
        private readonly bool _escapeEscaper;

        public ReservedFileNameProcessor(bool escapeEscaper = true)
        {
            StringBuilder patternBuilder = new StringBuilder();
            patternBuilder.Append("^((");
            foreach (string reservedName in _reservedNames)
            {
                patternBuilder.AppendFormat("{0}|", reservedName);
            }
            patternBuilder.Remove(patternBuilder.Length - 1, 1); // Remove the last separator |
            patternBuilder.Append(")(?<ext>\\..*)*)$");
            _encoder = new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

            patternBuilder.Insert(1, _escapeChar);
            _decoder = new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            _escapeEscaper = escapeEscaper;
        }

        /// <summary>
        /// Decodes a folder name to its original form. For example, if the folder name was encoded to '%CON' from 'CON', the return value would be 'CON'.
        /// </summary>
        public string Decode(string fileName)
        {
            if (_decoder.IsMatch(fileName))
            {
                fileName = _decoder.Replace(fileName, match => match.Groups[1].Value);
            }
            return PostDecode(fileName);
        }

        /// <summary>
        /// Encode a file name to avoid using reserved file names.
        /// </summary>
        public string Encode(string fileName)
        {
            fileName = PreEncode(fileName);
            return _encoder.IsMatch(fileName) ? _escapeChar + fileName : fileName;
        }

        private string PreEncode(string fileName)
        {
            if (!_escapeEscaper)
            {
                return fileName;
            }

            // Escape the escape character by doubling itself
            return fileName.Replace(_escapeChar, _escapeChar + _escapeChar);
        }

        /// <summary>
        /// Un-escaping the escape character
        /// </summary>
        private string PostDecode(string fileName)
        {
            if (!_escapeEscaper)
            {
                return fileName;
            }
            return fileName.Replace(_escapeChar + _escapeChar, _escapeChar);
        }
    }
}