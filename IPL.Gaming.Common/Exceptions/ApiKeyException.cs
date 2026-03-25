using System;
using System.Globalization;

namespace IPL.Gaming.Common.Exceptions
{
    public class ApiKeyException : Exception
    {
        public ApiKeyException() : base() { }

        public ApiKeyException(string message) : base(message) { }

        public ApiKeyException(string message, params object[] args) 
            : base(string.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
