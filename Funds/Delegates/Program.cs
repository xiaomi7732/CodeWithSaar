using System;

namespace LearnDelegates
{
    class Program
    {
        // private delegate void MyFirstDelegate(int value);

        private static Action<int> _myFirstDelegateInstance;
        static void Main(string[] args)
        {
            _myFirstDelegateInstance = TestMethod;
            _myFirstDelegateInstance = delegate (int value)
            {
                Console.WriteLine("With in anonymous method: {0}", value);
            };
            _myFirstDelegateInstance = (value) =>
            {
                Console.WriteLine("With in lambda expression: {0}", value);
            };
            _myFirstDelegateInstance(200);

            NumberPrinter numberPrinter = new NumberPrinter();
            Console.WriteLine("Double the number:");
            numberPrinter.Print(100, (value) => value * 2);

            Console.WriteLine("Half the number:");
            numberPrinter.Print(100, (value) => value / 2);
        }

        private static void TestMethod(int value)
        {
            Console.WriteLine(value);
        }
    }
}
