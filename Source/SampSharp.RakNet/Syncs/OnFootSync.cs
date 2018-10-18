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
            BS = bs;
        }
        public void ReadIncoming()
        {
            BS.ReadCompleted += (sender, args) =>
            {
                var result = args.Result;
                PacketId = (int)result["packetId"];
                LRKey = (int)result["lrKey"];
                UDKey = (int)result["udKey"];
                Keys = (int)result["keys"];
                Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                Quaternion = new Vector4((float)result["quaternion_X"], (float)result["quaternion_Y"], (float)result["quaternion_Z"], (float)result["quaternion_W"]); // order is different from one in a bitstream
                Health = (int)result["health"];
                Armour = (int)result["armour"];
                AdditionalKey = (int)result["additionalKey"];

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;
                    WeaponId = (int)result["weaponId"];
                    SpecialAction = (int)result["specialAction"];
                    Velocity = new Vector3((float)result["velocity_0"], (float)result["velocity_1"], (float)result["velocity_2"]);
                    SurfingOffsets = new Vector3((float)result["surfingOffsets_0"], (float)result["surfingOffsets_1"], (float)result["surfingOffsets_2"]);
                    SurfingVehicleId = (int)result["surfingVehicleId"];
                    AnimationId = (int)result["animationId"];
                    AnimationFlags = (int)result["animationFlags"];

                    ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
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
                Keys = (int)result["keys"];
                Position = new Vector3((float)result["position_0"], (float)result["position_1"], (float)result["position_2"]);
                Quaternion = BS.ReadNormQuat();

                var BS2 = new BitStream(BS.Id);
                BS2.ReadCompleted += (sender2, args2) =>
                {
                    result = args2.Result;

                    byte healthArmour = Convert.ToByte(((int)result["healthArmourByte"]));
                    HealthArmour.GetFromByte(healthArmour, out int health, out int armour);
                    Health = health;
                    Armour = armour;
                    WeaponId = (int)result["weaponId"];
                    SpecialAction = (int)result["specialAction"];
                    Velocity = BS.ReadVector();

                    bool hasSurfInfo = BS2.ReadBool();
                    if(hasSurfInfo)
                    {
                        SurfingVehicleId = BS2.ReadUInt16();
                        float offsetsX = BS2.ReadFloat();
                        float offsetsY = BS2.ReadFloat();
                        float offsetsZ = BS2.ReadFloat();
                        SurfingOffsets = new Vector3(offsetsX, offsetsY, offsetsZ);
                    }
                    else
                    {
                        SurfingVehicleId = -1;
                    }

                    bool hasAnimation = BS2.ReadBool();
                    if(hasAnimation)
                    {
                        AnimationId = BS2.ReadInt32();
                    }
                    
                    ReadCompleted.Invoke(this, new SyncReadEventArgs(this));
                };

                BS2.ReadValue(
                    ParamType.UInt8, "healthArmourByte",
                    ParamType.UInt8, "weaponId",
                    ParamType.UInt8, "specialAction"
                );
            };

            PacketId = BS.ReadUInt8();
            FromPlayerId = BS.ReadUInt16();

            //LEFT/RIGHT KEYS
            bool hasLR = BS.ReadBool();
            if (hasLR) LRKey = BS.ReadUInt16();

            // UP/DOWN KEYS
            bool hasUD = BS.ReadBool();
            if (hasUD) UDKey = BS.ReadUInt16();

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
                ParamType.UInt8, PacketId,
                ParamType.UInt16, LRKey,
                ParamType.UInt16, UDKey,
                ParamType.UInt16, Keys,
                ParamType.Float, Position.X,
                ParamType.Float, Position.Y,
                ParamType.Float, Position.Z,
                ParamType.Float, Quaternion.W,
                ParamType.Float, Quaternion.X,
                ParamType.Float, Quaternion.Y,
                ParamType.Float, Quaternion.Z,
                ParamType.UInt8, Health,
                ParamType.UInt8, Armour,
                ParamType.Bits, AdditionalKey, 2
            };

            BS.WriteValue(arguments.ToArray());

            arguments = new List<object>()
            {
                ParamType.Bits, WeaponId, 6,
                ParamType.UInt8, SpecialAction,
                ParamType.Float, Velocity.X,
                ParamType.Float, Velocity.Y,
                ParamType.Float, Velocity.Z,
                ParamType.Float, SurfingOffsets.X,
                ParamType.Float, SurfingOffsets.Y,
                ParamType.Float, SurfingOffsets.Z,
                ParamType.UInt16, SurfingVehicleId,
                ParamType.Int16, AnimationId,
                ParamType.Int16, AnimationFlags
            };

            BS.WriteValue(arguments.ToArray());
        }
        public void WriteOutcoming()
        {
            BS.WriteUInt8(PacketId);
            BS.WriteUInt16(FromPlayerId);
            if(LRKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUInt16(LRKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            if (UDKey != 0)
            {
                BS.WriteBool(true);
                BS.WriteUInt16(UDKey);
            }
            else
            {
                BS.WriteBool(false);
            }

            BS.WriteValue(
                ParamType.UInt16, Keys,
                ParamType.Float, Position.X,
                ParamType.Float, Position.Y,
                ParamType.Float, Position.Z
            );
        
            BS.WriteNormQuat(Quaternion);

            byte healthArmourByte = HealthArmour.SetInByte(Health, Armour);
            BS.WriteValue(
                ParamType.UInt8, (int)healthArmourByte,
                ParamType.UInt8, WeaponId,
                ParamType.UInt8, SpecialAction
            );
            BS.WriteVector(Velocity);
            if(SurfingVehicleId != 0)
            {
                BS.WriteValue(
                    ParamType.Bool, true,
                    ParamType.UInt8, SurfingVehicleId,
                    ParamType.Float, SurfingOffsets.X,
                    ParamType.Float, SurfingOffsets.Y,
                    ParamType.Float, SurfingOffsets.Z
                );
            }
            else
            {
                BS.WriteBool(false);
            }

            if(AnimationId != 0)
            {
                BS.WriteBool(true);
                BS.WriteInt32(AnimationId);
            }
            else
            {
                BS.WriteBool(false);
            }
        }
    }
}
