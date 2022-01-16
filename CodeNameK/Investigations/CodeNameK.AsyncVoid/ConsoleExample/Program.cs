async void AsyncJob()
{
    await Task.Delay(100);
    throw new InvalidOperationException("Hello exception!");
    // Console.WriteLine("Job is done!");
}

try
{
    // Notice: the exception thrown by AsyncJob won't be 
    // caught by this try catch.
    // It will crash the process instead.
    AsyncJob();
}
catch (Exception)
{
    Console.WriteLine("Friendly error.");
}
Console.WriteLine("Hello from main!");
Console.ReadKey(intercept: true);
