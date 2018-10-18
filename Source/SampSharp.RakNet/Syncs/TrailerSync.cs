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

                this.trailerID = (int)result["trailerID"];
                this.quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
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
                    ParamType.Float, "angularVelocity_0",
                    ParamType.Float, "angularVelocity_1",
                    ParamType.Float, "angularVelocity_2"
                );
            };
            
            var arguments = new List<object>()
            {
                ParamType.Uint8, "packetID",
                ParamType.Uint16, "trailerID",
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
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, "fromPlayerID");
            }

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.Uint8, this.packetID,
                ParamType.Uint16, this.trailerID,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z,
                ParamType.Float, this.quaternion.W,
                ParamType.Float, this.quaternion.X,
                ParamType.Float, this.quaternion.Y,
                ParamType.Float, this.quaternion.Z,
                ParamType.Float, this.velocity.X,
                ParamType.Float, this.velocity.Y,
                ParamType.Float, this.velocity.Z,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.Uint16);
                arguments.Insert(3, this.fromPlayerID);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.angularVelocity.X,
                ParamType.Float, this.angularVelocity.Y,
                ParamType.Float, this.angularVelocity.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}