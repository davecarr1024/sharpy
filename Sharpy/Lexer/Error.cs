using System;

namespace Sharpy.Lexer
{
    public class Error : Exception
    {
        public Location Location { get; }

        public Error(string message, Location location)
            : base($"{message} at {location}")
            => Location = location;
    }
}
