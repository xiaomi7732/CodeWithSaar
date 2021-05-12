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
            string result = await p.PassOnTheWords().ConfigureAwait(false);
            Console.WriteLine($"Final result: {result}");
        }

        #region Task Relay is OK
        private async Task<string> PassOnTheWords()
        {
            Console.WriteLine($"In {nameof(PassOnTheWords)}");
            return await ExceptionHandler().ConfigureAwait(false);
        }

        // private Task<string> PassOnTheWords()
        // {
        //     Console.WriteLine($"In {nameof(PassOnTheWords)}");
        //     return ExceptionHandler();
        // }
        #endregion

        #region  Task relay is not OK: Exception will escape

        private async Task<string> ExceptionHandler()
        {
            try
            {
                Console.WriteLine($"In {nameof(ExceptionHandler)}");
                return await GenerateString().ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Handle exception. Message: {ex.Message}");
                return "Something went wrong...";
            }
        }

        // The exception will escape the handler
        // private Task<string> ExceptionHandler()
        // {
        //     try
        //     {
        //         Console.WriteLine($"In {nameof(ExceptionHandler)}");
        //         return GenerateString();
        //     }
        //     catch (InvalidOperationException ex)
        //     {
        //         Console.WriteLine(ex);
        //         return Task.FromResult("NO VALID STRING...");
        //     }
        // }
        #endregion

        #region Exercise
        private async Task<string> GenerateString()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("Someting is wrong");
        }

        // Exercise: What is wrong with this code:
        // private Task WriteText(string input)
        // {
        //     using(StreamWriter sw = new StreamWriter("targetfile.txt"))
        //     {
        //         return sw.WriteAsync(input);
        //     }
        // }
    }
    #endregion

}
