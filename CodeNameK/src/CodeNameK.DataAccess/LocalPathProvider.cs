using System.IO;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;

namespace CodeNameK.DAL
{
    internal class LocalPathProvider : PathProviderBase, ILocalPathProvider
    {
        public override string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint, string? baseDirectory = null)
        {
            string dataPointPath = GetLocalPath(dataPoint, baseDirectory);
            return Path.ChangeExtension(dataPointPath, Constants.DeletedMarkerFileExtension);
        }

        protected override string DecodeCategory(string categoryName) => FileUtility.Decode(categoryName);

        protected override string EncodeCategory(string categoryName) => FileUtility.Encode(categoryName);
    }
}