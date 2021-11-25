using System.Text.Json;
using CodeNameK.Serializations;

DataPoint dataPoint = new DataPoint()
{
    Id = Guid.NewGuid(),
    WhenUTC = DateTime.UtcNow,
    Value = 2000,
};

using (Stream output = File.OpenWrite("data.psv"))
using (StreamWriter writer = new StreamWriter(output))
{
    await writer.WriteAsync($"{dataPoint.Id:N}|{dataPoint.WhenUTC:O}|{dataPoint.Value}").ConfigureAwait(false);
}

using (Stream output = File.OpenWrite("data.json"))
{
    await JsonSerializer.SerializeAsync(output, dataPoint).ConfigureAwait(false);
}

using (Stream input = File.OpenRead("data.psv"))
using (StreamReader reader = new StreamReader(input))
{
    string allText = await reader.ReadToEndAsync().ConfigureAwait(false);
    string[] tokens = allText.Split("|");
    DataPoint newDataPoint = new DataPoint()
    {
        Id = Guid.Parse(tokens[0]),
        // WhenUTC = DateTime.Parse(tokens[1]),
        WhenUTC = DateTime.Parse(tokens[1]).ToUniversalTime(),
        Value = int.Parse(tokens[2]),
    };
    Console.WriteLine("Deserialized from psv: {0}", newDataPoint);
}

using (Stream inputJson = File.OpenRead("data.json"))
{
    DataPoint? newDataPoint = await JsonSerializer.DeserializeAsync<DataPoint>(inputJson).ConfigureAwait(false);
    Console.WriteLine("Deserialized from json: {0}", newDataPoint);
}
