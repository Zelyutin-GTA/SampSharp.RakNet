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

        public BitStream BS { get; set; }

        public int PacketId { get; set; }
        public int FromPlayerId { get; set; }
        public int LRKey { get; set; }
        public int UDKey { get; set; }
        public int Keys { get; set; }
        public Vector3 Position { get; set; }
        public Vector4 Quaternion { get; set; }
        public int Health { get; set; }
        public int Armour { get; set; }
        public int AdditionalKey { get; set; }
        public int WeaponId { get; set; }
        public int SpecialAction { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 SurfingOffsets { get; set; }
        public int SurfingVehicleId { get; set; }
        public int AnimationId { get; set; }
        public int AnimationFlags { get; set; }

        public OnFootSync(BitStream bs)
        {
            this.BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                this.PacketId = (int)result["packetId"];
                this.LRKey = (int)result["lrKey"];
                this.UDKey = (int)result["udKey"];
                this.Keys = (int)result["keys"];
                this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                this.Health = (int)result["health"];
                this.Armour = (int)result["armour"];
                this.AdditionalKey = (int)result["additionalKey"];

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    this.WeaponId = (int)result["weaponId"];
                    this.SpecialAction = (int)result["specialAction"];
                    this.Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    this.SurfingOffsets = new Vector3((float)result["surfingOffsets_0"], (float)result["surfingOffsets_1"], (float)result["surfingOffsets_2"]);
                    this.SurfingVehicleId = (int)result["surfingVehicleId"];
                    this.AnimationId = (int)result["animationId"];
                    this.AnimationFlags = (int)result["animationFlags"];

                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.Bits, "weaponId", 6,
                    ParamType.UInt8, "specialAction",
                    ParamType.Float, "velocity_0",
                    ParamType.Float, "velocity_1",
                    ParamType.Float, "velocity_2",
                    ParamType.Float, "surfingOffsets_0",
                    ParamType.Float, "surfingOffsets_1",
                    ParamType.Float, "surfingOffsets_2",
                    ParamType.UInt16, "surfingVehicleId",
                    ParamType.Int16, "animationId",
                    ParamType.Int16, "animationFlags"
                );
            };

            var arguments = new List<object>()
            {
                ParamType.UInt8, "packetId",
                ParamType.UInt16, "lrKey",
                ParamType.UInt16, "udKey",
                ParamType.UInt16, "keys",
                ParamType.Float, "position_0",
                ParamType.Float, "position_1",
                ParamType.Float, "position_2",
                ParamType.Float, "quaternion_W",
                ParamType.Float, "quaternion_X",
                ParamType.Float, "quaternion_Y",
                ParamType.Float, "quaternion_Z",
                ParamType.UInt8, "health",
                ParamType.UInt8, "armour",
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
                this.Keys = (int)result["keys"];
                this.Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                this.Quaternion = BS.ReadNormQuat();

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    byte healthArmour = Convert.ToByte(((int)result["healthArmourByte"]));
                    HealthArmour.GetFromByte(healthArmour, out int health, out int armour);
                    this.Health = health;
                    this.Armour = armour;
                    this.WeaponId = (int)result["weaponId"];
                    this.SpecialAction = (int)result["specialAction"];
                    this.Velocity = BS.ReadVector();

                    bool hasSurfInfo = BS2.ReadBool();
                    if(hasSurfInfo)
                    {
                        this.SurfingVehicleId = BS2.ReadUInt16();
                        float offsetsX = BS2.ReadFloat();
                        float offsetsY = BS2.ReadFloat();
                        float offsetsZ = BS2.ReadFloat();
                        this.SurfingOffsets = new Vector3(offsetsX, offsetsY, offsetsZ);
                    }
                    else
                    {
                        this.SurfingVehicleId = -1;
                    }

                    bool hasAnimation = BS2.ReadBool();
                    if(hasAnimation)
                    {
                        this.AnimationId = BS2.ReadInt32();
                    }
                    
                    this.ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.UInt8, "healthArmourByte",
                    ParamType.UInt8, "weaponId",
                    ParamType.UInt8, "specialAction"
                );
            };

            this.PacketId = this.BS.ReadUInt8();
            this.FromPlayerId = this.BS.ReadUInt16();

            //LEFT/RIGHT KEYS
            bool hasLR = this.BS.ReadBool();
            if (hasLR) this.LRKey = this.BS.ReadUInt16();

            // UP/DOWN KEYS
            bool hasUD = this.BS.ReadBool();
            if (hasUD) this.UDKey = this.BS.ReadUInt16();

            var arguments = new List<object>()
            {
                ParamType.UInt16, "keys",
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
                ParamType.UInt8, this.PacketId,
                ParamType.UInt16, this.LRKey,
                ParamType.UInt16, this.UDKey,
                ParamType.UInt16, this.Keys,
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z,
                ParamType.Float, this.Quaternion.W,
                ParamType.Float, this.Quaternion.X,
                ParamType.Float, this.Quaternion.Y,
                ParamType.Float, this.Quaternion.Z,
                ParamType.UInt8, this.Health,
                ParamType.UInt8, this.Armour,
                ParamType.Bits, this.AdditionalKey, 2
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Bits, this.WeaponId, 6,
                ParamType.UInt8, this.SpecialAction,
                ParamType.Float, this.Velocity.X,
                ParamType.Float, this.Velocity.Y,
                ParamType.Float, this.Velocity.Z,
                ParamType.Float, this.SurfingOffsets.X,
                ParamType.Float, this.SurfingOffsets.Y,
                ParamType.Float, this.SurfingOffsets.Z,
                ParamType.UInt16, this.SurfingVehicleId,
                ParamType.Int16, this.AnimationId,
                ParamType.Int16, this.AnimationFlags
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteUInt8(this.PacketId);
            BS.WriteUInt16(this.FromPlayerId);
            if(this.LRKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUInt16(this.LRKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            if (this.UDKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUInt16(this.UDKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            BS.WriteValue(
                ParamType.UInt16, this.Keys,
                ParamType.Float, this.Position.X,
                ParamType.Float, this.Position.Y,
                ParamType.Float, this.Position.Z
            );
        
            BS.WriteNormQuat(this.Quaternion);

            byte healthArmourByte = HealthArmour.SetInByte(this.Health, this.Armour);
            BS.WriteValue(
                ParamType.UInt8, (int)healthArmourByte,
                ParamType.UInt8, this.WeaponId,
                ParamType.UInt8, this.SpecialAction
            );
            BS.WriteVector(this.Velocity);
            if(this.SurfingVehicleId != 0)
            {
                BS.WriteValue(
                    ParamType.Bool, true,
                    ParamType.UInt8, this.SurfingVehicleId,
                    ParamType.Float, this.SurfingOffsets.X,
                    ParamType.Float, this.SurfingOffsets.Y,
                    ParamType.Float, this.SurfingOffsets.Z
                );
            }
            else
            {
                BS.WriteBool(false);
            }

            if(this.AnimationId != 0)
            {
                BS.WriteBool(true);
                BS.WriteInt32(this.AnimationId);
            }
            else
            {
                BS.WriteBool(false);
            }
        }
    }
}
