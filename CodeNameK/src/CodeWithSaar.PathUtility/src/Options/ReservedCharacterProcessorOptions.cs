namespace CodeWithSaar
{
    public class ReservedCharacterProcessorOptions : IReservedCharacterProcessorOptions
    {
        public string InvalidCharacters { get; init; } = "<>:\"/\\|?*";
        public string Escaper { get; init; } = "%";
    }
}