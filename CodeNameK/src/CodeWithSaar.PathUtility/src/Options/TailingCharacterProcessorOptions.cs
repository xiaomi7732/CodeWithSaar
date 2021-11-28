namespace CodeWithSaar
{
    public class TailingCharacterProcessorOptions : ITailingCharacterProcessorOptions
    {
        public string InvalidCharacters => ". ";

        public string Escaper => "%";

        public bool EscapeEscaper { get; init; } = false;
    }
}