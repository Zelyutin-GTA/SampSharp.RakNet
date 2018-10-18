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

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int HitType { get; set; }
        public int HitId { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 HitPosition { get; set; }
        public Vector3 Offsets { get; set; }
        public int WeaponId { get; set; }

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
                this.PacketId = (int)result["packetId"];
                if (outcoming)
                {
                    this.FromPlayerId = (int)result["fromPlayerId"];
                }

                HitType = (int)result["hitType"];
                HitId = (int)result["hitId"];
                Origin = new Vector3((float)result["origin_0"], (float)result["origin_1"], (float)result["origin_2"]);
                HitPosition = new Vector3((float)result["hitPosition_0"], (float)result["hitPosition_1"], (float)result["hitPosition_2"]);
                Offsets = new Vector3((float)result["offsets_0"], (float)result["offsets_1"], (float)result["offsets_2"]);

                WeaponId = (int)result["weaponId"];

                this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
            };

            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt8, "hitType",
                ParamType.UInt16, "hitId",
                ParamType.Float, "origin_0",
                ParamType.Float, "origin_1",
                ParamType.Float, "origin_2",
                ParamType.Float, "hitPosition_0",
                ParamType.Float, "hitPosition_1",
                ParamType.Float, "hitPosition_2",
                ParamType.Float, "offsets_0",
                ParamType.Float, "offsets_1",
                ParamType.Float, "offsets_2",
                ParamType.UInt8, "weaponId",
            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, "fromPlayerId");
            }

            BS.ReadValue(arguments.ToArray());
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, this.PacketId,
                ParamType.UInt8, this.HitType,
                ParamType.UInt16, this.HitId,
                ParamType.Float, this.Origin.X,
                ParamType.Float, this.Origin.Y,
                ParamType.Float, this.Origin.Z,
                ParamType.Float, this.HitPosition.X,
                ParamType.Float, this.HitPosition.Y,
                ParamType.Float, this.HitPosition.Z,
                ParamType.Float, this.Offsets.X,
                ParamType.Float, this.Offsets.Y,
                ParamType.Float, this.Offsets.Z,
                ParamType.UInt8, this.WeaponId,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
