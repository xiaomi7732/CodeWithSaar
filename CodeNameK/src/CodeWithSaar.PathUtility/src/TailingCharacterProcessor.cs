using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar
{
    internal class TailingCharacterProcessor : IFileNameProcessor
    {
        private const string _escapeChar = "%";
        private readonly bool _escapeEscaper;
        private const string _invalidEnding = ". ";
        private readonly List<(string encodedEnding, char origin)> _encodedTrails;

        public TailingCharacterProcessor(bool escapeEscaper = true)
        {
            _escapeEscaper = escapeEscaper;
            _encodedTrails = new List<(string, char)>();
            foreach (char invalidEndingChar in _invalidEnding)
            {
                _encodedTrails.Add((Encode(invalidEndingChar), invalidEndingChar));
            }
        }

        public string Encode(string fileName)
        {
            if (_escapeEscaper)
            {
                fileName = PreEncode(fileName);
            }

            char tail = fileName[fileName.Length - 1];
            (string encodedString, char _) = _encodedTrails.FirstOrDefault(p => p.origin == tail);
            if (!string.IsNullOrEmpty(encodedString))
            {
                fileName = fileName.Substring(0, fileName.Length - 1) + encodedString;
            }
            return fileName;
        }

        public string Decode(string fileName)
        {
            string decoded = fileName;

            foreach ((string encodedEnding, char originChar) in _encodedTrails)
            {
                if (fileName.EndsWith(encodedEnding))
                {
                    decoded = fileName.Substring(0, fileName.Length - encodedEnding.Length) + originChar;
                }
            }
            return PostDecode(decoded);
        }

        private string Encode(char input)
        {
            return _escapeChar + ((short)input).ToString("X4");
        }

        private string PreEncode(string fileName)
        {
            if (!_escapeEscaper)
            {
                return fileName;
            }

            // Escape the escape character by doubling itself
            return fileName.Replace(_escapeChar.ToString(), _escapeChar.ToString() + _escapeChar);
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
            return fileName.Replace(_escapeChar.ToString() + _escapeChar, _escapeChar.ToString());
        }
    }
}