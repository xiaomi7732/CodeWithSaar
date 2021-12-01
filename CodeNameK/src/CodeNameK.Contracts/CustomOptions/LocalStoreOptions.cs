using System;

namespace CodeNameK.Contracts.CustomOptions
{
    public class LocalStoreOptions
    {
        public const string SectionName = "LocalStore";

        private string _dataStorePath = Environment.ExpandEnvironmentVariables("%userprofile%/.codeNameK/Data");
        public string DataStorePath
        {
            get
            {
                return _dataStorePath;
            }
            set
            {
                _dataStorePath = Environment.ExpandEnvironmentVariables(value);
            }
        }
    }
}