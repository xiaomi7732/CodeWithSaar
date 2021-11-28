using System;
using System.Text.RegularExpressions;

namespace CodeWithSaar
{
    /// <summary>
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
    /// </summary>    
    internal class ReservedCharacterProcessor : IFileNameProcessor
    {
        private readonly string _escapeChar = "%";
        private readonly Regex _encoder;
        private readonly Regex _decoder;

        public ReservedCharacterProcessor(IReservedCharacterProcessorOptions options)
        {
            _escapeChar = options.Escaper;
            string invalidChars = options.InvalidCharacters;

            _encoder = new Regex("[" + Regex.Escape(invalidChars + _escapeChar) + "]", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            _decoder = new Regex(Regex.Escape(_escapeChar) + "([0-9A-Z]{4})", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        }

        public string Encode(string filePath)
        {
            return _encoder.Replace(filePath, m => _escapeChar + ((short)(m.Value[0])).ToString("X4"));
        }

        public string Decode(string safeFileName)
        {
            return _decoder.Replace(safeFileName,
                m => ((char)Convert.ToInt16(m.Groups[1].Value, 16)).ToString());
        }
    }
}