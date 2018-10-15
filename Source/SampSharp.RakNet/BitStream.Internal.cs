using System;
using System.Collections.Generic;

using SampSharp.Core.Natives.NativeObjects;
using SampSharp.RakNet.Definitions;
using System.Linq;

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
                var NativeRead = loader.Load("BS_WriteValue", nativeParamsSizes, nativeParamsTypes);

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
                Console.WriteLine("WriteValue result: " + result);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            public virtual Dictionary<string, object> BS_ReadValue(int bs, params object[] arguments)
            {

                var @params = PrepareParams(bs, true, arguments);
                var nativeParamsTypes = (List<Type>)@params[0];
                var nativeParams = (List<object>)@params[1];
                var nativeParamsSizes = ((uint[])@params[2]).ToList();
                var returningParamsIndexes = (Dictionary<string, int>)@params[3];

                var returningParams = new Dictionary<string, object>();

                var count = nativeParamsSizes.Count; // Needed because real .Count will not be changed in the future

                uint? processedParamSize = null;
                bool processedParamSizeExists = false;

                while (nativeParamsSizes.Count > 0)
                {
                    if (processedParamSizeExists)
                    {
                        processedParamSize = nativeParamsSizes[0];
                        nativeParamsSizes.RemoveAt(0);
                        if (nativeParamsSizes.Count == 0)
                        {
                            break;
                        }
                    }

                    var necessaryParamsCount = 1; // bs
                    var curStringLengthIndex = (int)nativeParamsSizes[0];
                    nativeParamsSizes.RemoveAt(0);

                    var partialSize = curStringLengthIndex - (necessaryParamsCount - 1);

                    var partial_nativeParamsTypes = new List<Type>();
                    var partial_nativeParams = new List<object>();
                    var partial_nativeParamsSizes = new List<uint>();
                    var partial_returningParamsIndexes = new Dictionary<string, int>();

                    var stringTypeAndContentParamsCount = 2; //ParamType.STRING + string

                    for (int j = 0; j < necessaryParamsCount; j++) // bs
                    {
                        partial_nativeParamsTypes.Add(nativeParamsTypes[j]);
                        partial_nativeParams.Add(nativeParams[j]);
                    }

                    for (int j = 0; j < partialSize; j++)
                    {

                        partial_nativeParamsTypes.Add(nativeParamsTypes[necessaryParamsCount]);
                        nativeParamsTypes.RemoveAt(necessaryParamsCount);

                        partial_nativeParams.Add(nativeParams[necessaryParamsCount]);
                        nativeParams.RemoveAt(necessaryParamsCount);
                    }

                    string[] keys = new string[returningParamsIndexes.Count];
                    returningParamsIndexes.Keys.CopyTo(keys, 0);

                    for (int j = 0; j < keys.Length; j++)
                    {
                        if (returningParamsIndexes[keys[j]] <= curStringLengthIndex)
                        {
                            partial_returningParamsIndexes.Add(keys[j], returningParamsIndexes[keys[j]]);
                            returningParamsIndexes.Remove(keys[j]);
                        }
                        else
                        {
                            returningParamsIndexes[keys[j]] -= partialSize;
                            if (returningParamsIndexes[keys[j]] >= necessaryParamsCount + stringTypeAndContentParamsCount)
                            {
                                returningParamsIndexes[keys[j]] += 1; // Cause of future insertion of stringLen
                            }
                        }
                    }

                    for (int j = 0; j < nativeParamsSizes.Count; j++)
                    {
                        nativeParamsSizes[j] -= (uint)partialSize;
                        nativeParamsSizes[j] += 1; // Cause of future insertion of stringLen
                    }

                    var partial_nativeParamsArray = partial_nativeParams.ToArray(); // Needed to get returned value
                    ExecuteReturningNative(partial_nativeParamsTypes.ToArray(), ref partial_nativeParamsArray, partial_nativeParamsSizes.ToArray(), partial_returningParamsIndexes, ref returningParams, processedParamSize);

                    int stringLen = (int)partial_nativeParamsArray[curStringLengthIndex];

                    
                    nativeParamsTypes.Insert(necessaryParamsCount + stringTypeAndContentParamsCount, typeof(int).MakeByRefType());
                    nativeParams.Insert(necessaryParamsCount + stringTypeAndContentParamsCount, stringLen);
                    nativeParamsSizes.Insert(0, (uint)(necessaryParamsCount + stringTypeAndContentParamsCount)); // Add size param index that is right after string

                    processedParamSizeExists = true;
                }

                var nativeParamsArray = nativeParams.ToArray();
                
                ExecuteReturningNative(nativeParamsTypes.ToArray(), ref nativeParamsArray, nativeParamsSizes.ToArray(), returningParamsIndexes, ref returningParams, processedParamSize);
                return returningParams;
            }
            #endregion
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
        #endregion
    }
}
