using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class PassengerSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetID;
        public int fromPlayerID;
        public int driveBy;
        public int seatID;
        public int vehicleID;
        public int additionalKey;
        public int weaponID;
        public int playerHealth;
        public int playerArmour;
        public int lrKey;
        public int udKey;
        public int keys;
        public Vector3 position;

        public PassengerSync(BitStream bs)
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
                this.driveBy = (int)result["driveBy"];
                this.seatID = (int)result["seatID"];
                this.additionalKey = (int)result["additionalKey"];
                this.weaponID = (int)result["weaponID"];
                this.playerHealth = (int)result["playerHealth"];
                this.playerArmour = (int)result["playerArmour"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];

                var BS2 = new BitStream(BS.ID);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.FLOAT, "position_0",
                    ParamType.FLOAT, "position_1",
                    ParamType.FLOAT, "position_2"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UINT8, "packetID",
                ParamType.UINT16, "vehicleID",
                ParamType.BITS, "driveBy", 2,
                ParamType.BITS, "seatID", 6,
                ParamType.BITS, "additionalKey", 2,
                ParamType.BITS, "weaponID", 6,
                ParamType.UINT8, "playerHealth",
                ParamType.UINT8, "playerArmour",
                ParamType.UINT16, "lrKey",
                ParamType.UINT16, "udKey",
                ParamType.UINT16, "keys"

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
