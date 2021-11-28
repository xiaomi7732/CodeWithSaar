using System.Collections.Generic;

namespace CodeWithSaar
{
    public interface IReservedFileNameProcessorOptions
    {
        IEnumerable<string> ReservedFileNames { get; }
        string Escaper { get; }

        bool EscapeEscaper { get; }
    }

}