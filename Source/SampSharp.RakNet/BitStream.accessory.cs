using SampSharp.RakNet.Definitions;

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
    }
}