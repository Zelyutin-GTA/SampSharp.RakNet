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
                ParamType.UINT8, "packetID",
                ParamType.UINT8, "hitType",
                ParamType.UINT16, "hitId",
                ParamType.FLOAT, "origin_0",
                ParamType.FLOAT, "origin_1",
                ParamType.FLOAT, "origin_2",
                ParamType.FLOAT, "hitPosition_0",
                ParamType.FLOAT, "hitPosition_1",
                ParamType.FLOAT, "hitPosition_2",
                ParamType.FLOAT, "offsets_0",
                ParamType.FLOAT, "offsets_1",
                ParamType.FLOAT, "offsets_2",
                ParamType.UINT8, "weaponID",
            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UINT8, this.packetID,
                ParamType.UINT8, this.hitType,
                ParamType.UINT16, this.hitID,
                ParamType.FLOAT, this.origin.X,
                ParamType.FLOAT, this.origin.Y,
                ParamType.FLOAT, this.origin.Z,
                ParamType.FLOAT, this.hitPosition.X,
                ParamType.FLOAT, this.hitPosition.Y,
                ParamType.FLOAT, this.hitPosition.Z,
                ParamType.FLOAT, this.offsets.X,
                ParamType.FLOAT, this.offsets.Y,
                ParamType.FLOAT, this.offsets.Z,
                ParamType.UINT8, this.weaponID,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, this.fromPlayerID);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
