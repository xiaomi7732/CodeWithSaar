namespace CodeNameK.Contracts.CustomOptions
{
    public class LocalStoreOptions
    {
        public const string SectionName = "LocalStore";

        public string DataStorePath { get; set; } = "%userprofile%/.codeNameK/Data";
    }
}