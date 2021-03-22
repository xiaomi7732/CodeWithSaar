using System;

namespace CodeWithSaar.IPC
{
    public class NamedPipeOptions
    {
        /// <summary>
        /// Gets the section name of the named pipe settings for IConfiguration binding.
        /// </summary>
        public const string SectionName = "NamedPipe";

        /// <summary>
        /// Gets or sets the timeout for connection. For server, this is timeout before it gets a connection from the client;
        /// For client, this is timeout before it connects to a server.
        /// Optional. Default to 30 seconds.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the default timeout for sending or receving a message;
        /// Optional. Default to 5 seconds.
        /// </summary>
        public TimeSpan DefaultMessageTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
