using System;

namespace Photobox.CustomExceptions
{
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException () { }
        public UserDoesNotExistException (string message) : base (message) { }
    }
}