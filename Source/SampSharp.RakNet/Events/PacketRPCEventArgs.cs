using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Events
{
    public class PacketRPCEventArgs : EventArgs
    {
        public int Id;
        public int PlayerId;
        public int BitStreamId;

        public PacketRPCEventArgs(int id, int playerId, int bitStreamId)
        {
            this.Id = id;
            this.PlayerId = playerId;
            this.BitStreamId = bitStreamId;
        }
    }
}
