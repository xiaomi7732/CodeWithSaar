using System;

namespace ExtensionMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 20;
            x = x * 2;
            int y = x.Double();
            // z = x.Double();
            int z = ExtensionMethods.Double(x);
        }

    }

    static class ExtensionMethods
    {
        public static int Double(this int input)
        {
            return input * 2;
        }
    }
}
