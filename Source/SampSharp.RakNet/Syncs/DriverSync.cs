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

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int VehicleId { get; set; }
        public int LRKey { get; set; }
        public int UDKey { get; set; }
        public int Keys { get; set; }
        public Vector4 Quaternion { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float VehicleHealth { get; set; }
        public int PlayerHealth { get; set; }
        public int PlayerArmour { get; set; }
        public int AdditionalKey { get; set; }
        public int WeaponId { get; set; }
        public int SirenState { get; set; }
        public int LandingGearState { get; set; }
        public int TrailerId { get; set; }
        public float TrainSpeed { get; set; }

        public DriverSync(BitStream bs)
        {
            BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                PacketId = (int)result["packetId"];

                VehicleId = (int)result["vehicleId"];
                LRKey = (int)result["lrKey"];
                UDKey = (int)result["udKey"];
                Keys = (int)result["keys"];
                Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    VehicleHealth = (float)result["vehicleHealth"];
                    PlayerHealth = (int)result["playerHealth"];
                    PlayerArmour = (int)result["playerArmour"];
                    AdditionalKey = (int)result["additionalKey"];
                    WeaponId = (int)result["weaponId"];
                    SirenState = (int)result["sirenState"];
                    LandingGearState = (int)result["landingGearState"];
                    TrailerId = (int)result["trailerId"];
                    TrainSpeed = (float)result["trainSpeed"];

                    ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Float, "velocity_0",
                    ParamType.Float, "velocity_1",
                    ParamType.Float, "velocity_2",
                    ParamType.Float, "vehicleHealth",
                    ParamType.UInt8, "playerHealth",
                    ParamType.UInt8, "playerArmour",
                    ParamType.Bits, "additionalKey", 2,
                    ParamType.Bits, "weaponId", 6,
                    ParamType.UInt8, "sirenState",
                    ParamType.UInt8, "landingGearState",
                    ParamType.UInt16, "trailerId",
                    ParamType.Float, "trainSpeed"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "vehicleId",
                ParamType.UInt16, "lrKey",
                ParamType.UInt16, "udKey",
                ParamType.UInt16, "keys",
                ParamType.Float, "quaternion_W",
                ParamType.Float, "quaternion_X",
                ParamType.Float, "quaternion_Y",
                ParamType.Float, "quaternion_Z",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",

            };

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        public void ReadOutcoming()
        {
            PacketId = BS.ReadUInt8();
            FromPlayerId = BS.ReadUInt16();
            VehicleId = BS.ReadUInt16();

            // LEFT/RIGHT KEYS
            LRKey = BS.ReadUInt16();

            // UP/DOWN KEYS
            UDKey = BS.ReadUInt16();

            // GENERAL KEYS
            Keys = BS.ReadUInt16();
            
            // ROTATION
            Quaternion = BS.ReadNormQuat();

            float x = BS.ReadFloat();
            float y = BS.ReadFloat();
            float z = BS.ReadFloat();
            Position = new Vector3(x, y, z);

            Velocity = BS.ReadVector();
            VehicleHealth = (float)BS.ReadUInt16();

            byte healthArmour = Convert.ToByte(BS.ReadUInt8());

            HealthArmour.GetFromByte(healthArmour, out int health, out int armour);
            PlayerHealth = health;
            PlayerArmour = armour;

            WeaponId = BS.ReadUInt8();

            bool sirenState = BS.ReadCompressedBool();
            if(sirenState)
                SirenState = 1;

            bool landingGear = BS.ReadCompressedBool();
            if (landingGear)
                LandingGearState = 1;

            // HYDRA THRUST ANGLE AND TRAILER Id
            bool hydra = BS.ReadCompressedBool();
            bool trailer = BS.ReadCompressedBool();

            
            int trailerId_or_thrustAngle = BS.ReadUInt32();
            bool train = BS.ReadCompressedBool();

            if (train)
            {
                TrainSpeed = (float)BS.ReadUInt8();
            }

            ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
        }
        public void WriteIncoming()
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, PacketId,
                ParamType.UInt16, VehicleId,
                ParamType.UInt16, LRKey,
                ParamType.UInt16, UDKey,
                ParamType.UInt16, Keys,
                ParamType.Float, Quaternion.W,
                ParamType.Float, Quaternion.Y,
                ParamType.Float, Quaternion.Z,
                ParamType.Float, Position.X,
                ParamType.Float, Position.Y,
                ParamType.Float, Position.Z,
                ParamType.UInt16, FromPlayerId
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, Velocity.X,
                ParamType.Float, Velocity.Y,
                ParamType.Float, Velocity.Z,
                ParamType.Float, VehicleHealth,
                ParamType.UInt8, PlayerHealth,
                ParamType.UInt8, PlayerArmour,
                ParamType.Bits, AdditionalKey, 2,
                ParamType.Bits, WeaponId, 6,
                ParamType.UInt8, SirenState,
                ParamType.UInt8, LandingGearState,
                ParamType.UInt16, TrailerId,
                ParamType.Float, TrainSpeed
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteValue(
                ParamType.UInt8, PacketId,
                ParamType.UInt16, FromPlayerId,
                ParamType.UInt16, VehicleId,
                ParamType.UInt16, LRKey,
                ParamType.UInt16, UDKey,
                ParamType.UInt16, Keys
           );

            BS.WriteNormQuat(Quaternion);
            
            BS.WriteValue(
               ParamType.Float, Position.X,
               ParamType.Float, Position.Y,
               ParamType.Float, Position.Z
            );

            BS.WriteVector(Velocity);
            BS.WriteUInt16((int)VehicleHealth);

            byte healthArmour = HealthArmour.SetInByte(PlayerHealth, PlayerArmour);
            BS.WriteUInt8((int)healthArmour);
            BS.WriteUInt8(WeaponId);
            
            if (SirenState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            if (LandingGearState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            // HYDRA THRUST ANGLE AND TRAILER Id
            BS.WriteBool(false);
            BS.WriteBool(false);

            int trailerId_or_thrustAngle = 0;
            BS.WriteUInt32(trailerId_or_thrustAngle);

            // TRAIN SPECIAL
            BS.WriteBool(false);
        }
    }
}
