using System.Collections.Generic;
using System.Linq;

namespace CodeWithSaar
{
    public static class FileUtility
    {
        static IEnumerable<IFileNameProcessor> _processors = new List<IFileNameProcessor>(){
            new ReservedCharacterProcessor(),
            new ReservedFileNameProcessor(escapeEscaper: false),
            new TailingCharacterProcessor(escapeEscaper: false),
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