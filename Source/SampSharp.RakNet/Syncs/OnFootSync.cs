using System;
using System.Collections.Generic;

using SampSharp.GameMode;

using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet.Syncs
{
    public class OnFootSync : ISync
    {
        public event EventHandler<SyncReadEventArgs> ReadCompleted;

        public BitStream BS;

        public int packetId;
        public int fromPlayerId;
        public int lrKey;
        public int udKey;
        public int keys;
        public Vector3 position;
        public Vector4 quaternion;
        public int health;
        public int armour;
        public int additionalKey;
        public int weaponId;
        public int specialAction;
        public Vector3 velocity;
        public Vector3 surfingOffsets;
        public int surfingVehicleId;
        public int animationId;
        public int animationFlags;

        public OnFootSync(BitStream bs)
        {
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.packetId = (int)result["packetId"];
                this.lrKey = (int)result["lrKey"];
                this.udKey = (int)result["udKey"];
                this.keys = (int)result["keys"];
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.health = (int)result["health"];
                this.armour = (int)result["armour"];
                this.additionalKey = (int)result["additionalKey"];

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.weaponId = (int)result["weaponId"];
                    this.specialAction = (int)result["specialAction"];
                    this.velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.surfingOffsets = new Vector3((float)result["surfingOffsets_0"], (float)result["surfingOffsets_1"], (float)result["surfingOffsets_2"]);
                    this.surfingVehicleId = (int)result["surfingVehicleId"];
                    this.animationId = (int)result["animationId"];
                    this.animationFlags = (int)result["animationFlags"];

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Bits, "weaponId", 6,
                    ParamType.Uint8, "specialAction",
                    ParamType.Float, "velocity_0",
                    ParamType.Float, "velocity_1",
                    ParamType.Float, "velocity_2",
                    ParamType.Float, "surfingOffsets_0",
                    ParamType.Float, "surfingOffsets_1",
                    ParamType.Float, "surfingOffsets_2",
                    ParamType.Uint16, "surfingVehicleId",
                    ParamType.Int16, "animationId",
                    ParamType.Int16, "animationFlags"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.Uint8, "packetId",
                ParamType.Uint16, "lrKey",
                ParamType.Uint16, "udKey",
                ParamType.Uint16, "keys",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",
                ParamType.Float, "quaternion_W",
                ParamType.Float, "quaternion_X",
                ParamType.Float, "quaternion_Y",
                ParamType.Float, "quaternion_Z",
                ParamType.Uint8, "health",
                ParamType.Uint8, "armour",
                ParamType.Bits, "additionalKey", 2
            };

            BS.ReadValue(arguments.ToArray());
            //Need to divide up the reading cause of native arguments limit(32) in SampSharp.
        }
        public void ReadOutcoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.keys = (int)result["keys"];
                this.position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.quaternion = BS.ReadNormQuat();

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    byte healthArmour = Convert.ToByte(((int)result["healthArmourByte"]));
                    HealthArmour.GetFromByte(healthArmour, ref this.health, ref this.armour);
                    this.weaponId = (int)result["weaponId"];
                    this.specialAction = (int)result["specialAction"];
                    this.velocity = BS.ReadVector();

                    bool hasSurfInfo = BS2.ReadBool();
                    if(hasSurfInfo)
                    {
                        this.surfingVehicleId = BS2.ReadUint16();
                        float offsetsX = BS2.ReadFloat();
                        float offsetsY = BS2.ReadFloat();
                        float offsetsZ = BS2.ReadFloat();
                        this.surfingOffsets = new Vector3(offsetsX, offsetsY, offsetsZ);
                    }
                    else
                    {
                        this.surfingVehicleId = -1;
                    }

                    bool hasAnimation = BS2.ReadBool();
                    if(hasAnimation)
                    {
                        this.animationId = BS2.ReadInt32();
                    }
                    
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Uint8, "healthArmourByte",
                    ParamType.Uint8, "weaponId",
                    ParamType.Uint8, "specialAction"
                );
            };

            this.packetId = this.BS.ReadUint8();
            this.fromPlayerId = this.BS.ReadUint16();

            //LEFT/RIGHT KEYS
            bool hasLR = this.BS.ReadBool();
            if (hasLR) this.lrKey = this.BS.ReadUint16();

            // UP/DOWN KEYS
            bool hasUD = this.BS.ReadBool();
            if (hasUD) this.udKey = this.BS.ReadUint16();

            var arguments = new List<object>()
            {
                ParamType.Uint16, "keys",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2"
            };

            BS.ReadValue(arguments.ToArray());
        }
        public void WriteIncoming()
        {
            var arguments = new List<object>()
            {
                ParamType.Uint8, this.packetId,
                ParamType.Uint16, this.lrKey,
                ParamType.Uint16, this.udKey,
                ParamType.Uint16, this.keys,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z,
                ParamType.Float, this.quaternion.W,
                ParamType.Float, this.quaternion.X,
                ParamType.Float, this.quaternion.Y,
                ParamType.Float, this.quaternion.Z,
                ParamType.Uint8, this.health,
                ParamType.Uint8, this.armour,
                ParamType.Bits, this.additionalKey, 2
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Bits, this.weaponId, 6,
                ParamType.Uint8, this.specialAction,
                ParamType.Float, this.velocity.X,
                ParamType.Float, this.velocity.Y,
                ParamType.Float, this.velocity.Z,
                ParamType.Float, this.surfingOffsets.X,
                ParamType.Float, this.surfingOffsets.Y,
                ParamType.Float, this.surfingOffsets.Z,
                ParamType.Uint16, this.surfingVehicleId,
                ParamType.Int16, this.animationId,
                ParamType.Int16, this.animationFlags
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteUint8(this.packetId);
            BS.WriteUint16(this.fromPlayerId);
            if(this.lrKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUint16(this.lrKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            if (this.udKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUint16(this.udKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            BS.WriteValue(
                ParamType.Uint16, this.keys,
                ParamType.Float, this.position.X,
                ParamType.Float, this.position.Y,
                ParamType.Float, this.position.Z
            );
        
            BS.WriteNormQuat(this.quaternion);

            byte healthArmourByte = HealthArmour.SetInByte(this.health, this.armour);
            BS.WriteValue(
                ParamType.Uint8, (int)healthArmourByte,
                ParamType.Uint8, this.weaponId,
                ParamType.Uint8, this.specialAction
            );
            BS.WriteVector(this.velocity);
            if(this.surfingVehicleId != 0)
            {
                BS.WriteValue(
                    ParamType.Bool, true,
                    ParamType.Uint8, this.surfingVehicleId,
                    ParamType.Float, this.surfingOffsets.X,
                    ParamType.Float, this.surfingOffsets.Y,
                    ParamType.Float, this.surfingOffsets.Z
                );
            }
            else
            {
                BS.WriteBool(false);
            }

            if(this.animationId != 0)
            {
                BS.WriteBool(true);
                BS.WriteInt32(this.animationId);
            }
            else
            {
                BS.WriteBool(false);
            }
        }
    }
}
