using System;

namespace CodeWithSaar.IPC
{
    public class NamedPipeOptions
    {
        public const string SectionName = "NamedPipe";

        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
