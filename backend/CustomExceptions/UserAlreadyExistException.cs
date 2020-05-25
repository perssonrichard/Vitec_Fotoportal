using System;

namespace Photobox.CustomExceptions
{
    public class UserAlreadyExistException : Exception
    {
        public UserAlreadyExistException () { }
        public UserAlreadyExistException (string message) : base (message) { }
    }
}