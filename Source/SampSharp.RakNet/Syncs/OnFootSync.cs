using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class OnFootSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int lrKey;
        public int udKey;
        public int keys;
        public Vector3 position;
        public Vector4 quaternion;
        public int health;
        public int armour;
        public int additionalKey;
        public int weaponID;
        public int specialAction;
        public Vector3 velocity;
        public Vector3 surfingOffsets;
        public int surfingVehicleID;
        public int animationID;
        public int animationFlags;

        public OnFootSync(BitStream bs)
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
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.health = (int)result["health"];
                this.armour = (int)result["armour"];
                this.additionalKey = (int)result["additionalKey"];

                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.weaponID = (int)result["weaponID"];
                    this.specialAction = (int)result["specialAction"];
                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.surfingOffsets = new Vector3((float)result["surfingOffsets_0"], (float)result["surfingOffsets_1"], (float)result["surfingOffsets_2"]);
                    this.surfingVehicleID = (int)result["surfingVehicleID"];
                    this.animationID = (int)result["animationID"];
                    this.animationFlags = (int)result["animationFlags"];

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.BITS, "weaponID", 6,
                    ParamType.UINT8, "specialAction",
                    ParamType.FLOAT, "velocity_0",
                    ParamType.FLOAT, "velocity_1",
                    ParamType.FLOAT, "velocity_2",
                    ParamType.FLOAT, "surfingOffsets_0",
                    ParamType.FLOAT, "surfingOffsets_1",
                    ParamType.FLOAT, "surfingOffsets_2",
                    ParamType.UINT16, "surfingVehicleID",
                    ParamType.INT16, "animationID",
                    ParamType.INT16, "animationFlags"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "lrKey",
                ParamType.UINT16, "udKey",
                ParamType.UINT16, "keys",
                ParamType.FLOAT, "position_0",
                ParamType.FLOAT, "position_1",
                ParamType.FLOAT, "position_2",
                ParamType.FLOAT, "quaternion_W",
                ParamType.FLOAT, "quaternion_X",
                ParamType.FLOAT, "quaternion_Y",
                ParamType.FLOAT, "quaternion_Z",
                ParamType.UINT8, "health",
                ParamType.UINT8, "armour",
                ParamType.BITS, "additionalKey", 2
            };
            if(outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UINT8, this.packetID,
                ParamType.UINT16, this.lrKey,
                ParamType.UINT16, this.udKey,
                ParamType.UINT16, this.keys,
                ParamType.FLOAT, this.position.X,
                ParamType.FLOAT, this.position.Y,
                ParamType.FLOAT, this.position.Z,
                ParamType.FLOAT, this.quaternion.W,
                ParamType.FLOAT, this.quaternion.X,
                ParamType.FLOAT, this.quaternion.Y,
                ParamType.FLOAT, this.quaternion.Z,
                ParamType.UINT8, this.health,
                ParamType.UINT8, this.armour,
                ParamType.BITS, this.additionalKey, 2
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, this.fromPlayerID);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.BITS, this.weaponID, 6,
                ParamType.UINT8, this.specialAction,
                ParamType.FLOAT, this.velocity.X,
                ParamType.FLOAT, this.velocity.Y,
                ParamType.FLOAT, this.velocity.Z,
                ParamType.FLOAT, this.surfingOffsets.X,
                ParamType.FLOAT, this.surfingOffsets.Y,
                ParamType.FLOAT, this.surfingOffsets.Z,
                ParamType.UINT16, this.surfingVehicleID,
                ParamType.INT16, this.animationID,
                ParamType.INT16, this.animationFlags
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}
