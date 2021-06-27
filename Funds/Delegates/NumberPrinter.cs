using System;

namespace LearnDelegates
{
    class NumberPrinter
    {
        public void Print(int number, Func<int, int> preprocessor)
        {
            number = preprocessor(number);
            Console.WriteLine(number);
        }
    }
}