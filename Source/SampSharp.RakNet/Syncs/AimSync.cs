using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class AimSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS { get; set; }

        public int packetId { get; set; }
        public int fromPlayerId { get; set; }
        public int cameraMode { get; set; }
        public Vector3 cameraFrontVector { get; set; }
        public Vector3 cameraPosition { get; set; }
        public float aimZ { get; set; }
        public int weaponState { get; set; }
        public int cameraZoom { get; set; }
        public int aspectRatio { get; set; }

        public AimSync(BitStream bs)
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
                this.packetId = (int)result["packetId"];
                if (outcoming)
                {
                    this.fromPlayerId = (int)result["fromPlayerId"];
                }

                cameraMode = (int)result["cameraMode"];
                cameraFrontVector = new Vector3((float) result["cameraFrontVector_0"], (float) result["cameraFrontVector_1"], (float) result["cameraFrontVector_2"]);
                cameraPosition = new Vector3((float) result["cameraPosition_0"], (float) result["cameraPosition_2"], (float) result["cameraPosition_2"]);
                aimZ = (float) result["aimZ"];

                weaponState = (int)result["weaponState"];
                cameraZoom = (int)result["cameraZoom"];
                aspectRatio = (int)result["aspectRatio"];

                this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
            };

            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt8, "cameraMode",
                ParamType.Float, "cameraFrontVector_0",
                ParamType.Float, "cameraFrontVector_1",
                ParamType.Float, "cameraFrontVector_2",
                ParamType.Float, "cameraPosition_0",
                ParamType.Float, "cameraPosition_1",
                ParamType.Float, "cameraPosition_2",
                ParamType.Float, "aimZ",
                ParamType.Bits, "weaponState", 2,
                ParamType.Bits, "cameraZoom", 6,
                ParamType.UInt8, "aspectRatio",

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
                ParamType.UInt8, this.packetId,
                ParamType.UInt8, this.cameraMode,
                ParamType.Float, this.cameraFrontVector.X,
                ParamType.Float, this.cameraFrontVector.Y,
                ParamType.Float, this.cameraFrontVector.Z,
                ParamType.Float, this.cameraPosition.X,
                ParamType.Float, this.cameraPosition.Y,
                ParamType.Float, this.cameraPosition.Z,
                ParamType.Float, this.aimZ,
                ParamType.Bits, this.weaponState, 2,
                ParamType.Bits, this.cameraZoom, 6,
                ParamType.UInt8, this.aspectRatio
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.fromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());
        }
    }
}
