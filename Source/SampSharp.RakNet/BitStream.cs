using System;
using SampSharp.RakNet.Events;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet
{
    public partial class BitStream : IDisposable
    {
        public int Id { get; private set; }
        public bool IsHandMade{ get; private set; }
        public BitStream(int id, bool isHandMade = false)
        {
            if(id == 0)
            {
                Console.WriteLine("[SampSharp.RakNet][Warning] Trying to create BitStream with id(handle) = 0");
            }
            this.IsHandMade = isHandMade;
            this.Id = id;
        }
        public bool IsEmptyHandle()
        {
            return this.Id == 0;
        }
        public void Reset()
        {
            Internal.BS_Reset(this.Id);
        }
        public void ResetReadPointer()
        {
            Internal.BS_ResetReadPointer(this.Id);
        }
        public void ResetWritePointer()
        {
            Internal.BS_ResetWritePointer(this.Id);
        }
        public void IgnoreBits(int number)
        {
            Internal.BS_IgnoreBits(this.Id, number);
        }
        public int WriteOffset
        {
            get
            {
                Internal.BS_GetWriteOffset(this.Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetWriteOffset(this.Id, value);
            }
        }
        public int ReadOffset
        {
            get
            {
                Internal.BS_GetReadOffset(this.Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetReadOffset(this.Id, value);
            }
        }

        public int NumberOfBitsUsed
        {
            get
            {
                Internal.BS_GetNumberOfBitsUsed(this.Id, out int number);
                return number;
            }
        }
        public int NumberOfBytesUsed
        {
            get
            {
                Internal.BS_GetNumberOfBytesUsed(this.Id, out int number);
                return number;
            }
        }
        public int NumberOfUnreadBits
        {
            get
            {
                Internal.BS_GetNumberOfUnreadBits(this.Id, out int number);
                return number;
            }
        }
        public int NumberOfBitsAllocated
        {
            get
            {
                Internal.BS_GetNumberOfBitsAllocated(this.Id, out int number);
                return number;
            }
        }
        public void WriteValue(params object[] arguments)
        {
            Internal.BS_WriteValue(this.Id, arguments);
        }
        public void ReadValue(params object[] arguments)
        {
            var values = Internal.BS_ReadValue(this.Id, arguments);
            var e = new BitStreamReadEventArgs(values);
            this.ReadCompleted?.Invoke(this, e);
        }

        public void SendRPC(int rpcId, int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            var result = Internal.BS_RPC(this.Id, playerId, rpcId, (int)priority, (int)reliability);
        }
        public void SendPacket(int playerId, PacketPriority priority = PacketPriority.HighPriority, PacketReliability reliability = PacketReliability.ReliableOrdered)
        {
            var result = Internal.BS_Send(this.Id, playerId, (int)priority, (int)reliability);
        }

        public void Dispose()
        {
            int id = this.Id; // Added to let this.Id stay readonly (or with private setter)
            if(this.IsHandMade) Internal.BS_Delete(out id);
            this.Id = id;
        }
        public static BitStream New()
        {
            int id = Internal.BS_New();
            return new BitStream(id, true);
        }

        public event EventHandler<BitStreamReadEventArgs> ReadCompleted;
    }
}
