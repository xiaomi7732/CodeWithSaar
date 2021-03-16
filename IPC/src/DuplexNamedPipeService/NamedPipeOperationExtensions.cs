using System.Text.Json;
using System.Threading.Tasks;

namespace CodeWithSaar.IPC
{
    public static class NamedPipeOperationExtensions
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = false,
        };

        public static Task SendAsync<T>(this INamedPipeOperations pipe, T obj)
            => pipe.SendMessageAsync(JsonSerializer.Serialize(obj, _options));

        public static async Task<T> ReadAsync<T>(this INamedPipeOperations pipe)
        {
            string json = await pipe.ReadMessageAsync().ConfigureAwait(false);
            if (json is null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}