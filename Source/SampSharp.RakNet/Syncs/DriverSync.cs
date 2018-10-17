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
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetID = (int)result["packetID"];

                this.vehicleID = (int)result["vehicleID"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
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

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        public void ReadOutcoming()
        {
            this.packetID = this.BS.ReadUint8();
            this.fromPlayerID = this.BS.ReadUint16();
            this.vehicleID = this.BS.ReadUint16();

            // LEFT/RIGHT KEYS
            this.lrKey = this.BS.ReadUint16();

            // UP/DOWN KEYS
            this.udKey = this.BS.ReadUint16();

            // GENERAL KEYS
            this.keys = this.BS.ReadUint16();
            
            // ROTATION
            this.quaternion = BS.ReadNormQuat();

            float x = BS.ReadFloat();
            float y = BS.ReadFloat();
            float z = BS.ReadFloat();
            this.position = new Vector3(x, y, z);

            this.velocity = BS.ReadVector();
            this.vehicleHealth = (float)BS.ReadUint16();

            byte healthArmour = Convert.ToByte(BS.ReadUint8());
            HealthArmour.GetFromByte(healthArmour, ref this.playerHealth, ref this.playerArmour);

            this.weaponID = BS.ReadUint8();

            bool sirenState = BS.ReadCompressedBool();
            if(sirenState)
                this.sirenState = 1;

            bool landingGear = BS.ReadCompressedBool();
            if (landingGear)
                this.landingGearState = 1;

            // HYDRA THRUST ANGLE AND TRAILER ID
            bool hydra = BS.ReadCompressedBool();
            bool trailer = BS.ReadCompressedBool();

            
            int trailerID_or_thrustAngle = BS.ReadUint32();
            bool train = BS.ReadCompressedBool();

            if (train)
            {
                this.trainSpeed = (float)BS.ReadUint8();
            }

            this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
        }
        public void WriteIncoming()
        {
            var arguments = new List<object>()
            {
                ParamType.UINT8, this.packetID,
                ParamType.UINT16, this.vehicleID,
                ParamType.UINT16, this.lrKey,
                ParamType.UINT16, this.udKey,
                ParamType.UINT16, this.keys,
                ParamType.FLOAT, this.quaternion.W,
                ParamType.FLOAT, this.quaternion.X,
                ParamType.FLOAT, this.quaternion.Y,
                ParamType.FLOAT, this.quaternion.Z,
                ParamType.FLOAT, this.position.X,
                ParamType.FLOAT, this.position.Y,
                ParamType.FLOAT, this.position.Z,
                ParamType.UINT16, this.fromPlayerID
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.FLOAT, this.velocity.X,
                ParamType.FLOAT, this.velocity.Y,
                ParamType.FLOAT, this.velocity.Z,
                ParamType.FLOAT, this.vehicleHealth,
                ParamType.UINT8, this.playerHealth,
                ParamType.UINT8, this.playerArmour,
                ParamType.BITS, this.additionalKey, 2,
                ParamType.BITS, this.weaponID, 6,
                ParamType.UINT8, this.sirenState,
                ParamType.UINT8, this.landingGearState,
                ParamType.UINT16, this.trailerID,
                ParamType.FLOAT, this.trainSpeed
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteValue(
                ParamType.UINT8, this.packetID,
                ParamType.UINT16, this.fromPlayerID,
                ParamType.UINT16, this.vehicleID,
                ParamType.UINT16, this.lrKey,
                ParamType.UINT16, this.udKey,
                ParamType.UINT16, this.keys
           );

            BS.WriteNormQuat(this.quaternion);
            
            BS.WriteValue(
               ParamType.FLOAT, this.position.X,
               ParamType.FLOAT, this.position.Y,
               ParamType.FLOAT, this.position.Z
            );

            BS.WriteVector(this.velocity);
            BS.WriteUint16((int)this.vehicleHealth);

            byte healthArmour = HealthArmour.SetInByte(this.playerHealth, this.playerArmour);
            BS.WriteUint8((int)healthArmour);
            BS.WriteUint8(this.weaponID);
            
            if (this.sirenState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            if (this.landingGearState == 1)
                BS.WriteBool(true);
            else
                BS.WriteBool(false);

            // HYDRA THRUST ANGLE AND TRAILER ID
            BS.WriteBool(false);
            BS.WriteBool(false);

            int trailerID_or_thrustAngle = 0;
            BS.WriteUint32(trailerID_or_thrustAngle);

            // TRAIN SPECIAL
            BS.WriteBool(false);
        }
    }
}
