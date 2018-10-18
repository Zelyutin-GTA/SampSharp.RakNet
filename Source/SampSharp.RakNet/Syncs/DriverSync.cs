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
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.PacketId = (int)result["packetId"];

                this.VehicleId = (int)result["vehicleId"];
                this.LRKey = (int)result["lrKey"];
                this.UDKey = (int)result["udKey"];
                this.Keys = (int)result["keys"];
                this.Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.VehicleHealth = (float)result["vehicleHealth"];
                    this.PlayerHealth = (int)result["playerHealth"];
                    this.PlayerArmour = (int)result["playerArmour"];
                    this.AdditionalKey = (int)result["additionalKey"];
                    this.WeaponId = (int)result["weaponId"];
                    this.SirenState = (int)result["sirenState"];
                    this.LandingGearState = (int)result["landingGearState"];
                    this.TrailerId = (int)result["trailerId"];
                    this.TrainSpeed = (float)result["trainSpeed"];

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
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
            this.PacketId = this.BS.ReadUInt8();
            this.FromPlayerId = this.BS.ReadUInt16();
            this.VehicleId = this.BS.ReadUInt16();

            // LEFT/RIGHT KEYS
            this.LRKey = this.BS.ReadUInt16();

            // UP/DOWN KEYS
            this.UDKey = this.BS.ReadUInt16();

            // GENERAL KEYS
            this.Keys = this.BS.ReadUInt16();
            
            // ROTATION
            this.Quaternion = BS.ReadNormQuat();

            float x = BS.ReadFloat();
            float y = BS.ReadFloat();
            float z = BS.ReadFloat();
            this.Position = new Vector3(x, y, z);

            this.Velocity = BS.ReadVector();
            this.VehicleHealth = (float)BS.ReadUInt16();

            byte healthArmour = Convert.ToByte(BS.ReadUInt8());

            HealthArmour.GetFromByte(healthArmour, out int health, out int armour);
            this.PlayerHealth = health;
            this.PlayerArmour = armour;

            this.WeaponId = BS.ReadUInt8();

            bool sirenState = BS.ReadCompressedBool();
            if(sirenState)
                this.SirenState = 1;

            bool landingGear = BS.ReadCompressedBool();
            if (landingGear)
                this.LandingGearState = 1;

            // HYDRA THRUST ANGLE AND TRAILER Id
            bool hydra = BS.ReadCompressedBool();
            bool trailer = BS.ReadCompressedBool();

            
            int trailerId_or_thrustAngle = BS.ReadUInt32();
            bool train = BS.ReadCompressedBool();

            if (train)
            {
                this.TrainSpeed = (float)BS.ReadUInt8();
            }

            this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
        }
        public void WriteIncoming()
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, this.PacketId,
                ParamType.UInt16, this.VehicleId,
                ParamType.UInt16, this.LRKey,
                ParamType.UInt16, this.UDKey,
                ParamType.UInt16, this.Keys,
                ParamType.Float, this.Quaternion.W,
                ParamType.Float, this.Quaternion.Y,
                ParamType.Float, this.Quaternion.Z,
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z,
                ParamType.UInt16, this.FromPlayerId
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.Velocity.X,
                ParamType.Float, this.Velocity.Y,
                ParamType.Float, this.Velocity.Z,
                ParamType.Float, this.VehicleHealth,
                ParamType.UInt8, this.PlayerHealth,
                ParamType.UInt8, this.PlayerArmour,
                ParamType.Bits, this.AdditionalKey, 2,
                ParamType.Bits, this.WeaponId, 6,
                ParamType.UInt8, this.SirenState,
                ParamType.UInt8, this.LandingGearState,
                ParamType.UInt16, this.TrailerId,
                ParamType.Float, this.TrainSpeed
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteValue(
                ParamType.UInt8, this.PacketId,
                ParamType.UInt16, this.FromPlayerId,
                ParamType.UInt16, this.VehicleId,
                ParamType.UInt16, this.LRKey,
                ParamType.UInt16, this.UDKey,
                ParamType.UInt16, this.Keys
           );

            BS.WriteNormQuat(this.Quaternion);
            
            BS.WriteValue(
               ParamType.Float, this.Position.X,
               ParamType.Float, this.Position.Y,
               ParamType.Float, this.Position.Z
            );

            BS.WriteVector(this.Velocity);
            BS.WriteUInt16((int)this.VehicleHealth);

            byte healthArmour = HealthArmour.SetInByte(this.PlayerHealth, this.PlayerArmour);
            BS.WriteUInt8((int)healthArmour);
            BS.WriteUInt8(this.WeaponId);
            
            if (this.SirenState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            if (this.LandingGearState == 1)
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
