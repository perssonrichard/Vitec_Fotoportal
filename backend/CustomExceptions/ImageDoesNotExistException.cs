using System;

namespace Photobox.CustomExceptions
{
    public class ImageDoesNotExistException : Exception
    {
        public ImageDoesNotExistException () { }
        public ImageDoesNotExistException (string message) : base (message) { }
    }
}