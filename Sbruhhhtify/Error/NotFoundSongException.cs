using System;

namespace Sbruhhhtify.Error
{
    public class NotFoundSongException : Exception
    {
        private static readonly string message = "LMAO1223: Song not found";
        public NotFoundSongException() : base(message) { }

        public override string ToString()
        {
            return message;
        }
    }
}
