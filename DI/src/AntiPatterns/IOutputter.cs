using System;

namespace DI.ServiceContainerBasics
{
    public interface IOutputter : IDisposable
    {
        void WriteLine(string value);
    }

    public class ConsoleOutputter : IOutputter
    {
        Guid _id = Guid.NewGuid();

        public void Dispose()
        {
            Console.WriteLine($"{nameof(IOutputter)} Dispose is called.");
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(_id);
            Console.WriteLine(value);
        }
    }
}