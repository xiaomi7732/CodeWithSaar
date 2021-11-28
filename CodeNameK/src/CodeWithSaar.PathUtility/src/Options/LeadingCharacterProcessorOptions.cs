namespace CodeWithSaar
{
    public class LeadingCharacterProcessorOptions : ILeadingCharacterProcessorOptions
    {
        public string InvalidCharacters { get; init; } = "~$";

        public string Escaper { get; init; } = "%";

        public bool EscapeEscaper { get; init; } = true;
    }
}