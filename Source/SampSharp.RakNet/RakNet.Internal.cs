using System;
using System.Collections.Generic;
using System.Text;

using SampSharp.Core.Natives.NativeObjects;

namespace SampSharp.RakNet
{
    public class BitStream
    {

    }
    public partial class RakNet
    {
        public enum PR_HandlerType
        {
            PR_INCOMING_RPC,
            PR_INCOMING_PACKET,
            PR_OUTCOMING_RPC,
            PR_OUTCOMING_PACKET,
            PR_NUMBER_OF_HANDLER_TYPES
        };

        public enum PR_ValueType
        {
            PR_INT8,
            PR_INT16,
            PR_INT32,
            PR_UINT8,
            PR_UINT16,
            PR_UINT32,
            PR_FLOAT,
            PR_BOOL,
            PR_STRING,

            // compressed
            PR_CINT8,
            PR_CINT16,
            PR_CINT32,
            PR_CUINT8,
            PR_CUINT16,
            PR_CUINT32,
            PR_CFLOAT,
            PR_CBOOL,
            PR_CSTRING,

            PR_BITS
        };

        public enum PR_PacketPriority
        {
            PR_SYSTEM_PRIORITY,
            PR_HIGH_PRIORITY,
            PR_MEDIUM_PRIORITY,
            PR_LOW_PRIORITY
        };

        public enum PR_PacketReliability
        {
            PR_UNRELIABLE = 6,
            PR_UNRELIABLE_SEQUENCED,
            PR_RELIABLE,
            PR_RELIABLE_ORDERED,
            PR_RELIABLE_SEQUENCED
        };

        protected static RakNetInternal Internal;

        static RakNet()
        {
            Internal = NativeObjectProxyFactory.CreateInstance<RakNetInternal>();
        }
        public class RakNetInternal
        {
            #region Pawn.RakNet natives

            #region Main
            [NativeMethod]
            public virtual int BS_New()
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_Delete(int bs) // TODO: bs is ref-type
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_RPC(int bs, int playerid, int rpcid, int priority = (int)PR_PacketPriority.PR_HIGH_PRIORITY, int reliability = (int)PR_PacketReliability.PR_RELIABLE_ORDERED) // priority and reliability -> enums
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_Send(int bs, int playerid, int priority = (int)PR_PacketPriority.PR_HIGH_PRIORITY, int reliability = (int)PR_PacketReliability.PR_RELIABLE_ORDERED) // priority and reliability -> enums
            {
                throw new NativeNotImplementedException();
            }
            #endregion


            #region Resets
            [NativeMethod]
            public virtual int BS_Reset(int bs)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_ResetReadPointer(int bs)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_ResetWritePointer(int bs)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_IgnoreBits(int bs, int number_of_bits)
            {
                throw new NativeNotImplementedException();
            }
            #endregion

            #region Offsets
            [NativeMethod]
            public virtual int BS_SetWriteOffset(int bs, int offset)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_GetWriteOffset(int bs, int offset) // TODO: offset ref-type
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_SetReadOffset(int bs, int offset)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_GetReadOffset(int bs, int offset) // TODO: offset ref-type
            {
                throw new NativeNotImplementedException();
            }
            #endregion

            #region
            [NativeMethod]
            public virtual int BS_GetNumberOfBitsUsed(int bs, int number) // TODO: number ref-type
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_GetNumberOfBytesUsed(int bs, int number) // TODO: number ref-type
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_GetNumberOfUnreadBits(int bs, int number) // TODO: number ref-type
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_GetNumberOfBitsAllocated(int bs, int number) // TODO: number ref-type
            {
                throw new NativeNotImplementedException();
            }
            #endregion

            #endregion
            #region Common Utils
            [NativeMethod]
            public virtual int CallRemoteFunction(string functionName, string argumentsFormat)
            {
                throw new NativeNotImplementedException();
            }
            #endregion


            // Raknet Natives:
            /*
            native BitStream:BS_New();
            native BS_Delete(&BitStream:bs);

            native BS_RPC(BitStream:bs, playerid, rpcid, PR_PacketPriority:priority = PR_HIGH_PRIORITY, PR_PacketReliability:reliability = PR_RELIABLE_ORDERED);
            native BS_Send(BitStream:bs, playerid, PR_PacketPriority:priority = PR_HIGH_PRIORITY, PR_PacketReliability:reliability = PR_RELIABLE_ORDERED);

            native BS_Reset(BitStream:bs);
            native BS_ResetReadPointer(BitStream:bs);
            native BS_ResetWritePointer(BitStream:bs);
            native BS_IgnoreBits(BitStream:bs, number_of_bits);

            native BS_SetWriteOffset(BitStream:bs, offset);
            native BS_GetWriteOffset(BitStream:bs, &offset);
            native BS_SetReadOffset(BitStream:bs, offset);
            native BS_GetReadOffset(BitStream:bs, &offset);

            native BS_GetNumberOfBitsUsed(BitStream:bs, &number);
            native BS_GetNumberOfBytesUsed(BitStream:bs, &number);
            native BS_GetNumberOfUnreadBits(BitStream:bs, &number);
            native BS_GetNumberOfBitsAllocated(BitStream:bs, &number);

            native BS_WriteValue(BitStream:bs, { PR_ValueType, Float, _}:...);
            native BS_ReadValue(BitStream:bs, { PR_ValueType, Float, _}:...);
            */
        }
    }
}
