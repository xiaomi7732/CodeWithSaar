using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar
{
    public class LeadingCharacterProcessor
    {
        private readonly ILeadingCharacterProcessorOptions _options;

        private readonly IEnumerable<(string encodedChar, char origin)> _encodingMapping;

        public LeadingCharacterProcessor(ILeadingCharacterProcessorOptions options)
        {
            _options = options ?? throw new System.ArgumentNullException(nameof(options));

            List<(string, char)> mapping = new List<(string, char)>();
            foreach (char invalidCharacter in _options.InvalidCharacters)
            {
                mapping.Add((Encode(invalidCharacter), invalidCharacter));
            }
            _encodingMapping = mapping;
        }

        public string Encode(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new System.ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            }

            fileName = PreEncode(fileName);

            char leading = fileName[0];
            (string encodedString, char _) = _encodingMapping.FirstOrDefault(p => p.origin == leading);
            if (!string.IsNullOrEmpty(encodedString))
            {
                fileName = encodedString + fileName.Substring(1);
            }
            return fileName;
        }

        public string Decode(string fileName)
        {
            string decoded = fileName;

            foreach ((string encodedLeading, char originChar) in _encodingMapping)
            {
                if (fileName.StartsWith(encodedLeading))
                {
                    decoded = originChar + fileName.Substring(encodedLeading.Length);
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
