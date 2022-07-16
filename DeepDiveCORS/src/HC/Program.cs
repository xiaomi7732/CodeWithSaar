using (HttpClient httpClient = new HttpClient())
{
    try
    {
        Uri requestUri = new Uri("https://localhost:8080/weatherforecast");
        Console.WriteLine("Sending request to: {0}", requestUri.AbsoluteUri);
        HttpResponseMessage response = await httpClient.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Got successful result:");
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
    catch
    {
        Console.WriteLine("Failed connection. Did you started the backend at https://localhost:8080 already?");
    }
}