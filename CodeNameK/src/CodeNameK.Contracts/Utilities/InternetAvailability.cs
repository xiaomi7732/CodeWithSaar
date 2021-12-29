#define ASSUME_NO_INTERNET
#undef ASSUME_NO_INTERNET

using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CodeNameK.Core.Utilities
{
    public class InternetAvailability
    {
        private readonly HttpClient _httpClient;

        public InternetAvailability(HttpClient httpClient)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(3);
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        public async Task<bool> IsInternetAvailableAsync()
        {
#if ASSUME_NO_INTERNET
                return false;
#endif

            // Fast fail when there's no internet connection
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            // Global
            if (await TryGetNoContentAsync(new Uri("http://www.gstatic.com/generate_204")).ConfigureAwait(false))
            {
                return true;
            }

            // China
            if (await TryGetNoContentAsync(new Uri("http://www.baidu.com")).ConfigureAwait(false))
            {
                return true;
            }

            // Iran
            if (await TryGetNoContentAsync(new Uri("http://www.aparat.com")).ConfigureAwait(false))
            {
                return true;
            }

            return false;
        }

        private async Task<bool> TryGetNoContentAsync(Uri dest)
        {
            try
            {
                HttpResponseMessage respnose = await _httpClient.GetAsync(dest).ConfigureAwait(false);
                return respnose.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}