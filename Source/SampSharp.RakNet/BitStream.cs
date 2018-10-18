using System;
using SampSharp.RakNet.Events;

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
            IsHandMade = isHandMade;
            Id = id;
        }
        public bool IsEmptyHandle()
        {
            return Id == 0;
        }
        public void Reset()
        {
            Internal.BS_Reset(Id);
        }
        public void ResetReadPointer()
        {
            Internal.BS_ResetReadPointer(Id);
        }
        public void ResetWritePointer()
        {
            Internal.BS_ResetWritePointer(Id);
        }
        public void IgnoreBits(int number)
        {
            Internal.BS_IgnoreBits(Id, number);
        }
        public int WriteOffset
        {
            get
            {
                Internal.BS_GetWriteOffset(Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetWriteOffset(Id, value);
            }
        }
        public int ReadOffset
        {
            get
            {
                Internal.BS_GetReadOffset(Id, out int offset);
                return offset;
            }
            set
            {
                Internal.BS_SetReadOffset(Id, value);
            }
        }

        public int NumberOfBitsUsed
        {
            get
            {
                Internal.BS_GetNumberOfBitsUsed(Id, out int number);
                return number;
            }
        }
        public int NumberOfBytesUsed
        {
            get
            {
                Internal.BS_GetNumberOfBytesUsed(Id, out int number);
                return number;
            }
        }
        public int NumberOfUnreadBits
        {
            get
            {
                Internal.BS_GetNumberOfUnreadBits(Id, out int number);
                return number;
            }
        }
        public int NumberOfBitsAllocated
        {
            get
            {
                Internal.BS_GetNumberOfBitsAllocated(Id, out int number);
                return number;
            }
        }
        public void WriteValue(params object[] arguments)
        {
            Internal.BS_WriteValue(Id, arguments);
        }
        public void ReadValue(params object[] arguments)
        {
            var values = Internal.BS_ReadValue(Id, arguments);
            var e = new BitStreamReadEventArgs(values);
            ReadCompleted?.Invoke(this, e);
        }
        
        public void Dispose()
        {
            if (IsHandMade)
            {
                Internal.BS_Delete(out int id);
                Id = id;
            }
        }
        public static BitStream Create()
        {
            int id = Internal.BS_New();
            return new BitStream(id, true);
        }

        public event EventHandler<BitStreamReadEventArgs> ReadCompleted;
    }
}
