using System;

namespace CodeNameK.Core.CustomExceptions
{
    public class SigninTimeoutException : Exception
    {
        public SigninTimeoutException(TimeSpan timeout) : base($"Sign in timeout after {timeout:c}.")
        {
        }
    }
}
