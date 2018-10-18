using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class SpectatorSync : ISync
    {
        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int LRKey { get; set; }
        public int UDKey { get; set; }
        public int Keys { get; set; }
        public Vector3 position { get; set; }

        public SpectatorSync(BitStream bs)
        {
            BS = bs;
        }
        public void ReadIncoming()
        {
            Read(false);
        }
        public void ReadOutcoming()
        {
            //Spectator does not have outcoming packets
            return;
        }
        public void WriteIncoming()
        {
            Write(false);
        }
        public void WriteOutcoming()
        {
            //Spectator does not have outcoming packets
            return;
        }
        private void Read(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "lrKey",
                ParamType.UInt16, "udKey",
                ParamType.UInt16, "keys",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",

            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, "fromPlayerId");
            }

            var result = BS.ReadValue(arguments.ToArray());

            PacketId = (int)result["packetId"];
            if (outcoming)
            {
                FromPlayerId = (int)result["fromPlayerId"];
            }

            LRKey = (int)result["lrKey"];
            UDKey = (int)result["udKey"];
            Keys = (int)result["keys"];
            position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, PacketId,
                ParamType.UInt16, LRKey,
                ParamType.UInt16, UDKey,
                ParamType.UInt16, Keys,
                ParamType.Float, position.X,
                ParamType.Float, position.Y,
                ParamType.Float, position.Z
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
