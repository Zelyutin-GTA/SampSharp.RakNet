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

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int cameraMode;
        public Vector3 cameraFrontVector;
        public Vector3 cameraPosition;
        public float aimZ;
        public int weaponState;
        public int cameraZoom;
        public int aspectRatio;

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
                this.packetID = (int)result["packetID"];
                if (outcoming)
                {
                    this.fromPlayerID = (int)result["fromPlayerID"];
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
                ParamType.Uint8, "packetID",
                ParamType.Uint8, "cameraMode",
                ParamType.Float, "cameraFrontVector_0",
                ParamType.Float, "cameraFrontVector_1",
                ParamType.Float, "cameraFrontVector_2",
                ParamType.Float, "cameraPosition_0",
                ParamType.Float, "cameraPosition_1",
                ParamType.Float, "cameraPosition_2",
                ParamType.Float, "aimZ",
                ParamType.Bits, "weaponState", 2,
                ParamType.Bits, "cameraZoom", 6,
                ParamType.Uint8, "aspectRatio",

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
                ParamType.Uint8, this.cameraMode,
                ParamType.Float, this.cameraFrontVector.X,
                ParamType.Float, this.cameraFrontVector.Y,
                ParamType.Float, this.cameraFrontVector.Z,
                ParamType.Float, this.cameraPosition.X,
                ParamType.Float, this.cameraPosition.Y,
                ParamType.Float, this.cameraPosition.Z,
                ParamType.Float, this.aimZ,
                ParamType.Bits, this.weaponState, 2,
                ParamType.Bits, this.cameraZoom, 6,
                ParamType.Uint8, this.aspectRatio
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
