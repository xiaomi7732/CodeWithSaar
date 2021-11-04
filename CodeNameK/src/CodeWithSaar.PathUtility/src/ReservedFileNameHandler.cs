using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeWithSaar
{
    /// <summary>
    /// Do not use the following reserved names for the name of a file:
    /// CON, PRN, AUX, NUL, COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, LPT1, LPT2, LPT3, LPT4, LPT5, LPT6, LPT7, LPT8, and LPT9.
    /// Also avoid these names followed immediately by an extension; for example, NUL.txt is not recommended.
    /// </summary>
    public class ReservedFileNameHandler : IFileNameProcessor
    {
        private readonly char _escapeChar;

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

        private readonly Regex _decoderRegex;
        private readonly Regex _encodeRegex;

        public ReservedFileNameHandler(char? escapeCharacter = null)
        {
            _escapeChar = escapeCharacter ?? '%';

            StringBuilder patternBuilder = new StringBuilder();
            patternBuilder.Append("^" + _escapeChar + "(");
            foreach (string reservedName in _reservedNames)
            {
                patternBuilder.AppendFormat("(?:{0})|", reservedName);
            }
            patternBuilder.Remove(patternBuilder.Length - 1, 1);
            patternBuilder.Append(")$");
            _decoderRegex = new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

            patternBuilder.Clear();
            patternBuilder.Append("^(");
            foreach (string reservedName in _reservedNames)
            {
                patternBuilder.AppendFormat("(?:{0})|", reservedName);
            }
            patternBuilder.Remove(patternBuilder.Length - 1, 1);
            patternBuilder.Append(")$");
            _encodeRegex = new Regex(patternBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Decodes a folder name to its original form. For example, if the folder name was encoded to '%CON' from 'CON', the return value would be 'CON'.
        /// </summary>
        public string Decode(string safeFileName)
        {
            string filePath = DoDecode(safeFileName);
            return filePath.Replace(_escapeChar.ToString() + _escapeChar, _escapeChar.ToString());
        }

        /// <summary>
        /// Encode a file name to avoid using reserved file names.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string Encode(string filePath)
        {
            filePath = PreEncode(filePath);
            return DoEncode(filePath);
        }

        private string PreEncode(string filePath)
        {
            // Escape the escape character by doubling itself
            return filePath.Replace(_escapeChar.ToString(), _escapeChar.ToString() + _escapeChar);
        }

        private string DoDecode(string safeFileName)
        {
            string fileName = Path.GetFileName(safeFileName);
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(safeFileName);
            string fileExtension = fileName.Substring(fileNameNoExtension.Length);
            string decodedFileName = fileNameNoExtension;
            if (_decoderRegex.IsMatch(decodedFileName))
            {
                decodedFileName = _decoderRegex.Replace(decodedFileName, match => match.Groups[1].Value);
            }

            string parent = safeFileName.Substring(0, safeFileName.Length - fileName.Length);
            string decodedParent = parent;
            if (parent.Length > 1)
            {
                decodedParent = DoDecode(decodedParent.Substring(0, parent.Length - 1));
                decodedParent += parent.Last();
            }

            return decodedParent + decodedFileName + fileExtension;
        }

        private string DoEncode(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = fileName.Substring(fileNameNoExtension.Length);

            string encodedFileName = fileNameNoExtension;
            if (_encodeRegex.IsMatch(encodedFileName))
            {
                encodedFileName = _encodeRegex.Replace(encodedFileName, match => _escapeChar + match.Value);
            }

            string directory = filePath.Substring(0, filePath.Length - fileName.Length);
            string encodedDirectory = directory;
            if (directory.Length > 1)
            {
                encodedDirectory = DoEncode(directory.Substring(0, directory.Length - 1));
                encodedDirectory += directory.Last();
            }

            return encodedDirectory + encodedFileName + fileExtension;
        }
    }
}