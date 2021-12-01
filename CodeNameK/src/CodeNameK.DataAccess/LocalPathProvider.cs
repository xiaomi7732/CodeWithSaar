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

        public LocalPathProvider(IOptions<LocalStoreOptions> options)
        {
            _options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
        }

        public override string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint, string? baseDirectory = null)
        {
            string dataPointPath = GetLocalPath(dataPoint, baseDirectory);
            return Path.ChangeExtension(dataPointPath, Constants.DeletedMarkerFileExtension);
        }

        public override bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo, string? localStoreBasePath = null)
        {
            string dataPointPath = GetLocalPath(dataPointPathInfo, localStoreBasePath);
            return File.Exists(dataPointPath);
        }

        protected override string DecodeCategory(string categoryName) => FileUtility.Decode(categoryName);
        protected override string EncodeCategory(string categoryName) => FileUtility.Encode(categoryName);

        protected override string DecodePath(string path) => path;
        protected override string EncodePath(string path) => path;
    }
}