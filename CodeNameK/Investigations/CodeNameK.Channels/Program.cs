using System.Text.Json;
using System.Threading.Channels;

Task? lastDelayTask = Task.CompletedTask;
Channel<int> channel = Channel.CreateUnbounded<int>();
int dataValue = 1;

if (File.Exists("messages.json"))
{
    Console.WriteLine("There are messages left from the last session. Restore them.");
    using (Stream inputStream = File.OpenRead("messages.json"))
    {
        List<int>? leftOver = await JsonSerializer.DeserializeAsync<List<int>>(inputStream).ConfigureAwait(false);
        if (leftOver is not null)
        {
            foreach (int item in leftOver)
            {
                await channel.Writer.WriteAsync(item).ConfigureAwait(false);
            }
            Console.WriteLine("{0} items restored from the last session", leftOver?.Count);
        }
    }
}

using (CancellationTokenSource cts = new CancellationTokenSource())
{
    _ = Task.Run(async () => await SetupReaderAsync(cts.Token));
    _ = Task.Run(async () => await SetupWriterAsync(cts.Token));

    Console.WriteLine("Press any key to start quiting ...");
    Console.ReadKey(intercept: true);

    channel.Writer.Complete();  // Stop the writing
    cts.Cancel();   // Stop the reading and processing;
    await lastDelayTask; // Finish what has been processed.
    Console.WriteLine("Cancel requested. There are {0} items left in the channel:", channel.Reader.Count);

    // Time to persistent the items left
    List<int> messages = new List<int>();
    await foreach (int item in channel.Reader.ReadAllAsync())
    {
        messages.Add(item);
    }
    using (Stream output = File.OpenWrite("messages.json"))
    {
        await JsonSerializer.SerializeAsync(output, messages);
    }

    Console.ReadKey(intercept: true);
}

async Task SetupWriterAsync(CancellationToken cancellationToken)
{
    while (await channel.Writer.WaitToWriteAsync())
    {
        await channel.Writer.WriteAsync(dataValue);
        Console.WriteLine("In channel: {0}", dataValue++);
        await Task.Delay(500);
    }
}

async Task SetupReaderAsync(CancellationToken cancellationToken)
{
    while (await channel.Reader.WaitToReadAsync(cancellationToken))
    {
        int result = await channel.Reader.ReadAsync(cancellationToken);
        lastDelayTask = Task.Delay(1000);
        await lastDelayTask;
        Console.WriteLine("Processed: {0}", result);
    }

    Console.WriteLine("There are {0} items in the channel.", channel.Reader.Count);
}
