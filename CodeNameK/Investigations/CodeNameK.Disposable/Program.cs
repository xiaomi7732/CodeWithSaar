// Common Issues:
// Corrupt file
// Exhausted SLQ connection pool
// Exhausted Sockets

using System.Diagnostics;
using System.Text;

Console.WriteLine("Process Id: {0}", Process.GetCurrentProcess().Id);
Console.WriteLine("Press any key to continue ...");
Console.ReadKey(intercept: true);

Stream output = File.OpenWrite("output.txt");
StreamWriter writer = new StreamWriter(output);
writer.WriteLine("Hello");

Stream output2 = File.OpenWrite("output2.txt");
StreamWriter writer2 = new StreamWriter(output2, Encoding.UTF8);
for (int i = 0; i < 850; i++)
{
    writer2.WriteLine(i);
}


async Task ProcessFileAsync()
{
    MemoryStream data = new MemoryStream();
    using(Stream fileStream = File.OpenRead(@"C:\tmp\mscordbi.pdb"))
    {
        await fileStream.CopyToAsync(data);
    }
}