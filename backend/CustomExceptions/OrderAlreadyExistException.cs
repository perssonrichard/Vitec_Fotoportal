using System;

namespace Photobox.CustomExceptions
{
    public class OrderAlreadyExistException : Exception
    {
        public OrderAlreadyExistException () { }
        public OrderAlreadyExistException (string message) : base (message) { }
    }
}