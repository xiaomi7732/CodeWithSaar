namespace CodeWithSaar
{
    public interface ILeadingCharacterProcessorOptions
    {
        string InvalidCharacters { get; }
        string Escaper { get; }
        bool EscapeEscaper { get; }
    }
}