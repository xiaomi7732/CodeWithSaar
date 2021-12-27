using System.Threading.Channels;

Task? lastDelayTask = Task.CompletedTask;
Channel<int> channel = Channel.CreateUnbounded<int>();
_ = Task.Run(SetupReaderAsync);

await Task.Run(async () =>
{
    await channel.Writer.WriteAsync(100);
    await channel.Writer.WriteAsync(200);
    channel.Writer.Complete();
});

await channel.Reader.Completion;
await lastDelayTask;

async Task SetupReaderAsync()
{
    while (true)
    {
        int result = await channel.Reader.ReadAsync();
        lastDelayTask = Task.Delay(1000);
        await lastDelayTask;
        Console.WriteLine(result);
    }
}