using System;

namespace DI.ConsoleApp
{
    public interface IOutputer
    {
        void WriteLine(string value);
    }

    public class ConsoleOutputer : IOutputer
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }
    }
}