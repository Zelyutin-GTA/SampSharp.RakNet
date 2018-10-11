using System;
using SampSharp.RakNet.Definitions;

namespace SampSharp.RakNet
{
    public partial class BitStream
    {
        public partial class BitStreamInternal
        {
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
                    nativeParamsTypes[nonArgumentsCount + i] = typeof(int).MakeByRefType(); // Should be reference to take values
                    nativeParams[nonArgumentsCount + i] = (int)argument;

                    //Adding Param content to native parameters
                    switch (argument)
                    {
                        case ParamType.INT8:
                        case ParamType.INT16:
                        case ParamType.INT32:
                        case ParamType.UINT8:
                        case ParamType.UINT16:
                        case ParamType.UINT32:
                        case ParamType.CINT8:
                        case ParamType.CINT16:
                        case ParamType.CINT32:
                        case ParamType.CUINT8:
                        case ParamType.CUINT16:
                        case ParamType.CUINT32:
                        {
                            //int;
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
                        case ParamType.BOOL:
                        case ParamType.CBOOL:
                        {
                            //bool;
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
                        case ParamType.FLOAT:
                        case ParamType.CFLOAT:
                        {
                            //float;
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
                        case ParamType.STRING:
                        case ParamType.CSTRING:
                        {
                            //string;
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
                        case ParamType.BITS:
                        {
                            //bits;
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
        }
    }
}
