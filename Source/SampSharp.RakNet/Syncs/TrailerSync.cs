using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class TrailerSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
    
        public int TrailerId { get; set; }
        public Vector3 Position { get; set; }
        public Vector4 Quaternion { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 AngularVelocity { get; set; }

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

                this.TrailerId = (int)result["trailerId"];
                this.Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.AngularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Float, "angularVelocity_0",
                    ParamType.Float, "angularVelocity_1",
                    ParamType.Float, "angularVelocity_2"
                );
            };
            
            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "trailerId",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",
                ParamType.Float, "quaternion_W",
                ParamType.Float, "quaternion_X",
                ParamType.Float, "quaternion_Y",
                ParamType.Float, "quaternion_Z",
                ParamType.Float, "velocity_0",
                ParamType.Float, "velocity_1",
                ParamType.Float, "velocity_2",
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
                ParamType.UInt16, this.TrailerId,
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z,
                ParamType.Float, this.Quaternion.W,
                ParamType.Float, this.Quaternion.X,
                ParamType.Float, this.Quaternion.Y,
                ParamType.Float, this.Quaternion.Z,
                ParamType.Float, this.Velocity.X,
                ParamType.Float, this.Velocity.Y,
                ParamType.Float, this.Velocity.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.AngularVelocity.X,
                ParamType.Float, this.AngularVelocity.Y,
                ParamType.Float, this.AngularVelocity.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}