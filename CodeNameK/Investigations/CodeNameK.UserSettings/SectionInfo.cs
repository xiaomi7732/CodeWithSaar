namespace UserSettingsDemo;

public class SectionInfo<T>
{
    public SectionInfo(string sectionName, T value)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            throw new ArgumentException($"'{nameof(sectionName)}' cannot be null or empty.", nameof(sectionName));
        }
        SectionName = sectionName;
        Value = value;
    }

    public string SectionName { get; }
    public T Value { get; }
}