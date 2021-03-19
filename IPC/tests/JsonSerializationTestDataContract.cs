namespace CodeWithSaars.IPC.Tests
{
    class JsonSerializationTestDataContract
    {
        public string StringValue { get; set; }
        public JsonSerializationTestDataType DataType { get; set; } = JsonSerializationTestDataType.Unknown;
    }
}