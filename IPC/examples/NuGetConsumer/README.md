# Example for this to work in Hosted Service

## Start the Server

* Run `dotnet run` under `./Server.HostedConsole` and you will see logs like this:

    > info: Server.HostedConsole.GreetingServices[0]  
    Listening on named pipe: e19c036e-0d60-4552-a7be-e8007cecf83c

* Note down (copy) the pipename above.

* Run `dotnet run` under `./Client.Console` with the pipename argument liek this:

  ```shell
  dotnet run e19c036e-0d60-4552-a7be-e8007cecf83c
  ```

  The output will look like:

  > [CLIENT] Try to reach server at pipe: e19c036e-0d60-4552-a7be-e8007cecf83c  
    [CLIENT] Got handshake message from the server: Hi, welcome!

* The client could be run for more than once.
