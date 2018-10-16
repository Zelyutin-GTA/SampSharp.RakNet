﻿using System;
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
                ParamType.FLOAT, "quaternion_0",
                ParamType.FLOAT, "quaternion_1",
                ParamType.FLOAT, "quaternion_2",
                ParamType.FLOAT, "quaternion_3",
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
    }
}
