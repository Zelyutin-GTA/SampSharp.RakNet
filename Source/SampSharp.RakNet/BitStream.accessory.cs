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
            var result = ReadValue(ParamType.Int8, "param");
            return (int)result["param"];
        }
        public int ReadInt16()
        {
            var result = ReadValue(ParamType.Int16, "param");
            return (int)result["param"];
        }
        public int ReadInt32()
        {
            var result = ReadValue(ParamType.Int32, "param");
            return (int)result["param"];
        }
        public int ReadUInt8()
        {
            var result = ReadValue(ParamType.UInt8, "param");
            return (int)result["param"];
        }
        public int ReadUInt16()
        {
            var result = ReadValue(ParamType.UInt16, "param");
            return (int)result["param"];
        }
        public int ReadUInt32()
        {
            var result = ReadValue(ParamType.UInt32, "param");
            return (int)result["param"];
        }
        public float ReadFloat()
        {
            var result = ReadValue(ParamType.Float, "param");
            return (float)result["param"];
        }
        public bool ReadBool()
        {
            var result = ReadValue(ParamType.Bool, "param");
            return (bool)result["param"];
        }
        public string ReadString(int length)
        {
            var result = ReadValue(ParamType.String, "param", length);
            return (string)result["param"];
        }
        public int ReadBits(int count)
        {
            var result = ReadValue(ParamType.Bits, "param", count);
            return (int)result["param"];
        }
        #endregion
        
        #region Writings
        public void WriteInt8(int param)
        {
            WriteValue(ParamType.Int8, param);
        }
        public void WriteInt16(int param)
        {
            WriteValue(ParamType.Int16, param);
        }
        public void WriteInt32(int param)
        {
            WriteValue(ParamType.Int32, param);
        }
        public void WriteUInt8(int param)
        {
            WriteValue(ParamType.UInt8, param);
        }
        public void WriteUInt16(int param)
        {
            WriteValue(ParamType.UInt16, param);
        }
        public void WriteUInt32(int param)
        {
            WriteValue(ParamType.UInt32, param);
        }
        public void WriteFloat(float param)
        {
            WriteValue(ParamType.Float, param);
        }
        public void WriteBool(bool param)
        {
            WriteValue(ParamType.Bool, param);
        }
        public void WriteString(string param)
        {
            WriteValue(ParamType.String, param);
        }
        public void WriteBits(int param, int count)
        {
            WriteValue(ParamType.Bits, param, count);
        }
        #endregion
        
        #region Compressed readings
        public int ReadCompressedInt8()
        {
            var result = ReadValue(ParamType.CompressedInt8, "param");
            return (int)result["param"];
        }
        public int ReadCompressedInt16()
        {
            var result = ReadValue(ParamType.CompressedInt16, "param");
            return (int)result["param"];
        }
        public int ReadCompressedInt32()
        {
            var result = ReadValue(ParamType.CompressedInt32, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUInt8()
        {
            var result = ReadValue(ParamType.CompressedUInt8, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUInt16()
        {
            var result = ReadValue(ParamType.CompressedUInt16, "param");
            return (int)result["param"];
        }
        public int ReadCompressedUInt32()
        {
            var result = ReadValue(ParamType.CompressedUInt32, "param");
            return (int)result["param"];
        }
        public float ReadCompressedFloat()
        {
            var result = ReadValue(ParamType.CompressedFloat, "param");
            return (float)result["param"];
        }
        public bool ReadCompressedBool()
        {
            var result = ReadValue(ParamType.CompressedBool, "param");
            return (bool)result["param"];
        }
        public string ReadCompressedString(int length)
        {
            var result = ReadValue(ParamType.CompressedString, "param", length);
            return (string)result["param"];
        }
        #endregion
        
        #region Compressed writings
        public void WriteCompressedInt8(int param)
        {
            WriteValue(ParamType.CompressedInt8, param);
        }
        public void WriteCompressedInt16(int param)
        {
            WriteValue(ParamType.CompressedInt16, param);
        }
        public void WriteCompressedInt32(int param)
        {
            WriteValue(ParamType.CompressedInt32, param);
        }
        public void WriteCompressedUInt8(int param)
        {
            WriteValue(ParamType.CompressedUInt8, param);
        }
        public void WriteCompressedUInt16(int param)
        {
            WriteValue(ParamType.CompressedUInt16, param);
        }
        public void WriteCompressedUInt32(int param)
        {
            WriteValue(ParamType.CompressedUInt32, param);
        }
        public void WriteCompressedFloat(float param)
        {
            WriteValue(ParamType.CompressedFloat, param);
        }
        public void WriteCompressedBool(bool param)
        {
            WriteValue(ParamType.CompressedBool, param);
        }
        public void WriteCompressedString(string param)
        {
            WriteValue(ParamType.CompressedString, param);
        }
        #endregion

        #region Vector and Quat
        public void WriteVector(Vector3 vector)
        {
            float x = vector.X;
            float y = vector.Y;
            float z = vector.Z;

            float magnitude = (float)Math.Sqrt(x * x + y * y + z * z);
            WriteFloat(magnitude);

            if (magnitude > 0.0f)
            {
                WriteUInt16((int)((x / magnitude + 1.0f) * 32767.5f));
                WriteUInt16((int)((y / magnitude + 1.0f) * 32767.5f));
                WriteUInt16((int)((z / magnitude + 1.0f) * 32767.5f));
            }
        }
        public void WriteNormQuat(Vector4 quat)
        {
            float w = quat.W;
            float x = quat.X;
            float y = quat.Y;
            float z = quat.Z;

            WriteBool((bool)(w < 0.0f));
            WriteBool((bool)(x < 0.0f));
            WriteBool((bool)(y < 0.0f));
            WriteBool((bool)(z < 0.0f));
            WriteUInt16((int)(Math.Abs(x) * 65535.0));
            WriteUInt16((int)(Math.Abs(y) * 65535.0));
            WriteUInt16((int)(Math.Abs(z) * 65535.0));
            // Leave out w and calcuate it on the target
        }
        public Vector3 ReadVector()
        {
            float x;
            float y;
            float z;

            float magnitude;
            int sx, sy, sz;
            magnitude = ReadFloat();

            if (magnitude != 0.0f)
            {
                sx = ReadUInt16();
                sy = ReadUInt16();
                sz = ReadUInt16();

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
            cwNeg = ReadBool();
            cxNeg = ReadBool();
            cyNeg = ReadBool();
            czNeg = ReadBool();
            cx = ReadUInt16();
            cy = ReadUInt16();
            cz = ReadUInt16();

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
        #endregion
    }
}