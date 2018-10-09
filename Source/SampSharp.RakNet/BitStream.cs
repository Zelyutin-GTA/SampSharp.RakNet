using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet
{
    partial class BitStream : IDisposable
    {
        public int ID { get; private set; }
        public BitStream(int id)
        {
            this.ID = id;
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
    }
}
