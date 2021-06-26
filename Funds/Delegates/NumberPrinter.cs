using System;

namespace LearnDelegates
{
    class NumberPrinter
    {
        public void Print(int value)
        {
            value *= 2;
            Console.WriteLine(value);
        }
    }
}