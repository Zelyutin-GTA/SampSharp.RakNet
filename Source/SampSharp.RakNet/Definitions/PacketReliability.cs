using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Definitions
{
    public enum PacketReliability
    {
        UNRELIABLE = 6,
        UNRELIABLE_SEQUENCED,
        RELIABLE,
        RELIABLE_ORDERED,
        RELIABLE_SEQUENCED
    };
}
