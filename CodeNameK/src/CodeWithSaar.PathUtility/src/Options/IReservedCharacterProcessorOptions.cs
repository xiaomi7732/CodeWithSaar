namespace CodeWithSaar
{
    public interface IReservedCharacterProcessorOptions
    {
        string InvalidCharacters { get; }
        string Escaper { get; }
    }
}