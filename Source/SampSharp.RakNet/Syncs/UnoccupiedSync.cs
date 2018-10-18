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

        public int packetId;
        public int fromPlayerId;

        public int vehicleId;
        public int seatId;
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
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetId = (int)result["packetId"];
                if (outcoming)
                {
                    this.fromPlayerId = (int)result["fromPlayerId"];
                }

                this.vehicleId = (int)result["vehicleId"];
                this.seatId = (int)result["seatId"];
                this.roll = new Vector3((float)result["roll_0"], (float)result["roll_1"], (float)result["roll_2"]);
                this.direction = new Vector3((float)result["direction_0"], (float)result["direction_1"], (float)result["direction_2"]);
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.angularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
                    this.vehicleHealth = (float)result["vehicleHealth"];
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Float, "velocity_0",
                    ParamType.Float, "velocity_1",
                    ParamType.Float, "velocity_2",
                    ParamType.Float, "angularVelocity_0",
                    ParamType.Float, "angularVelocity_1",
                    ParamType.Float, "angularVelocity_2",
                    ParamType.Float, "vehicleHealth"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.Uint8, "packetId",
                ParamType.Uint16, "vehicleId",
                ParamType.Uint8, "seatId",
                ParamType.Float, "roll_0",
                ParamType.Float, "roll_1",
                ParamType.Float, "roll_2",
                ParamType.Float, "direction_0",
                ParamType.Float, "direction_1",
                ParamType.Float, "direction_2",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, "fromPlayerId");
            }

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.Uint8, this.packetId,
                ParamType.Uint16, this.vehicleId,
                ParamType.Uint8, this.seatId,
                ParamType.Float, this.roll.X,
                ParamType.Float, this.roll.Y,
                ParamType.Float, this.roll.Z,
                ParamType.Float, this.direction.X,
                ParamType.Float, this.direction.Y,
                ParamType.Float, this.direction.Z,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, this.fromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.velocity.X,
                ParamType.Float, this.velocity.Y,
                ParamType.Float, this.velocity.Z,
                ParamType.Float, this.angularVelocity.X,
                ParamType.Float, this.angularVelocity.Y,
                ParamType.Float, this.angularVelocity.Z,
                ParamType.Float, this.vehicleHealth,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}