
using System;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Options;
using Xunit;

namespace CodeNameK.DAL.UnitTests
{
    public class DataPointPathServiceTests
    {
        [Fact]
        public void ShouldGetProperRelativePath()
        {
            Guid dataPointId = Guid.NewGuid();
            DateTime utcNow = DateTime.UtcNow;
            string category = "TestCategory";
            DataPoint dataPoint = new DataPoint()
            {
                Id = dataPointId,
                WhenUTC = utcNow,
                Value = 1000,
                Category = new Category()
                {
                    Id = category,
                },
            };

            LocalStoreOptions options = new LocalStoreOptions() { 
                DataStorePath = String.Empty,
            };
            ILocalPathProvider target = new LocalPathProvider(Options.Create(options));
            string actual = target.GetLocalPath(dataPoint).Replace("\\", "/");
            string expected = $"{category}/{dataPointId:D}.dpt";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldGetProperRelativePathWithBasePath()
        {
            Guid dataPointId = Guid.NewGuid();
            DateTime utcNow = DateTime.UtcNow;
            string category = "TestCategory";
            string basePath = "basePath";
            DataPoint dataPoint = new DataPoint()
            {
                Id = dataPointId,
                WhenUTC = utcNow,
                Value = 1000,
                Category = new Category()
                {
                    Id = category,
                },
            };

            LocalStoreOptions options = new LocalStoreOptions()
            {
                DataStorePath = basePath,
            };
            ILocalPathProvider target = new LocalPathProvider(Options.Create(options));
            string actual = target.GetLocalPath(dataPoint).Replace("\\", "/"); // Normalize separator for various OS.
            string expected = $"{basePath}/{category}/{dataPointId:D}.dpt";
            
            Assert.Equal(expected, actual);
        }
    }
}