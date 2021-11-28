using System.Collections.Generic;

namespace CodeWithSaar
{
    public class ReservedFilenameProcessorOptions : IReservedFileNameProcessorOptions
    {
        public IEnumerable<string> ReservedFileNames { get; init; } = new List<string>
        {
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

        public string Escaper { get; init; } = "%";

        public bool EscapeEscaper { get; init; } = false;
    }
}