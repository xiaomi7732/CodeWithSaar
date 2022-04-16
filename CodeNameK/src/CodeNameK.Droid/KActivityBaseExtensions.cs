namespace CodeNameK.Droid
{
    internal static class KActivityBaseExtensions
    {
        private const string keyPrefix = "net.codewithsaar.numberit";
        public static string MakeIntentKeyForApp(this KActivityBase activity, string value)
        {
            return $"{keyPrefix}.{value}";
        }
    }
}