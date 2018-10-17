using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Events
{
    public class PacketRPCEventArgs : EventArgs
    {
        public int ID;
        public int PlayerID;
        public int BitStreamID;

        public PacketRPCEventArgs(int id, int playerID, int bitStreamID)
        {
            this.ID = id;
            this.PlayerID = playerID;
            this.BitStreamID = bitStreamID;
        }
    }
}
