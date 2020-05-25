using System;

namespace Photobox.CustomExceptions
{
    public class ImageTooSmallException : Exception
    {
        public ImageTooSmallException () { }
        public ImageTooSmallException (string message) : base (message) { }
    }
}