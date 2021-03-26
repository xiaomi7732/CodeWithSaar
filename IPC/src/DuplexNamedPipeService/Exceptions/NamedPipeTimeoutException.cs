using System;
using System.Runtime.CompilerServices;

namespace CodeWithSaar.IPC
{
    public class NamedPipeTimeoutException : TimeoutException
    {
        public string Operation { get; set; }

        public NamedPipeTimeoutException([CallerMemberName] string methodName = null)
        {
            Operation = methodName;
        }

        public NamedPipeTimeoutException(string message, [CallerMemberName] string methodName = null) : base(message)
        {
            Operation = methodName;
        }

        public NamedPipeTimeoutException(string message, System.Exception inner, [CallerMemberName] string methodName = null) : base(message, inner)
        {
            Operation = methodName;
        }
    }
}