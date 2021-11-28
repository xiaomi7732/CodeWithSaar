namespace CodeWithSaar
{
    public interface ITailingCharacterProcessorOptions
    {
        string InvalidCharacters { get; }
        string Escaper { get; }
        bool EscapeEscaper { get; }
    }
}