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

            var result = BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.

            PacketId = (int)result["packetId"];
            if (outcoming)
            {
                FromPlayerId = (int)result["fromPlayerId"];
            }

            VehicleId = (int)result["vehicleId"];
            DriveBy = (int)result["driveBy"];
            SeatId = (int)result["seatId"];
            AdditionalKey = (int)result["additionalKey"];
            WeaponId = (int)result["weaponId"];
            PlayerHealth = (int)result["playerHealth"];
            PlayerArmour = (int)result["playerArmour"];
            LRKey = (int)result["lrKey"];
            UDKey = (int)result["udKey"];
            Keys = (int)result["keys"];

            result = BS.ReadValue(
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2"
            );

            Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);

            ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
        }
        private void Write(bool outcoming)
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, PacketId,
                ParamType.UInt16, VehicleId,
                ParamType.Bits, DriveBy, 2,
                ParamType.Bits, SeatId, 6,
                ParamType.Bits, AdditionalKey, 2,
                ParamType.Bits, WeaponId, 6,
                ParamType.UInt8, PlayerHealth,
                ParamType.UInt8, PlayerArmour,
                ParamType.UInt16, LRKey,
                ParamType.UInt16, UDKey,
                ParamType.UInt16, Keys,
            };

            if (outcoming)
            {
                arguments.Insert(2, ParamType.UInt16);
                arguments.Insert(3, FromPlayerId);
            }

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, Position.X,
                ParamType.Float, Position.Y,
                ParamType.Float, Position.Z,
            };

            BS.WriteValue(arguments.ToArray());
        }
    }
}
