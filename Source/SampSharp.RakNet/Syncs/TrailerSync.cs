using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class TrailerSync : ISync
    {
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

            var result = BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.

            PacketId = (int)result["packetId"];
            if (outcoming)
            {
                FromPlayerId = (int)result["fromPlayerId"];
            }

            TrailerId = (int)result["trailerId"];
            Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
            Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
            Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
            
            result = BS.ReadValue(
                ParamType.Float, "angularVelocity_0",
                ParamType.Float, "angularVelocity_1",
                ParamType.Float, "angularVelocity_2"
            );

            AngularVelocity = new Vector3((float)result["angularVelocity_0"], (float)result["angularVelocity_1"], (float)result["angularVelocity_2"]);
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, PacketId,
                ParamType.UInt16, TrailerId,
                ParamType.Float, Position.X,
                ParamType.Float, Position.Y,
                ParamType.Float, Position.Z,
                ParamType.Float, Quaternion.W,
                ParamType.Float, Quaternion.X,
                ParamType.Float, Quaternion.Y,
                ParamType.Float, Quaternion.Z,
                ParamType.Float, Velocity.X,
                ParamType.Float, Velocity.Y,
                ParamType.Float, Velocity.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, AngularVelocity.X,
                ParamType.Float, AngularVelocity.Y,
                ParamType.Float, AngularVelocity.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}