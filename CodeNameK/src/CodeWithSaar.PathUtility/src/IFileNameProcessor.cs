namespace CodeWithSaar
{
    public interface IFileNameProcessor
    {
        /// <summary>
        /// Encode a file path, so that it would be safe to be used as a file path.
        /// </summary>
        string Encode(string fileName);

        /// <summary>
        /// Decodes a file path to be used as a plain string.
        /// </summary>
        string Decode(string fileName);
    }
}