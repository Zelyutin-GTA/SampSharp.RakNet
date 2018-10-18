using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class SpectatorSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int lrKey;
        public int udKey;
        public int keys;
        public Vector3 position;

        public SpectatorSync(BitStream bs)
        {
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            this.Read(false);
        }
        public void ReadOutcoming()
        {
            //Spectator does not have outcoming packets
            return;
        }
        public void WriteIncoming()
        {
            this.Write(false);
        }
        public void WriteOutcoming()
        {
            //Spectator does not have outcoming packets
            return;
        }
        private void Read(bool outcoming)
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetID = (int)result["packetID"];
                if (outcoming)
                {
                    this.fromPlayerID = (int)result["fromPlayerID"];
                }

                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);

                this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
            };

            var arguments = new List<object>()
            {
                ParamType.Uint8, "packetID",
                ParamType.Uint16, "lrKey",
                ParamType.Uint16, "udKey",
                ParamType.Uint16, "keys",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",

            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.Uint8, this.packetID,
                ParamType.Uint16, this.lrKey,
                ParamType.Uint16, this.udKey,
                ParamType.Uint16, this.keys,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, this.fromPlayerID);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
