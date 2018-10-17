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
            var result = Internal.BS_ReadValue(this.ID, ParamType.INT8, "param");
            return (int)result["param"];
        }
        public int ReadInt16()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.INT16, "param");
            return (int)result["param"];
        }
        public int ReadInt32()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.INT32, "param");
            return (int)result["param"];
        }
        public int ReadUint8()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.UINT8, "param");
            return (int)result["param"];
        }
        public int ReadUint16()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.UINT16, "param");
            return (int)result["param"];
        }
        public int ReadUint32()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.UINT32, "param");
            return (int)result["param"];
        }
        public float ReadFloat()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.FLOAT, "param");
            return (float)result["param"];
        }
        public bool ReadBool()
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.BOOL, "param");
            return (bool)result["param"];
        }
        public string ReadString(int length)
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.STRING, "param", length);
            return (string)result["param"];
        }
        public int ReadBits(int count)
        {
            var result = Internal.BS_ReadValue(this.ID, ParamType.BITS, "param", count);
            return (int)result["param"];
        }
        #endregion
        #region Writings
        public void WriteInt8(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.INT8, param);
        }
        public void WriteInt16(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.INT16, param);
        }
        public void WriteInt32(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.INT32, param);
        }
        public void WriteUint8(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.UINT8, param);
        }
        public void WriteUint16(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.UINT16, param);
        }
        public void WriteUint32(int param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.UINT32, param);
        }
        public void WriteFloat(float param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.FLOAT, param);
        }
        public void WriteBool(bool param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.BOOL, param);
        }
        public void WriteString(string param)
        {
            Internal.BS_WriteValue(this.ID, ParamType.STRING, param);
        }
        public void WriteBits(int param, int count)
        {
            Internal.BS_WriteValue(this.ID, ParamType.BITS, param, count);
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