# Custom Logging Provider

## Overview

This is an example of building a Custom ILogger Provider. YouTube videos:

1. [How to implement a custom logging provider for files](https://youtu.be/3RUpYR4dZM4)
2. //TBD

## Project Structure

**MyILoggerLib** is the core project, it contains:

* A custom ILogger: [MyLogger](./MyILoggerLib/MyLogger.cs)
* A logger provider: [MyLoggerProvider](./MyILoggerLib/MyLoggerProvider.cs)
* An extension method class: [MyLoggerExtensions](./MyILoggerLib/MyLoggerExtensions.cs)
* A logger options, which contains a custom settings for output file path: [MyLoggerOptions](./MyILoggerLib/MyLoggerOptions.cs)

**UseCustomILogger** is a WebAPI project to demo the usage of MyLogger.

**ConsoleMyLogger** is a placeholder to use the simplest CustomMyLogger. It is used by the hands-on with tags. There expect to be compile errors in main branch.

## Hands on Lab (video 1)

### Play around the individual ILogger implementation

* Fork this repo if haven't done it before.
* Check out tag: `hands-on-custom-logging-provider`.

    ```shell
    git fetch --all --tags
    git checkout tags/hands-on-custom-logging-provider -b custom-logging-lab
    ```

* Try build, run, updating code under this folder: `CodeNameK\Investigations\CodeNameK.CustomILogger\UseCustomILogger`. Some ideas:

  * Run it directly to see if there log output;
  * Update MyLogger implementation, add a timestamp to each logging entry;
  * Output LogLevel;

* Delete the fork when you are done with it.

## Hands on Lab (video 2)

### Appending user settings

* Fork this repo if haven't done it before.
* Check out tag: `hands-on-custom-logging-provider-2`. Refer the command above.
* Try build, run, updating code under this folder: `CodeNameK\Investigations\CodeNameK.CustomILogger\UseCustomILogger`.
  * If you got it correct, there's going to be logging output in file `AnotherLogFile.log` in the current folder with content like this:

    ```log
    [Microsoft.Hosting.Lifetime, Information] Now listening on: https://localhost:7064
    [Microsoft.Hosting.Lifetime, Information] Now listening on: http://localhost:5175
    [Microsoft.Hosting.Lifetime, Information] Application started. Press Ctrl+C to shut down.
    [Microsoft.Hosting.Lifetime, Information] Hosting environment: Development
    [Microsoft.Hosting.Lifetime, Information] Content root path: C:\YourLocalPath
    ```
* Other ideas to try:
  * Update the code to accept a format string for outputting timestamp. For example, in Program.cs:

    ```csharp
    loggingBuilder.AddFile(opt=>{
        opt.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    });
    ```
    * Expect output log entries like this:
    
    ```log
    [2022-04-02 15:24:44 Microsoft.Hosting.Lifetime, Information] Now listening on: http://localhost:5175
    ```

    * Make it work by specifying the format in `appsettings.json`.
