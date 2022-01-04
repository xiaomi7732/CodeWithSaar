using System.Text.Json;
using System.Threading.Channels;

Channel<string> newChannel = Channel.CreateUnbounded<string>();
using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

_ = Task.Run(async ()=>{
    using(Stream inputStream = File.OpenRead("jobs.json"))
    {
        List<string>? toResume = await JsonSerializer.DeserializeAsync<List<string>>(inputStream);
        if(toResume is not null)
        {
            foreach(string item in toResume)
            {
                await newChannel.Writer.WriteAsync(item);
            }
        }
    }
});

_ = Task.Run(async () =>
{
    while (await newChannel.Reader.WaitToReadAsync(cancellationTokenSource.Token))
    {
        string message = await newChannel.Reader.ReadAsync();
        Console.WriteLine(message);
        await Task.Delay(TimeSpan.FromSeconds(5));
    }
});

Task writing1 = Task.Run(async () => await newChannel.Writer.WriteAsync("Hello Channel!!"));
Task writing2 = Task.Run(async () =>
{
    await newChannel.Writer.WriteAsync("From Thread 2");
    await Task.Delay(TimeSpan.FromSeconds(1));
    await newChannel.Writer.WriteAsync("From Thread 2 again");
});
await Task.WhenAll(writing1, writing1);
newChannel.Writer.Complete();
cancellationTokenSource.Cancel();
List<string> left = new List<string>();
await foreach (string payload in newChannel.Reader.ReadAllAsync())
{
    left.Add(payload);
}
using (Stream outputStream = File.OpenWrite("jobs.json"))
{
    await JsonSerializer.SerializeAsync<List<string>>(outputStream, left).ConfigureAwait(false);
}

await newChannel.Reader.Completion;

// Console.WriteLine("Press any key to continue...");
// Console.ReadKey(intercept: true);
