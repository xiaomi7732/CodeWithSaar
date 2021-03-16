# Two-way named pipe stream wrapper

## Description

This is a simple warper around named-pipe for two-way communication.

* **src/DuplexNamedPipeService** contains the implementations.
* **src/Example.Server** spins up a named pipe server;
  * [Program.cs](./src/Example.Server/Program.cs)
* **src/Example.Client** spins up a named pipe client;
  * [Program.cs](./src/Example.Client/Program.cs)
There are string transmitting from either side; there are serialized object transfer too.

## To run the example

Start the server in a console:

```shell
dotnet run src/Example.Server
```

The server will start and wait for clients to connect.

Then, in another console, run:

```shell
dotnet run src/Example.Client
```

The client will connect to the server and unlock the flow.

## Limitation

For the simplicity, EOL is used as separator for messages; Messages with EOL in it requires escaping to work.
