namespace CodeNameK.Contracts
{
    /// <summary>
    /// Data model for user preferences that controls the behavior of the application.
    /// </summary>
    public class UserPreference
    {
        /// <summary>
        /// Gets or sets whether sync is enabled or not.
        /// </summary>
        public bool EnableSync { get; set; }

        /// <summary>
        /// Gets or sets whether to disable sync for this session.
        /// </summary>
        public bool DisableSyncForThisSession { get; set; }
    }
}