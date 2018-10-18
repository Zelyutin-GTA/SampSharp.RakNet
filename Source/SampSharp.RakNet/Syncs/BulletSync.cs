using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class BulletSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int hitType;
        public int hitID;
        public Vector3 origin;
        public Vector3 hitPosition;
        public Vector3 offsets;
        public int weaponID;

        public BulletSync(BitStream bs)
        {
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            this.Read(false);
        }
        public void ReadOutcoming()
        {
            this.Read(true);
        }
        public void WriteIncoming()
        {
            this.Write(false);
        }
        public void WriteOutcoming()
        {
            this.Write(true);
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

                hitType = (int)result["hitType"];
                hitID = (int)result["hitId"];
                origin = new Vector3((float)result["origin_0"], (float)result["origin_1"], (float)result["origin_2"]);
                hitPosition = new Vector3((float)result["hitPosition_0"], (float)result["hitPosition_1"], (float)result["hitPosition_2"]);
                offsets = new Vector3((float)result["offsets_0"], (float)result["offsets_1"], (float)result["offsets_2"]);

                weaponID = (int)result["weaponID"];

                this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
            };

            var arguments = new List<object>()
            {
                ParamType.Uint8, "packetID",
                ParamType.Uint8, "hitType",
                ParamType.Uint16, "hitId",
                ParamType.Float, "origin_0",
                ParamType.Float, "origin_1",
                ParamType.Float, "origin_2",
                ParamType.Float, "hitPosition_0",
                ParamType.Float, "hitPosition_1",
                ParamType.Float, "hitPosition_2",
                ParamType.Float, "offsets_0",
                ParamType.Float, "offsets_1",
                ParamType.Float, "offsets_2",
                ParamType.Uint8, "weaponID",
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
                ParamType.Uint8, this.hitType,
                ParamType.Uint16, this.hitID,
                ParamType.Float, this.origin.X,
                ParamType.Float, this.origin.Y,
                ParamType.Float, this.origin.Z,
                ParamType.Float, this.hitPosition.X,
                ParamType.Float, this.hitPosition.Y,
                ParamType.Float, this.hitPosition.Z,
                ParamType.Float, this.offsets.X,
                ParamType.Float, this.offsets.Y,
                ParamType.Float, this.offsets.Z,
                ParamType.Uint8, this.weaponID,
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
