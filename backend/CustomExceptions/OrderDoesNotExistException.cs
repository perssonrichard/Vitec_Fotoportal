using System;

namespace Photobox.CustomExceptions
{
    public class OrderDoesNotExistException : Exception
    {
        public OrderDoesNotExistException () { }
        public OrderDoesNotExistException (string message) : base (message) { }
    }
}