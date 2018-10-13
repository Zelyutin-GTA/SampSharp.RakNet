using System;
using System.Collections.Generic;

using SampSharp.Core.Natives.NativeObjects;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet
{
    public partial class BitStream
    {
        #region BitStreamInternal
        protected static BitStreamInternal Internal;

        static BitStream()
        {
            Internal = NativeObjectProxyFactory.CreateInstance<BitStreamInternal>(); // TODO: change to class extension as in BaseModeInternal
        }
        public partial class BitStreamInternal
        {
            #region Pawn.RakNet BitStream natives

            #region Main
            [NativeMethod]
            public virtual int BS_New()
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_Delete(out int bs)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_RPC(int bs, int playerid, int rpcid, int priority, int reliability)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_Send(int bs, int playerid, int priority, int reliability)
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
            public virtual int BS_IgnoreBits(int bs, int numberOfBits)
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
            public virtual int BS_GetWriteOffset(int bs, out int offset)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_SetReadOffset(int bs, int offset)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_GetReadOffset(int bs, out int offset)
            {
                throw new NativeNotImplementedException();
            }
            #endregion

            #region
            [NativeMethod]
            public virtual int BS_GetNumberOfBitsUsed(int bs, out int number)
            {
                throw new NativeNotImplementedException();
            }
            [NativeMethod]
            public virtual int BS_GetNumberOfBytesUsed(int bs, out int number)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_GetNumberOfUnreadBits(int bs, out int number)
            {
                throw new NativeNotImplementedException();
            }

            [NativeMethod]
            public virtual int BS_GetNumberOfBitsAllocated(int bs, out int number)
            {
                throw new NativeNotImplementedException();
            }
            #endregion

            #region Floating params number Natives
            public virtual void BS_WriteValue(int bs, params object[] arguments)
            {
                var @params = PrepareParams(bs, false, arguments);
                var nativeParamsTypes = (Type[])@params[0];
                var nativeParams = (object[])@params[1];
                var nativeParamsSizes = (uint[])@params[2];

                var loader = RakNet.Client.NativeLoader;
                var NativeRead = loader.Load("BS_WriteValue", null, nativeParamsTypes);

                Console.WriteLine("ParamTypes:");
                Console.WriteLine($"Length: {nativeParamsTypes.Length}");
                foreach (var t in nativeParamsTypes)
                {
                    Console.WriteLine(t);
                }
                Console.WriteLine("Params:");
                Console.WriteLine($"Length: {nativeParams.Length}");
                foreach (var t in nativeParams)
                {
                    Console.WriteLine(t);
                }
                Console.WriteLine("Params sizes:");
                foreach (var t in nativeParamsSizes)
                {
                    Console.WriteLine(t);
                }

                var result = NativeRead.Invoke(nativeParams);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("WriteValue result: "+result);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            public virtual Dictionary<string, object> BS_ReadValue(int bs, params object[] arguments)
            {

                var @params = PrepareParams(bs, true, arguments);
                var nativeParamsTypes = (Type[])@params[0];
                var nativeParams = (object[])@params[1];
                var nativeParamsSizes = (uint[])@params[2];
                var returningParamsIndexes = (Dictionary<string, int>)@params[3];
                

                var loader = RakNet.Client.NativeLoader;
                var NativeRead = loader.Load("BS_ReadValue", nativeParamsSizes, nativeParamsTypes);

                NativeRead.Invoke(nativeParams);
                Console.WriteLine("ParamTypes:");
                Console.WriteLine($"Length: {nativeParamsTypes.Length}");
                foreach (var t in nativeParamsTypes)
                {
                    Console.WriteLine(t);
                }
                Console.WriteLine("Params:");
                Console.WriteLine($"Length: {nativeParams.Length}");
                foreach (var t in nativeParams)
                {
                    Console.WriteLine(t);
                }
                Console.WriteLine("Params sizes:");
                foreach (var t in nativeParamsSizes)
                {
                    Console.WriteLine(t);
                }
                var returningParams = new Dictionary<string, object>();
                foreach (KeyValuePair<string, int> keyValue in returningParamsIndexes)
                {
                    returningParams.Add(keyValue.Key, nativeParams[keyValue.Value]);
                }
                Console.WriteLine("Returning Params:");
                Console.WriteLine("Returning Params sizes:");
                foreach (var t in returningParams)
                {
                    Console.WriteLine(t);
                }

                return returningParams;
            }
            #endregion

            #endregion
            [NativeMethod(Function = "BS_WriteValue")]
            public virtual int Test_WriteValue(int bs, int playerIDType, int playerID, int nameLenType, int nameLen, int nameType, string name)
            {
                throw new NativeNotImplementedException();
            }

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
        #endregion
    }
}
