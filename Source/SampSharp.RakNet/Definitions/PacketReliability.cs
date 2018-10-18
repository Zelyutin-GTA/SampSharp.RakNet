using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Definitions
{
    public enum PacketReliability
    {
        Unreliable = 6,
        UnreliableSequenced,
        Reliable,
        ReliableOrdered,
        ReliableSequenced
    };
}
