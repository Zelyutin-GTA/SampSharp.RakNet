using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class UnoccupiedSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;

        public int vehicleID;
        public int seatID;
        public Vector3 roll;
        public Vector3 direction;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public float vehicleHealth;

        public UnoccupiedSync(BitStream bs)
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
            //ReadSync() playerID;

            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetID = (int)result["packetID"];
                if (outcoming)
                {
                    this.fromPlayerID = (int)result["fromPlayerID"];
                }

                this.vehicleID = (int)result["vehicleID"];
                this.seatID = (int)result["seatID"];
                this.roll = new Vector3((float)result["roll_0"], (float)result["roll_1"], (float)result["roll_2"]);
                this.direction = new Vector3((float)result["direction_0"], (float)result["direction_1"], (float)result["direction_2"]);
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.angularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
                    this.vehicleHealth = (float)result["vehicleHealth"];
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.FLOAT, "velocity_0",
                    ParamType.FLOAT, "velocity_1",
                    ParamType.FLOAT, "velocity_2",
                    ParamType.FLOAT, "angularVelocity_0",
                    ParamType.FLOAT, "angularVelocity_1",
                    ParamType.FLOAT, "angularVelocity_2",
                    ParamType.FLOAT, "vehicleHealth"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "vehicleID",
                ParamType.UINT8, "seatID",
                ParamType.FLOAT, "roll_0",
                ParamType.FLOAT, "roll_1",
                ParamType.FLOAT, "roll_2",
                ParamType.FLOAT, "direction_0",
                ParamType.FLOAT, "direction_1",
                ParamType.FLOAT, "direction_2",
                ParamType.FLOAT, "position_0",
                ParamType.FLOAT, "position_1",
                ParamType.FLOAT, "position_2",
            };

            if (outcoming)
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
                ParamType.UINT16, this.vehicleID,
                ParamType.UINT8, this.seatID,
                ParamType.FLOAT, this.roll.X,
                ParamType.FLOAT, this.roll.Y,
                ParamType.FLOAT, this.roll.Z,
                ParamType.FLOAT, this.direction.X,
                ParamType.FLOAT, this.direction.Y,
                ParamType.FLOAT, this.direction.Z,
                ParamType.FLOAT, this.position.X,
                ParamType.FLOAT, this.position.Y,
                ParamType.FLOAT, this.position.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UINT16);
                arguments.Insert(3, this.fromPlayerID);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.FLOAT, this.velocity.X,
                ParamType.FLOAT, this.velocity.Y,
                ParamType.FLOAT, this.velocity.Z,
                ParamType.FLOAT, this.angularVelocity.X,
                ParamType.FLOAT, this.angularVelocity.Y,
                ParamType.FLOAT, this.angularVelocity.Z,
                ParamType.FLOAT, this.vehicleHealth,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}