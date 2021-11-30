namespace CodeWithSaar
{
    public class TailingCharacterProcessorOptions : ITailingCharacterProcessorOptions
    {
        public string InvalidCharacters => ". ";

        public string Escaper { get; init; } = "%";

        public bool EscapeEscaper { get; init; } = false;
    }
}