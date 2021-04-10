using System;

namespace DI.ServiceContainerBasics
{
    public interface IOutputter
    {
        void WriteLine(string value);
    }

    public class ConsoleOutputter : IOutputter
    {
        Guid _id = Guid.NewGuid();
        public void WriteLine(string value)
        {
            Console.WriteLine(_id);
            Console.WriteLine(value);
        }
    }
}