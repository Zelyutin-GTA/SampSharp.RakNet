﻿using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class TrailerSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;

        public int trailerID;
        public Vector3 position;
        public Vector4 quaternion;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public TrailerSync(BitStream bs)
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

                this.trailerID = (int)result["trailerID"];
                this.quaternion = new Vector4((float)result["quaternion_0"], (float)result["quaternion_1"], (float)result["quaternion_2"], (float)result["quaternion_3"]);
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);

                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.angularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.FLOAT, "angularVelocity_0",
                    ParamType.FLOAT, "angularVelocity_1",
                    ParamType.FLOAT, "angularVelocity_2"
                );
            };
            
            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "trailerID",
                ParamType.FLOAT, "position_0",
                ParamType.FLOAT, "position_1",
                ParamType.FLOAT, "position_2",
                ParamType.FLOAT, "quaternion_0",
                ParamType.FLOAT, "quaternion_1",
                ParamType.FLOAT, "quaternion_2",
                ParamType.FLOAT, "quaternion_3",
                ParamType.FLOAT, "velocity_0",
                ParamType.FLOAT, "velocity_1",
                ParamType.FLOAT, "velocity_2",
            };
            
            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
    }
}