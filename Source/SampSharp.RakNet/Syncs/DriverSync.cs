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

        public int packetId { get; set; }
        public int fromPlayerId { get; set; }
        public int vehicleId { get; set; }
        public int lrKey { get; set; }
        public int udKey { get; set; }
        public int keys { get; set; }
        public Vector4 quaternion { get; set; }
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public float vehicleHealth { get; set; }
        public int playerHealth { get; set; }
        public int playerArmour { get; set; }
        public int additionalKey { get; set; }
        public int weaponId { get; set; }
        public int sirenState { get; set; }
        public int landingGearState { get; set; }
        public int trailerId { get; set; }
        public float trainSpeed { get; set; }

        public DriverSync(BitStream bs)
        {
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetId = (int)result["packetId"];

                this.vehicleId = (int)result["vehicleId"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);


                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.vehicleHealth = (float)result["vehicleHealth"];
                    this.playerHealth = (int)result["playerHealth"];
                    this.playerArmour = (int)result["playerArmour"];
                    this.additionalKey = (int)result["additionalKey"];
                    this.weaponId = (int)result["weaponId"];
                    this.sirenState = (int)result["sirenState"];
                    this.landingGearState = (int)result["landingGearState"];
                    this.trailerId = (int)result["trailerId"];
                    this.trainSpeed = (float)result["trainSpeed"];

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
            this.packetId = this.BS.ReadUInt8();
            this.fromPlayerId = this.BS.ReadUInt16();
            this.vehicleId = this.BS.ReadUInt16();

            // LEFT/RIGHT KEYS
            this.lrKey = this.BS.ReadUInt16();

            // UP/DOWN KEYS
            this.udKey = this.BS.ReadUInt16();

            // GENERAL KEYS
            this.keys = this.BS.ReadUInt16();
            
            // ROTATION
            this.quaternion = BS.ReadNormQuat();

            float x = BS.ReadFloat();
            float y = BS.ReadFloat();
            float z = BS.ReadFloat();
            this.position = new Vector3(x, y, z);

            this.velocity = BS.ReadVector();
            this.vehicleHealth = (float)BS.ReadUInt16();

            byte healthArmour = Convert.ToByte(BS.ReadUInt8());
            HealthArmour.GetFromByte(healthArmour, ref this.playerHealth, ref this.playerArmour);

            this.weaponId = BS.ReadUInt8();

            bool sirenState = BS.ReadCompressedBool();
            if(sirenState)
                this.sirenState = 1;

            bool landingGear = BS.ReadCompressedBool();
            if (landingGear)
                this.landingGearState = 1;

            // HYDRA THRUST ANGLE AND TRAILER Id
            bool hydra = BS.ReadCompressedBool();
            bool trailer = BS.ReadCompressedBool();

            
            int trailerId_or_thrustAngle = BS.ReadUInt32();
            bool train = BS.ReadCompressedBool();

            if (train)
            {
                this.trainSpeed = (float)BS.ReadUInt8();
            }

            this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
        }
        public void WriteIncoming()
        {
            var arguments = new List<object>()
            {
                ParamType.UInt8, this.packetId,
                ParamType.UInt16, this.vehicleId,
                ParamType.UInt16, this.lrKey,
                ParamType.UInt16, this.udKey,
                ParamType.UInt16, this.keys,
                ParamType.Float, this.quaternion.W,
                ParamType.Float, this.quaternion.X,
                ParamType.Float, this.quaternion.Y,
                ParamType.Float, this.quaternion.Z,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z,
                ParamType.UInt16, this.fromPlayerId
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Float, this.velocity.X,
                ParamType.Float, this.velocity.Y,
                ParamType.Float, this.velocity.Z,
                ParamType.Float, this.vehicleHealth,
                ParamType.UInt8, this.playerHealth,
                ParamType.UInt8, this.playerArmour,
                ParamType.Bits, this.additionalKey, 2,
                ParamType.Bits, this.weaponId, 6,
                ParamType.UInt8, this.sirenState,
                ParamType.UInt8, this.landingGearState,
                ParamType.UInt16, this.trailerId,
                ParamType.Float, this.trainSpeed
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteValue(
                ParamType.UInt8, this.packetId,
                ParamType.UInt16, this.fromPlayerId,
                ParamType.UInt16, this.vehicleId,
                ParamType.UInt16, this.lrKey,
                ParamType.UInt16, this.udKey,
                ParamType.UInt16, this.keys
           );

            BS.WriteNormQuat(this.quaternion);
            
            BS.WriteValue(
               ParamType.Float, this.position.X,
               ParamType.Float, this.position.Y,
               ParamType.Float, this.position.Z
            );

            BS.WriteVector(this.velocity);
            BS.WriteUInt16((int)this.vehicleHealth);

            byte healthArmour = HealthArmour.SetInByte(this.playerHealth, this.playerArmour);
            BS.WriteUInt8((int)healthArmour);
            BS.WriteUInt8(this.weaponId);
            
            if (this.sirenState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            if (this.landingGearState == 1)
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
