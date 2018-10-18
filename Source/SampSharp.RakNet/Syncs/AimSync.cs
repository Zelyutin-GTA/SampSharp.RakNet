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

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int CameraMode { get; set; }
        public Vector3 CameraFrontVector { get; set; }
        public Vector3 CameraPosition { get; set; }
        public float AimZ { get; set; }
        public int WeaponState { get; set; }
        public int CameraZoom { get; set; }
        public int AspectRatio { get; set; }

        public AimSync(BitStream bs)
        {
            BS = bs;
        }
        public void ReadIncoming()
        {
            Read(false);
        }
        public void ReadOutcoming()
        {
            Read(true);
        }
        public void WriteIncoming()
        {
            Write(false);
        }
        public void WriteOutcoming()
        {
            Write(true);
        }
        private void Read(bool outcoming)
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                PacketId = (int)result["packetId"];
                if (outcoming)
                {
                    FromPlayerId = (int)result["fromPlayerId"];
                }

                CameraMode = (int)result["cameraMode"];
                CameraFrontVector = new Vector3((float) result["cameraFrontVector_0"], (float) result["cameraFrontVector_1"], (float) result["cameraFrontVector_2"]);
                CameraPosition = new Vector3((float) result["cameraPosition_0"], (float) result["cameraPosition_2"], (float) result["cameraPosition_2"]);
                AimZ = (float) result["aimZ"];

                WeaponState = (int)result["weaponState"];
                CameraZoom = (int)result["cameraZoom"];
                AspectRatio = (int)result["aspectRatio"];

                ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
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
                ParamType.UInt8, PacketId,
                ParamType.UInt8, CameraMode,
                ParamType.Float, CameraFrontVector.X,
                ParamType.Float, CameraFrontVector.Y,
                ParamType.Float, CameraFrontVector.Z,
                ParamType.Float, CameraPosition.X,
                ParamType.Float, CameraPosition.Y,
                ParamType.Float, CameraPosition.Z,
                ParamType.Float, AimZ,
                ParamType.Bits, WeaponState, 2,
                ParamType.Bits, CameraZoom, 6,
                ParamType.UInt8, AspectRatio
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
