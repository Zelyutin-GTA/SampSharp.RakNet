using System;
using SampSharp.RakNet.Definitions;
using System.Linq;
using System.Collections.Generic;

namespace SampSharp.RakNet
{
    public partial class BitStream
    {
        public partial class BitStreamInternal
        {
            private enum ParamTypeGroup
            {
                INT,
                Bool,
                Float,
                String,
                Bits
            }
            private object[] PrepareParams(int bs, bool returning, params object[] arguments)
            {
                const int nonArgumentsCount = 1; // int bs
                var nativeParamsTypes = new List<Type>();
                var nativeParams = new List<object>();
                var returningParamsIndexes = new Dictionary<string, int>();
                var nativeParamsSizes = new List<uint>();
                nativeParamsTypes.Add(typeof(int));
                nativeParams.Add(bs);

                int i = 0;
                while (i < arguments.Length)
                {
                    var argument = arguments[i];
                    if (!typeof(ParamType).IsAssignableFrom(argument.GetType()))
                    {
                        throw new RakNetException($"Param [index:{i}] is not ParamType");
                    }

                    //Adding ParamType to native parameters
                    nativeParamsTypes.Add(typeof(int).MakeByRefType()); // Should be reference to take in values
                    nativeParams.Add((int)argument);

                    var paramTypeGroup = GetParamTypeGroup((ParamType)argument);
                    
                    //Adding Param content to native parameters
                    var followingParamsCount = GetFollowingParamsCount(paramTypeGroup);
                    var types = GetFollowingParamsTypes(paramTypeGroup, returning);

                    if (i + followingParamsCount >= arguments.Length)
                    {
                        throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParamsCount}] with content");
                    }
                    for (int j = 1; j <= followingParamsCount; j++)
                    {
                        if (types[j - 1] == typeof(string) && !returning)
                            nativeParamsTypes.Add(types[j - 1]);
                        else
                            nativeParamsTypes.Add(types[j - 1].MakeByRefType());

                        if (returning)
                        {
                            if (types[j - 1] == typeof(int[])) // Was string
                            {
                                nativeParams.Add(new int[0]);
                            }
                            else if (paramTypeGroup == ParamTypeGroup.Bits && j == 2)
                            {
                                nativeParams.Add(arguments[i + j]);
                            }
                            else
                            {
                                nativeParams.Add(Activator.CreateInstance(types[j - 1]));
                            }
                        }
                        else
                        {
                            nativeParams.Add(arguments[i + j]);
                        }
                    }
                    if (paramTypeGroup == ParamTypeGroup.String)
                    {
                        int lengthSpecifierParamTypeIndex = i - 2;
                        int lengthSpecifierIndex = i - 1;
                        if (lengthSpecifierParamTypeIndex < 0 || !typeof(ParamType).IsAssignableFrom(arguments[lengthSpecifierParamTypeIndex].GetType()) || GetParamTypeGroup((ParamType)arguments[lengthSpecifierParamTypeIndex]) != ParamTypeGroup.INT)
                        {
                            throw new RakNetException($"String param [index:{i}] doesn't have prior length specifying");
                        }
                        
                        nativeParamsSizes.Add((uint)(nonArgumentsCount + lengthSpecifierIndex));
                    }
                    if (returning)
                    {
                        if (!typeof(string).IsAssignableFrom(arguments[i + 1].GetType()))
                        {
                            throw new RakNetException($"Param [index:{i + 1}] is not a string representing label name");
                        }
                        returningParamsIndexes.Add((string)arguments[i + 1], nonArgumentsCount + i + 1);
                    }
                    i += followingParamsCount + 1;
                }
                return new object[4] { nativeParamsTypes, nativeParams, nativeParamsSizes.ToArray(), returningParamsIndexes };
            }
            private ParamTypeGroup GetParamTypeGroup(ParamType param)
            {
                var intType = new ParamType[]
                {
                    ParamType.Int8,
                    ParamType.Int16,
                    ParamType.Int32,
                    ParamType.Uint8,
                    ParamType.Uint16,
                    ParamType.Uint32,
                    ParamType.CInt8,
                    ParamType.CInt16,
                    ParamType.CInt32,
                    ParamType.CUint8,
                    ParamType.CUint16,
                    ParamType.CUint32
                };
                var boolType = new ParamType[]
                {
                    ParamType.Bool,
                    ParamType.CBool
                };
                var floatType = new ParamType[]
                {
                    ParamType.Float,
                    ParamType.CFloat
                };
                var stringType = new ParamType[]
                {
                    ParamType.String,
                    ParamType.CString
                };
                var bitsType = new ParamType[]
                {
                    ParamType.Bits
                };

                if (intType.Contains(param)) return ParamTypeGroup.INT;
                else if (boolType.Contains(param)) return ParamTypeGroup.Bool;
                else if (floatType.Contains(param)) return ParamTypeGroup.Float;
                else if (stringType.Contains(param)) return ParamTypeGroup.String;
                else if (bitsType.Contains(param)) return ParamTypeGroup.Bits;

                throw new RakNetException($"Param has unknown ParamType");
            }
            private int GetFollowingParamsCount(ParamTypeGroup group)
            {
                switch (group)
                {
                    case ParamTypeGroup.INT: return 1;
                    case ParamTypeGroup.Bool: return 1;
                    case ParamTypeGroup.Float: return 1;
                    case ParamTypeGroup.String: return 1;
                    case ParamTypeGroup.Bits: return 2;
                }

                throw new RakNetException($"Param has unknown ParamGroupType");
            }
            private Type[] GetFollowingParamsTypes(ParamTypeGroup group, bool returning)
            {
                switch (group)
                {
                    case ParamTypeGroup.INT: return new Type[]{ typeof(int) };
                    case ParamTypeGroup.Bool: return new Type[] { typeof(bool) };
                    case ParamTypeGroup.Float: return new Type[] { typeof(float) };
                    case ParamTypeGroup.String: if (returning) return new Type[] { typeof(int[]) }; else return new Type[] { typeof(string) };
                    case ParamTypeGroup.Bits: return new Type[] { typeof(int), typeof(int) };
                }

                throw new RakNetException($"Param has unknown ParamGroupType");
            }

            private void ExecuteReturningNative(Type[] nativeParamsTypes, ref object[] nativeParams, uint[] nativeParamsSizes, Dictionary<string, int> returningParamsIndexes, ref Dictionary<string, object> returningParams, uint? extraParamSize)
            {
                var loader = RakNet.Client.NativeLoader;

                if (extraParamSize != null)
                {
                    var newNativeParamsSizes = new uint[1+ nativeParamsSizes.Length];
                    newNativeParamsSizes[0] = (uint)extraParamSize;
                    for(int i = 0; i < nativeParamsSizes.Length; i++)
                    {
                        newNativeParamsSizes[1+i] = nativeParamsSizes[i];
                    }
                    nativeParamsSizes = newNativeParamsSizes;
                }
                
                var NativeRead = loader.Load("BS_ReadValue", nativeParamsSizes, nativeParamsTypes);
                NativeRead.Invoke(nativeParams);
                foreach (KeyValuePair<string, int> keyValue in returningParamsIndexes)
                {
                    if (nativeParams[keyValue.Value] is int[]) // converting to string
                    {
                        var stringInt = (int[])nativeParams[keyValue.Value];
                        var stringChar = new char[stringInt.Length];
                        for (int i = 0; i < stringInt.Length; i++)
                        {
                            stringChar[i] = (char)stringInt[i];
                        }
                        var @string = new string(stringChar);
                        returningParams.Add(keyValue.Key, @string);
                    }
                    else
                    {
                        returningParams.Add(keyValue.Key, nativeParams[keyValue.Value]);
                    }
                }
            }
        }
    }
}
