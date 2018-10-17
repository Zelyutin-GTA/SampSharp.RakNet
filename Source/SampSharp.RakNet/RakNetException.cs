using System;

namespace SampSharp.RakNet
{
    class RakNetException : Exception
    {
        public RakNetException(string message) : base(message, null) { }
    }
}
