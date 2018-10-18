using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Events
{
    public class PacketRpcEventArgs : EventArgs
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int BitStreamId { get; set; }

        public PacketRpcEventArgs(int id, int playerId, int bitStreamId)
        {
            Id = id;
            PlayerId = playerId;
            BitStreamId = bitStreamId;
        }
    }
}
