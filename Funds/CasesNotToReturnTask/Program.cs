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
            // Why unhandled InvalidOperationException?
            string result = await p.MethodARelayBAsync().ConfigureAwait(false);
            Console.WriteLine($"Final result: {result}");
        }

        #region Where Task Relay is OK
        // private async Task<string> MethodAAsync()
        // {
        //     return await MethodBAsync().ConfigureAwait(false);
        // }

        private Task<string> MethodARelayBAsync()
        {
            return MethodBAsync();
        }
        #endregion

        #region  Task relay is not OK: Exception will escape

        private Task<string> MethodBAsync()
        {
            try
            {
                return GenerateStringAsync();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Handle exception. Message: {ex.Message}");
                return Task.FromResult("Something went wrong...");
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
        private Task WriteText(string input)
        {
            using (StreamWriter sw = new StreamWriter("targetfile.txt"))
            {
                return sw.WriteAsync(input);
            }
        }
        #endregion
    }
}
