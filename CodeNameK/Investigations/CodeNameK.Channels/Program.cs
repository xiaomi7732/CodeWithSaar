using System.Threading.Channels;

Task? lastDelayTask = Task.CompletedTask;
Channel<int> channel = Channel.CreateUnbounded<int>();
int dataValue = 1;

using (CancellationTokenSource cts = new CancellationTokenSource())
{
    _ = Task.Run(async () => await SetupReaderAsync(cts.Token));
    _ = Task.Run(async () =>
    {
        while (true)
        {
            await channel.Writer.WriteAsync(dataValue);
            Console.WriteLine("In channel: {0}", dataValue++);
            await Task.Delay(500);
        }
    }, cts.Token);

    Console.WriteLine("Press any key to start quiting ...");
    Console.ReadKey(intercept: true);
    channel.Writer.Complete();  // Stop the writing
    cts.Cancel();   // Stop the reading and processing;
    await lastDelayTask; // Finish what has been processed.
    Console.WriteLine("Cancel requested. There are {0} items left in the channel:", channel.Reader.Count);
    await foreach (var item in channel.Reader.ReadAllAsync())
    {
        Console.WriteLine(item);
    }
    // Time to persistent the items left
    
}

async Task SetupReaderAsync(CancellationToken cancellationToken)
{
    while (await channel.Reader.WaitToReadAsync())
    {
        int result = await channel.Reader.ReadAsync();
        lastDelayTask = Task.Delay(1000);
        await lastDelayTask;
        Console.WriteLine("Processed: {0}", result);
        cancellationToken.ThrowIfCancellationRequested();
    }

    Console.WriteLine("There are {0} items in the channel.", channel.Reader.Count);
}
