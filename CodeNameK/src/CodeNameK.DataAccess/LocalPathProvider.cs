using System.Collections.Generic;
using System.IO;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;
using Microsoft.Extensions.Options;

namespace CodeNameK.DAL
{
    internal class LocalPathProvider : PathProviderBase, ILocalPathProvider
    {
        private readonly LocalStoreOptions _options;
        public override string BasePath => _options.DataStorePath;

        public LocalPathProvider(IOptions<LocalStoreOptions> options)
        {
            _options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
        }

        public override bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo)
        {
            string dataPointPath = GetLocalPath(dataPointPathInfo);
            return File.Exists(dataPointPath);
        }

        protected override string DecodeCategory(string categoryName) => FileUtility.Decode(categoryName);
        protected override string EncodeCategory(string categoryName) => FileUtility.Encode(categoryName);

        protected override string DecodePath(string path) => path;
        protected override string EncodePath(string path) => path;

        public override IEnumerable<DataPointPathInfo> ListAllDataPointPaths()
        {
            if (!Directory.Exists(_options.DataStorePath))
            {
                yield break;
            }

            foreach (string filePath in Directory.EnumerateFiles(_options.DataStorePath, "*", new EnumerationOptions() { RecurseSubdirectories = true }))
            {
                if (!TryGetDataPointInfo(filePath, out DataPointPathInfo? pathInfo))
                {
                    continue;
                }
                yield return pathInfo!;
            }
        }
    }
}