using SampSharp.GameMode;

using SampSharp.RakNet.Definitions;
using System;

namespace SampSharp.RakNet
{
    partial class BitStream
    {
        #region Readings
        public int ReadInt8()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Int8, "param");
            return (int)result["param"];
        }
        public int ReadInt16()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Int16, "param");
            return (int)result["param"];
        }
        public int ReadInt32()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Int32, "param");
            return (int)result["param"];
        }
        public int ReadUint8()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Uint8, "param");
            return (int)result["param"];
        }
        public int ReadUint16()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Uint16, "param");
            return (int)result["param"];
        }
        public int ReadUint32()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Uint32, "param");
            return (int)result["param"];
        }
        public float ReadFloat()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Float, "param");
            return (float)result["param"];
        }
        public bool ReadBool()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Bool, "param");
            return (bool)result["param"];
        }
        public string ReadString(int length)
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.String, "param", length);
            return (string)result["param"];
        }
        public int ReadBits(int count)
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.Bits, "param", count);
            return (int)result["param"];
        }
        #endregion
        #region Writings
        public void WriteInt8(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Int8, param);
        }
        public void WriteInt16(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Int16, param);
        }
        public void WriteInt32(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Int32, param);
        }
        public void WriteUint8(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Uint8, param);
        }
        public void WriteUint16(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Uint16, param);
        }
        public void WriteUint32(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Uint32, param);
        }
        public void WriteFloat(float param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Float, param);
        }
        public void WriteBool(bool param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Bool, param);
        }
        public void WriteString(string param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.String, param);
        }
        public void WriteBits(int param, int count)
        {
            Internal.BS_WriteValue(this.Id, ParamType.Bits, param, count);
        }
        #endregion
        #region Compressed readings
        public int ReadCompressedInt8()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedInt8, "param");
            return (int)result["param"];
        }
        public int ReadCompressedInt16()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedInt16, "param");
            return (int)result["param"];
        }
        public int ReadCompressedInt32()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedInt32, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUint8()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedUInt8, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUint16()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedUInt16, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUint32()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedUInt32, "param");
            return (int)result["param"];
        }
        public float ReadCompressedFloat()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedFloat, "param");
            return (float)result["param"];
        }
        public bool ReadCompressedBool()
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedBool, "param");
            return (bool)result["param"];
        }
        public string ReadCompressedString(int length)
        {
            var result = Internal.BS_ReadValue(this.Id, ParamType.CompressedString, "param", length);
            return (string)result["param"];
        }
        #endregion
        #region Compressed writings
        public void WriteCompressedInt8(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedInt8, param);
        }
        public void WriteCompressedInt16(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedInt16, param);
        }
        public void WriteCompressedInt32(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedInt32, param);
        }
        public void WriteCompressedUint8(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedUInt8, param);
        }
        public void WriteCompressedUint16(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedUInt16, param);
        }
        public void WriteCompressedUint32(int param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedUInt32, param);
        }
        public void WriteCompressedFloat(float param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedFloat, param);
        }
        public void WriteCompressedBool(bool param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedBool, param);
        }
        public void WriteCompressedString(string param)
        {
            Internal.BS_WriteValue(this.Id, ParamType.CompressedString, param);
        }
        #endregion

        public void WriteVector(Vector3 vector)
        {
            float x = vector.X;
            float y = vector.Y;
            float z = vector.Z;

            float magnitude = (float)Math.Sqrt(x * x + y * y + z * z);
            WriteFloat(magnitude);

            if (magnitude > 0.0f)
            {
                this.WriteUint16((int)((x / magnitude + 1.0f) * 32767.5f));
                this.WriteUint16((int)((y / magnitude + 1.0f) * 32767.5f));
                this.WriteUint16((int)((z / magnitude + 1.0f) * 32767.5f));
            }
        }
        public void WriteNormQuat(Vector4 quat)
        {
            float w = quat.W;
            float x = quat.X;
            float y = quat.Y;
            float z = quat.Z;

            this.WriteBool((bool)(w < 0.0f));
            this.WriteBool((bool)(x < 0.0f));
            this.WriteBool((bool)(y < 0.0f));
            this.WriteBool((bool)(z < 0.0f));
            this.WriteUint16((int)(Math.Abs(x) * 65535.0));
            this.WriteUint16((int)(Math.Abs(y) * 65535.0));
            this.WriteUint16((int)(Math.Abs(z) * 65535.0));
            // Leave out w and calcuate it on the target
        }
        public Vector3 ReadVector()
        {
            float x;
            float y;
            float z;

            float magnitude;
            int sx, sy, sz;
            magnitude = this.ReadFloat();

            if (magnitude != 0.0f)
            {
                sx = ReadUint16();
                sy = ReadUint16();
                sz = ReadUint16();

                x = ((float)sx / 32767.5f - 1.0f) * magnitude;
                y = ((float)sy / 32767.5f - 1.0f) * magnitude;
                z = ((float)sz / 32767.5f - 1.0f) * magnitude;
            }
            else
            {
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
            }
            return new Vector3(x, y, z);
        }
        public Vector4 ReadNormQuat()
        {
            float w;
            float x;
            float y;
            float z;

            bool cwNeg, cxNeg, cyNeg, czNeg;
            int cx, cy, cz;
            cwNeg = this.ReadBool();
            cxNeg = this.ReadBool();
            cyNeg = this.ReadBool();
            czNeg = this.ReadBool();
            cx = this.ReadUint16();
            cy = this.ReadUint16();
            cz = this.ReadUint16();

            // Calculate w from x,y,z
            x = cx / 65535.0f;
            y = cy / 65535.0f;
            z = cz / 65535.0f;
            if (cxNeg) x = -x;
            if (cyNeg) y = -y;
            if (czNeg) z = -z;
            w = (float)Math.Sqrt(1.0f - x * x - y * y - z * z);
            if (cwNeg)
                w = -w;
            return new Vector4(x, y, z, w);
        }
    }
}