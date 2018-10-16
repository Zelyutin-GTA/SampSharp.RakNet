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
        public Vector3 cameraPos;
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
        private void Read(bool outcoming)
        {
            //ReadSync() playerID;

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
                cameraPos = new Vector3((float) result["cameraPos_0"], (float) result["cameraPos_2"], (float) result["cameraPos_2"]);
                aimZ = (float) result["aimZ"];

                weaponState = (int)result["weaponState"];
                cameraZoom = (int)result["cameraZoom"];
                aspectRatio = (int)result["aspectRatio"];

                this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
            };

            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT8, "cameraMode",
                ParamType.FLOAT, "cameraFrontVector_0",
                ParamType.FLOAT, "cameraFrontVector_1",
                ParamType.FLOAT, "cameraFrontVector_2",
                ParamType.FLOAT, "cameraPos_0",
                ParamType.FLOAT, "cameraPos_1",
                ParamType.FLOAT, "cameraPos_2",
                ParamType.FLOAT, "aimZ",
                ParamType.BITS, "weaponState", 2,
                ParamType.BITS, "cameraZoom", 6,
                ParamType.UINT8, "aspectRatio",

            };
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
        }
    }
}
