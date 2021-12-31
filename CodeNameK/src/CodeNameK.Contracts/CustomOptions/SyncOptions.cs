using System;

namespace CodeNameK.Contracts.CustomOptions
{
    public class SyncOptions
    {
        public const string SectionName = "Sync";
        public TimeSpan SignInTimeout { get; set; } = TimeSpan.FromMinutes(2);
    }
}