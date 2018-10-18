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

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }

        public int VehicleId { get; set; }
        public int SeatId { get; set; }
        public Vector3 Roll { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public float VehicleHealth { get; set; }

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
                this.PacketId = (int)result["packetId"];
                if (outcoming)
                {
                    this.FromPlayerId = (int)result["fromPlayerId"];
                }

                this.VehicleId = (int)result["vehicleId"];
                this.SeatId = (int)result["seatId"];
                this.Roll = new Vector3((float)result["roll_0"], (float)result["roll_1"], (float)result["roll_2"]);
                this.Direction = new Vector3((float)result["direction_0"], (float)result["direction_1"], (float)result["direction_2"]);
                this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.AngularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
                    this.VehicleHealth = (float)result["vehicleHealth"];
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
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "vehicleId",
                ParamType.UInt8, "seatId",
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
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, "fromPlayerId");
            }

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, this.PacketId,
                ParamType.UInt16, this.VehicleId,
                ParamType.UInt8, this.SeatId,
                ParamType.Float, this.Roll.X,
                ParamType.Float, this.Roll.Y,
                ParamType.Float, this.Roll.Z,
                ParamType.Float, this.Direction.X,
                ParamType.Float, this.Direction.Y,
                ParamType.Float, this.Direction.Z,
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.Velocity.X,
                ParamType.Float, this.Velocity.Y,
                ParamType.Float, this.Velocity.Z,
                ParamType.Float, this.AngularVelocity.X,
                ParamType.Float, this.AngularVelocity.Y,
                ParamType.Float, this.AngularVelocity.Z,
                ParamType.Float, this.VehicleHealth,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}