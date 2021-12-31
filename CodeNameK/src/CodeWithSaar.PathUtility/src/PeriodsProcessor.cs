namespace CodeWithSaar
{
    internal class PeriodsProcessor : IFileNameProcessor
    {
        private const string _escapeChar = "%";
        private readonly bool _escapeEscaper;

        public PeriodsProcessor(bool escapeEscaper = true)
        {
            _escapeEscaper = escapeEscaper;
        }

        public string Encode(string fileName)
        {
            if (_escapeEscaper)
            {
                fileName = PreEncode(fileName);
            }
            switch (fileName)
            {
                case ".":
                    return _escapeChar + "002E";
                case "..":
                    return _escapeChar + "002E" + _escapeChar + "002E";
                default:
                    return fileName;
            }
        }

        public string Decode(string fileName)
        {
            string decoded;
            switch (fileName)
            {
                case _escapeChar + "002E":
                    decoded = ".";
                    break;
                case _escapeChar + "002E" + _escapeChar + "002E":
                    decoded = "..";
                    break;
                default:
                    decoded = fileName;
                    break;
            }
            return PostDecode(decoded);
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