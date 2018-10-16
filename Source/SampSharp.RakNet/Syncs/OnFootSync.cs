using System;

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
        public void Read()
        {
            //ReadSync() playerID;

            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetID = (int)result["packetID"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.quaternion = new Vector4((float)result["quaternion_0"], (float)result["quaternion_1"], (float)result["quaternion_2"], (float)result["quaternion_3"]);
                this.health = (int)result["health"];
                this.armour = (int)result["armour"];
                this.additionalKey = (int)result["additionalKey"];

                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    var result2 = args2.Result;
                    this.weaponID = (int)result2["weaponID"];
                    this.specialAction = (int)result2["specialAction"];
                    this.velocity = new Vector3((float)result2["velocity_0"], (float)result2["velocity_1"], (float)result2["velocity_2"]);
                    this.surfingOffsets = new Vector3((float)result2["surfingOffsets_0"], (float)result2["surfingOffsets_1"], (float)result2["surfingOffsets_2"]);
                    this.surfingVehicleID = (int)result2["surfingVehicleID"];
                    this.animationID = (int)result2["animationID"];
                    this.animationFlags = (int)result2["animationFlags"];

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

            BS.ReadValue(
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "lrKey",
                ParamType.UINT16, "udKey",
                ParamType.UINT16, "keys",
                ParamType.FLOAT, "position_0",
                ParamType.FLOAT, "position_1",
                ParamType.FLOAT, "position_2",
                ParamType.FLOAT, "quaternion_0",
                ParamType.FLOAT, "quaternion_1",
                ParamType.FLOAT, "quaternion_2",
                ParamType.FLOAT, "quaternion_3",
                ParamType.UINT8, "health",
                ParamType.UINT8, "armour",
                ParamType.BITS, "additionalKey", 2
            );
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
    }
}
