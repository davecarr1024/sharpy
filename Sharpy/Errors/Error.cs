using System;

namespace Sharpy.Errors
{
    public class Error : Exception
    {
        public Lexer.Location? Location { get; }

        public Error(string message, Lexer.Location? location = null, Error inner_error = null) : base(message, inner_error)
        {
            Location = location;
        }

        public override bool Equals(object obj) 
            => obj is Error rhs && Location.Equals(rhs.Location) && Message == rhs.Message && 
                (InnerException is null || InnerException.Equals(rhs.InnerException));

        public override int GetHashCode() => HashCode.Combine(Location, Message, InnerException);
    }
}