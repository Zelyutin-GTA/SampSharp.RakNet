using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Events
{
    public class PacketRPCEventArgs : EventArgs
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BitStreamId { get; set; }

        public PacketRPCEventArgs(int id, int playerId, int bitStreamId)
        {
            this.Id = id;
            this.PlayerId = playerId;
            this.BitStreamId = bitStreamId;
        }
    }
}
