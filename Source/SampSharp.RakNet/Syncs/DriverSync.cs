using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class DriverSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int vehicleID;
        public int lrKey;
        public int udKey;
        public int keys;
        public Vector4 quaternion;
        public Vector3 position;
        public Vector3 velocity;
        public float vehicleHealth;
        public int playerHealth;
        public int playerArmour;
        public int additionalKey;
        public int weaponID;
        public int sirenState;
        public int landingGearState;
        public int trailerID;
        public float trainSpeed;

        public DriverSync(BitStream bs)
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

                this.vehicleID = (int)result["vehicleID"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.quaternion = new Vector4((float) result["quaternion_X"], (float) result["quaternion_Y"], (float) result["quaternion_Z"], (float) result["quaternion_W"]); // order is different from one in a bitstream
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                

                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.vehicleHealth = (float)result["vehicleHealth"];
                    this.playerHealth = (int)result["playerHealth"];
                    this.playerArmour = (int)result["playerArmour"];
                    this.additionalKey = (int)result["additionalKey"];
                    this.weaponID = (int)result["weaponID"];
                    this.sirenState = (int)result["sirenState"];
                    this.landingGearState = (int)result["landingGearState"];
                    this.trailerID = (int)result["trailerID"];
                    this.trainSpeed = (float)result["trainSpeed"];

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.FLOAT, "velocity_0",
                    ParamType.FLOAT, "velocity_1",
                    ParamType.FLOAT, "velocity_2",
                    ParamType.FLOAT, "vehicleHealth",
                    ParamType.UINT8, "playerHealth",
                    ParamType.UINT8, "playerArmour",
                    ParamType.BITS, "additionalKey", 2,
                    ParamType.BITS, "weaponID", 6,
                    ParamType.UINT8, "sirenState",
                    ParamType.UINT8, "landingGearState",
                    ParamType.UINT16, "trailerID",
                    ParamType.FLOAT, "trainSpeed"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "vehicleID",
                ParamType.UINT16, "lrKey",
                ParamType.UINT16, "udKey",
                ParamType.UINT16, "keys",
                ParamType.FLOAT, "quaternion_W",
                ParamType.FLOAT, "quaternion_X",
                ParamType.FLOAT, "quaternion_Y",
                ParamType.FLOAT, "quaternion_Z",
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
   
    }
}
