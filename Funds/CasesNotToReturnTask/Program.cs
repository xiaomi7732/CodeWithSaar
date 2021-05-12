using System;
using System.IO;
using System.Threading.Tasks;

namespace CasesNotToReturnTask
{
    class Program
    {
        async static Task Main(string[] args)
        {
            Program p = new Program();
            string result = await p.MethodA().ConfigureAwait(false);
            Console.WriteLine($"Final result: {result}");
        }

        #region Where Task Relay is OK
        private async Task<string> MethodA()
        {
            return await MethodB().ConfigureAwait(false);
        }
        #endregion

        #region  Task relay is not OK: Exception will escape

        private async Task<string> MethodB()
        {
            try
            {
                return await GenerateStringAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Handle exception. Message: {ex.Message}");
                return "Something went wrong...";
            }
        }

        #endregion
        private async Task<string> GenerateStringAsync()
        {
            await Task.Delay(200).ConfigureAwait(false);
            throw new InvalidOperationException("Someting is wrong");
        }

        #region Exercise
        // Exercise: What is wrong with this code:
        // private Task WriteText(string input)
        // {
        //     using(StreamWriter sw = new StreamWriter("targetfile.txt"))
        //     {
        //         return sw.WriteAsync(input);
        //     }
        // }
        #endregion
    }
}
