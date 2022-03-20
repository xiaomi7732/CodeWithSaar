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

* Try build, run, updating code. Some ideas:

  * Run it directly to see if there log output;
  * Update MyLogger implementation, add a timestamp to each logging entry;
  * Output LogLevel;

* Delete the fork when you are done with it.
