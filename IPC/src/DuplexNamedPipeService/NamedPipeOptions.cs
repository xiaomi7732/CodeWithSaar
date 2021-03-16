using System;

namespace CodeWithSaar.IPC
{
    public class NamedPipeOptions
    {
        public string PipeName { get; set; }

        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
