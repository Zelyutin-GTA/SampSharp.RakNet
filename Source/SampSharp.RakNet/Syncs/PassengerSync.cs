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

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int DriveBy { get; set; }
        public int SeatId { get; set; }
        public int VehicleId { get; set; }
        public int AdditionalKey { get; set; }
        public int WeaponId { get; set; }
        public int PlayerHealth { get; set; }
        public int PlayerArmour { get; set; }
        public int LRKey { get; set; }
        public int UDKey { get; set; }
        public int Keys { get; set; }
        public Vector3 Position { get; set; }

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
                this.PacketId = (int)result["packetId"];
                if (outcoming)
                {
                    this.FromPlayerId = (int)result["fromPlayerId"];
                }

                this.VehicleId = (int)result["vehicleId"];
                this.DriveBy = (int)result["driveBy"];
                this.SeatId = (int)result["seatId"];
                this.AdditionalKey = (int)result["additionalKey"];
                this.WeaponId = (int)result["weaponId"];
                this.PlayerHealth = (int)result["playerHealth"];
                this.PlayerArmour = (int)result["playerArmour"];
                this.LRKey = (int)result["lrKey"];
                this.UDKey = (int)result["udKey"];
                this.Keys = (int)result["keys"];

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);

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
                ParamType.UInt8, this.PacketId,
                ParamType.UInt16, this.VehicleId,
                ParamType.Bits, this.DriveBy, 2,
                ParamType.Bits, this.SeatId, 6,
                ParamType.Bits, this.AdditionalKey, 2,
                ParamType.Bits, this.WeaponId, 6,
                ParamType.UInt8, this.PlayerHealth,
                ParamType.UInt8, this.PlayerArmour,
                ParamType.UInt16, this.LRKey,
                ParamType.UInt16, this.UDKey,
                ParamType.UInt16, this.Keys,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, this.FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}
