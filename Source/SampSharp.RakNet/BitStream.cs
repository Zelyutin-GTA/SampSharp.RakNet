using System;
using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet
{
    public partial class BitStream : IDisposable
    {
        public int ID { get; private set; }
        public BitStream(int id)
        {
            if(id == 0)
            {
                Console.WriteLine("[SampSharp.RakNet][Warning] Trying to create BitStream with id(handle) = 0");
            }
            this.ID = id;
        }
        public bool IsEmptyHandle()
        {
            return this.ID == 0;
        }
        public void Reset()
        {
            Internal.BS_Reset(this.ID);
        }
        public void ResetReadPointer()
        {
            Internal.BS_ResetReadPointer(this.ID);
        }
        public void ResetWritePointer()
        {
            Internal.BS_ResetWritePointer(this.ID);
        }
        public void IgnoreBits(int number)
        {
            Internal.BS_IgnoreBits(this.ID, number);
        }
        public int WriteOffset
        {
            get
            {
                Internal.BS_GetWriteOffset(this.ID, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetWriteOffset(this.ID, value);
            }
        }
        public int ReadOffset
        {
            get
            {
                Internal.BS_GetReadOffset(this.ID, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetReadOffset(this.ID, value);
            }
        }

        public int NumberOfBitsUsed
        {
            get
            {
                Internal.BS_GetNumberOfBitsUsed(this.ID, out int number);
                return number;
            }
        }
        public int NumberOfBytesUsed
        {
            get
            {
                Internal.BS_GetNumberOfBytesUsed(this.ID, out int number);
                return number;
            }
        }
        public int NumberOfUnreadBits
        {
            get
            {
                Internal.BS_GetNumberOfUnreadBits(this.ID, out int number);
                return number;
            }
        }
        public int NumberOfBitsAllocated
        {
            get
            {
                Internal.BS_GetNumberOfBitsAllocated(this.ID, out int number);
                return number;
            }
        }
        public void WriteValue(params object[] arguments)
        {
            Internal.BS_WriteValue(this.ID, arguments);
        }
        public void TestWriteValue(ParamType playerIDType, int playerID, ParamType nameLenType, int nameLen, ParamType nameType, string name)
        {
            var int_playerIDType = (int)playerIDType;
            var int_nameLenType = (int)nameLenType;
            var int_nameType = (int)nameType;
            Console.WriteLine("Len: "+nameLen);


            Internal.Test_WriteValue(this.ID, ref int_playerIDType, ref playerID, ref int_nameLenType, ref nameLen, ref int_nameType, ref name);
        }
        public void ReadValue(params object[] arguments)
        {
            var values = Internal.BS_ReadValue(this.ID, arguments);
            var e = new BitStreamReadEventArgs(values);
            this.ReadCompleted?.Invoke(this, e);
        }

        public void SendRPC(int rpcID, int playerID, PacketPriority priority = PacketPriority.HIGH_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED)
        {
            var result = Internal.BS_RPC(this.ID, playerID, rpcID, (int)priority, (int)reliability);
            Console.WriteLine("Send RPC Result: "+result);
        }

        public void Dispose()
        {
            int id = this.ID; // Added to let this.ID stay readonly (or with private setter)
            Internal.BS_Delete(out id);
            this.ID = id;
        }
        public static BitStream New()
        {
            int id = Internal.BS_New();
            return new BitStream(id);
        }

        public event EventHandler<BitStreamReadEventArgs> ReadCompleted;
    }
}
