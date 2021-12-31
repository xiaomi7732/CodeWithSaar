using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar
{
    public static class OneDriveFileUtility
    {
        private static readonly List<string> reservedFileNames = new List<string>(){
            ".lock", "CON", "PRN", "AUX", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "_vti_", "desktop.ini"
        };

        static IEnumerable<IFileNameProcessor> _processors = new List<IFileNameProcessor>(){
            new ReservedCharacterProcessor(new ReservedCharacterProcessorOptions(){ InvalidCharacters = @"""*:<>?/\|", Escaper="_" }),
            new ReservedFileNameProcessor(new ReservedFilenameProcessorOptions() { ReservedFileNames = reservedFileNames, EscapeEscaper = false, Escaper="_"}),
            new TailingCharacterProcessor(new TailingCharacterProcessorOptions() { EscapeEscaper = false, Escaper="_" }),
            new LeadingCharacterProcessor(new LeadingCharacterProcessorOptions(){ EscapeEscaper = false, Escaper="_" }),
        };

        public static string Encode(string fileName)
        {
            foreach (IFileNameProcessor processor in _processors)
            {
                fileName = processor.Encode(fileName);
            }
            return fileName;
        }
        public static string Decode(string fileName)
        {
            foreach (IFileNameProcessor processor in _processors.Reverse())
            {
                fileName = processor.Decode(fileName);
            }
            return fileName;
        }
    }
}