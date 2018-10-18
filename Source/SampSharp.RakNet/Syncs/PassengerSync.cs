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

        public int packetId;
        public int fromPlayerId;
        public int driveBy;
        public int seatId;
        public int vehicleId;
        public int additionalKey;
        public int weaponId;
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
                this.driveBy = (int)result["driveBy"];
                this.seatId = (int)result["seatId"];
                this.additionalKey = (int)result["additionalKey"];
                this.weaponId = (int)result["weaponId"];
                this.playerHealth = (int)result["playerHealth"];
                this.playerArmour = (int)result["playerArmour"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Float, "position_0",
                    ParamType.Float, "position_1",
                    ParamType.Float, "position_2"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "vehicleId",
                ParamType.Bits, "driveBy", 2,
                ParamType.Bits, "seatId", 6,
                ParamType.Bits, "additionalKey", 2,
                ParamType.Bits, "weaponId", 6,
                ParamType.UInt8, "playerHealth",
                ParamType.UInt8, "playerArmour",
                ParamType.UInt16, "lrKey",
                ParamType.UInt16, "udKey",
                ParamType.UInt16, "keys"
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
                ParamType.UInt8, this.packetId,
                ParamType.UInt16, this.vehicleId,
                ParamType.Bits, this.driveBy, 2,
                ParamType.Bits, this.seatId, 6,
                ParamType.Bits, this.additionalKey, 2,
                ParamType.Bits, this.weaponId, 6,
                ParamType.UInt8, this.playerHealth,
                ParamType.UInt8, this.playerArmour,
                ParamType.UInt16, this.lrKey,
                ParamType.UInt16, this.udKey,
                ParamType.UInt16, this.keys,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.fromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}
