using System;

namespace DI.ServiceContainerBasics
{
    public interface IOutputter
    {
        void WriteLine(string value);
    }

    public class ConsoleOutputter : IOutputter
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}