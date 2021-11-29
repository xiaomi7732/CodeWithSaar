
using System;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DAL.OneDrive;
using CodeNameK.DataContracts;
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

            ILocalPathProvider target = new PathProvider();
            string actual = target.GetRelativePath(dataPoint, null);

            string expected = $"{category}/{utcNow.Year}/{utcNow.Month:00}/{dataPointId:D}.dpt";

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

            DataPointPathService target = new DataPointPathService();
            
            string actual = target.GetRelativePath(dataPoint, basePath).Replace("\\", "/"); // Normalize separator for various OS.
            string expected = $"{basePath}/{category}/{utcNow.Year}/{utcNow.Month:00}/{dataPointId:D}.dpt";
            
            Assert.Equal(expected, actual);
        }
    }
}