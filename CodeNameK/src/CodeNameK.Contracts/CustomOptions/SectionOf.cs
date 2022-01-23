namespace CodeNameK.Contracts.CustomOptions
{
    /// <summary>
    /// A section of a model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record SectionOf<T>
    {
        /// <summary>
        /// Gets the section name;
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the object
        /// </summary>
        /// <value></value>
        public T Value { get; }

        /// <summary>
        /// Creates a section
        /// </summary>
        /// <param name="name">The name for the section.</param>
        /// <param name="value">The content.</param>
        public SectionOf(string name, T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            }

            Name = name;
            Value = value;
        }
    }
}