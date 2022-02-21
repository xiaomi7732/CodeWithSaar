// Common Issues:
// Corrupt file
// Exhausted SLQ connection pool
// Exhausted Sockets
// ...

// using (Stream output = File.OpenWrite("output.txt"))
// using (StreamWriter writer = new StreamWriter(output))
// {
//     writer.WriteLine("Hello");
//     throw new InvalidOperationException("This make it real!");
// }
// using System.Text;

// using (Stream output = File.OpenWrite("output.txt"))
// using (StreamWriter writer = new StreamWriter(output, Encoding.UTF8))
// {
//     for (int i = 0; i < 850; i++)
//     {
//         writer.WriteLine(i);
//     }
// }

using LearnDisposable;

using (MyTextWriter writer = new MyTextWriterV2("output2.txt"))
{
    writer.WriteMessage("Hello");
    writer.WriteMessage("World!");
}
