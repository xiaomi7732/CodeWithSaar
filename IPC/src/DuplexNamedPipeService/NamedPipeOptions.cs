using System;

namespace CodeWithSaar.IPC
{
    public class NamedPipeOptions
    {

        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
