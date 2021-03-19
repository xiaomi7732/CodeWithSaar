using CodeWithSaar.IPC;
using Xunit;

namespace CodeWithSaars.IPC.Tests
{
    public class DefaultSerializationProviderTests
    {
        [Fact]
        public void ShouldSerializeObjectInExpectedWay()
        {
            JsonSerializationTestDataContract payload = new JsonSerializationTestDataContract()
            {
                StringValue = "Hello XUniT!"
            };
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canSerialize = target.TrySerialize(payload, out string actual);
            Assert.True(canSerialize);

            // Expectations:
            // 1. Property names are camel cased;
            // 2. Case in values are persisted;
            // 3. No indentation or newline;
            // 4. Enum is serialized to string;
            string expected = @"{""stringValue"":""Hello XUniT!"",""dataType"":""Unknown""}";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldDeserializePropertyCaseInSensitive()
        {
            string serialized = @"{""STRINGVALUE"":""Hello XUniT!"",""dataType"":""Unknown""}";
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.True(canDeserialize);

            Assert.Equal("Hello XUniT!", actual.StringValue);
        }

        [Fact]
        public void ShouldDeserializeEnumFromString()
        {
            string serialized = @"{""stringValue"":""Hello XUniT!"",""dataType"":""Type1""}";
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.True(canDeserialize);

            Assert.Equal(JsonSerializationTestDataType.Type1, actual.DataType);
        }

        [Fact]
        public void ShouldDeserializeEnumFromStringCaseInsensitive()
        {
            string serialized = @"{""stringValue"":""Hello XUniT!"",""dataType"":""type1""}";
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.True(canDeserialize);

            Assert.Equal(JsonSerializationTestDataType.Type1, actual.DataType);
        }

        [Fact]
        public void ShouldNotDeserializeNull()
        {
            string serialized = null;
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.False(canDeserialize);
            Assert.Null(actual);
        }

        [Fact]
        public void ShouldNotSerializeNull()
        {
            JsonSerializationTestDataContract payload = null;
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canSerialize = target.TrySerialize<JsonSerializationTestDataContract>(payload, out string actual);
            Assert.False(canSerialize);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData(@"{""stringValue"":256,""dataType"":""type1""}")]       // 256 is a number, allowed in value: how to reach this?
        [InlineData(@"{""stringValue"":""256"",""dataType"":1}")]               // 1 is a enum value, but it isn't in string format.
        public void ShouldAllowDeserializeCompatibleConverts(string serialized)
        {
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.True(canDeserialize);
        }

        [Theory]
        [InlineData("abc")] // Not a json
        [InlineData(@"{""stringValue"":""Hello XUniT!""")] // Malformat json
        public void ShouldNotThrowDeserializing(string serialized)
        {
            ISerializationProvider target = new DefaultSerializationProvider();
            bool canDeserialize = target.TryDeserialize<JsonSerializationTestDataContract>(serialized, out JsonSerializationTestDataContract actual);
            Assert.False(canDeserialize);
        }
    }
}
