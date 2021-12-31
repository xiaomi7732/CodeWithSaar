using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar
{
    internal class TailingCharacterProcessor : IFileNameProcessor
    {
        private readonly ITailingCharacterProcessorOptions _options;
        private readonly List<(string encodedEnding, char origin)> _encodingMapping;

        public TailingCharacterProcessor(
            ITailingCharacterProcessorOptions options)
        {
            _options = options ?? throw new System.ArgumentNullException(nameof(options));

            _encodingMapping = new List<(string, char)>();
            foreach (char invalidEndingChar in _options.InvalidCharacters)
            {
                _encodingMapping.Add((Encode(invalidEndingChar), invalidEndingChar));
            }
        }

        public string Encode(string fileName)
        {
            fileName = PreEncode(fileName);

            char tail = fileName[fileName.Length - 1];
            (string encodedString, char _) = _encodingMapping.FirstOrDefault(p => p.origin == tail);
            if (!string.IsNullOrEmpty(encodedString))
            {
                fileName = fileName.Substring(0, fileName.Length - 1) + encodedString;
            }
            return fileName;
        }

        public string Decode(string fileName)
        {
            string decoded = fileName;

            foreach ((string encodedEnding, char originChar) in _encodingMapping)
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
            return _options.Escaper + ((short)input).ToString("X4");
        }

        private string PreEncode(string fileName)
        {
            if (!_options.EscapeEscaper)
            {
                return fileName;
            }

            // Escape the escape character by doubling itself
            return fileName.Replace(_options.Escaper, _options.Escaper + _options.Escaper);
        }

        /// <summary>
        /// Un-escaping the escape character
        /// </summary>
        private string PostDecode(string fileName)
        {
            if (!_options.EscapeEscaper)
            {
                return fileName;
            }
            return fileName.Replace(_options.Escaper + _options.Escaper, _options.Escaper);
        }
    }
}