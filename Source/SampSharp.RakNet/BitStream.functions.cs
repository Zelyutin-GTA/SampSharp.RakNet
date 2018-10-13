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
                BOOL,
                FLOAT,
                STRING,
                BITS
            }
            private object[] PrepareParams(int bs, bool returning, params object[] arguments)
            {
                const int nonArgumentsCount = 1; // int bs
                var nativeParamsTypes = new Type[nonArgumentsCount + arguments.Length];
                var nativeParams = new object[nonArgumentsCount + arguments.Length];
                var returningParamsIndexes = new Dictionary<string, int>();
                var nativeParamsSizes = new List<int>();
                nativeParamsTypes[0] = typeof(int);
                nativeParams[0] = bs;

                int i = 0;
                while (i < arguments.Length)
                {
                    var argument = arguments[i];
                    if (!typeof(ParamType).IsAssignableFrom(argument.GetType()))
                    {
                        throw new RakNetException($"Param [index:{i}] is not ParamType");
                    }

                    //Adding ParamType to native parameters
                    nativeParamsTypes[nonArgumentsCount + i] = typeof(int).MakeByRefType(); // Should be reference to take in values
                    nativeParams[nonArgumentsCount + i] = (int)argument;

                    var paramTypeGroup = GetParamTypeGroup((ParamType)argument);
                    
                    //Adding Param content to native parameters
                    var followingParamsCount = GetFollowingParamsCount(paramTypeGroup);
                    var types = GetFollowingParamsTypes(paramTypeGroup);

                    if (i + followingParamsCount >= arguments.Length)
                    {
                        throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParamsCount}] with content");
                    }
                    for (int j = 1; j <= followingParamsCount; j++)
                    {
                        if (types[j - 1] == typeof(string))
                            nativeParamsTypes[nonArgumentsCount + i + j] = types[j - 1];
                        else
                            nativeParamsTypes[nonArgumentsCount + i + j] = types[j - 1].MakeByRefType();
                        if (returning)
                        {
                            if (types[j - 1] == typeof(string))
                            {
                                nativeParams[nonArgumentsCount + i + j] = new String("");
                            }
                            //For testing sizes
                            else if (types[j - 1] == typeof(int))
                            {
                                nativeParams[nonArgumentsCount + i + j] = 32;
                            }
                            else
                            {
                                nativeParams[nonArgumentsCount + i + j] = Activator.CreateInstance(types[j - 1]);
                            }
                        }
                        else
                        {
                            nativeParams[nonArgumentsCount + i + j] = arguments[i + j];
                        }
                    }
                    if (paramTypeGroup == ParamTypeGroup.STRING)
                    {
                        int lengthSpecifierParamTypeIndex = i - 2;
                        int lengthSpecifierIndex = i - 1;
                        if (lengthSpecifierParamTypeIndex < 0 || !typeof(ParamType).IsAssignableFrom(arguments[lengthSpecifierParamTypeIndex].GetType()) || GetParamTypeGroup((ParamType)arguments[lengthSpecifierParamTypeIndex]) != ParamTypeGroup.INT)
                        {
                            throw new RakNetException($"String param [index:{i}] doesn't have prior length specifying");
                        }
                        nativeParamsSizes.Add((int)nonArgumentsCount + lengthSpecifierIndex);
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
                    ParamType.INT8,
                    ParamType.INT16,
                    ParamType.INT32,
                    ParamType.UINT8,
                    ParamType.UINT16,
                    ParamType.UINT32,
                    ParamType.CINT8,
                    ParamType.CINT16,
                    ParamType.CINT32,
                    ParamType.CUINT8,
                    ParamType.CUINT16,
                    ParamType.CUINT32
                };
                var boolType = new ParamType[]
                {
                    ParamType.BOOL,
                    ParamType.CBOOL
                };
                var floatType = new ParamType[]
                {
                    ParamType.FLOAT,
                    ParamType.CFLOAT
                };
                var stringType = new ParamType[]
                {
                    ParamType.STRING,
                    ParamType.CSTRING
                };
                var bitsType = new ParamType[]
                {
                    ParamType.BITS
                };

                if (intType.Contains(param)) return ParamTypeGroup.INT;
                else if (boolType.Contains(param)) return ParamTypeGroup.BOOL;
                else if (floatType.Contains(param)) return ParamTypeGroup.FLOAT;
                else if (stringType.Contains(param)) return ParamTypeGroup.STRING;
                else if (bitsType.Contains(param)) return ParamTypeGroup.BITS;

                throw new RakNetException($"Param has unknown ParamType");
            }
            private int GetFollowingParamsCount(ParamTypeGroup group)
            {
                switch (group)
                {
                    case ParamTypeGroup.INT: return 1;
                    case ParamTypeGroup.BOOL: return 1;
                    case ParamTypeGroup.FLOAT: return 1;
                    case ParamTypeGroup.STRING: return 1;
                    case ParamTypeGroup.BITS: return 2;
                }

                throw new RakNetException($"Param has unknown ParamGroupType");
            }
            private Type[] GetFollowingParamsTypes(ParamTypeGroup group)
            {
                switch (group)
                {
                    case ParamTypeGroup.INT: return new Type[]{ typeof(int) };
                    case ParamTypeGroup.BOOL: return new Type[] { typeof(bool) };
                    case ParamTypeGroup.FLOAT: return new Type[] { typeof(float) };
                    case ParamTypeGroup.STRING: return new Type[] { typeof(string) };
                    case ParamTypeGroup.BITS: return new Type[] { typeof(int), typeof(int) };
                }

                throw new RakNetException($"Param has unknown ParamGroupType");
            }
        }
    }
}
