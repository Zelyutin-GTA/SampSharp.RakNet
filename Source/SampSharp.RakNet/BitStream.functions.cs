using System;
using SampSharp.RakNet.Definitions;
using System.Linq;

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
            private object[] PrepareParams(int bs, params object[] arguments)
            {
                const int nonArgumentsCount = 1; // int bs
                var nativeParamsTypes = new Type[nonArgumentsCount + arguments.Length];
                var nativeParams = new object[nonArgumentsCount + arguments.Length];
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
                    switch (paramTypeGroup)
                    {
                        case ParamTypeGroup.INT:
                        {
                            const int followingParams = 1;
                            if (i + followingParams >= arguments.Length)
                            {
                                throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParams}] with content");
                            }

                            nativeParamsTypes[nonArgumentsCount + i + 1] = typeof(int).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 1] = arguments[i + 1];

                            i += followingParams + 1;
                            break;
                        }
                        case ParamTypeGroup.BOOL:
                        {
                            const int followingParams = 1;
                            if (i + followingParams >= arguments.Length)
                            {
                                throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParams}] with content");
                            }

                            nativeParamsTypes[nonArgumentsCount + i + 1] = typeof(bool).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 1] = arguments[i + 1];

                            i += followingParams + 1;
                            break;
                        }
                        case ParamTypeGroup.FLOAT:
                        {
                            const int followingParams = 1;
                            if (i + followingParams >= arguments.Length)
                            {
                                throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParams}] with content");
                            }

                            nativeParamsTypes[nonArgumentsCount + i + 1] = typeof(float).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 1] = arguments[i + 1];

                            i += followingParams + 1;
                            break;
                        }
                        case ParamTypeGroup.STRING:
                        {
                            const int followingParams = 1;
                            if (i + followingParams >= arguments.Length)
                            {
                                throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParams}] with content");
                            }

                            nativeParamsTypes[nonArgumentsCount + i + 1] = typeof(string).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 1] = arguments[i + 1];

                            i += followingParams + 1;
                            break;
                        }
                        case ParamTypeGroup.BITS:
                        {
                            const int followingParams = 2;
                            if (i + followingParams >= arguments.Length)
                            {
                                throw new RakNetException($"Param [index:{i}] does not have following arguments [amount:{followingParams}] with content");
                            }

                            nativeParamsTypes[nonArgumentsCount + i + 1] = typeof(int).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 1] = arguments[i + 1];
                            nativeParamsTypes[nonArgumentsCount + i + 2] = typeof(int).MakeByRefType();
                            nativeParams[nonArgumentsCount + i + 2] = arguments[i + 2];

                            i += followingParams + 1;
                            break;
                        }
                        default:
                        {
                            throw new RakNetException($"Param [index:{i}] has unknown ParamType");
                        }
                    }
                }
                return new object[2] { nativeParamsTypes, nativeParams };
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
        }
    }
}
