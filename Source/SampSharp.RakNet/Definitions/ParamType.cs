using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Definitions
{
    public enum ParamType
    {
        Int8,
        Int16,
        Int32,
        Uint8,
        Uint16,
        Uint32,
        Float,
        Bool,
        String,

        // compressed
        CInt8,
        CInt16,
        CInt32,
        CUint8,
        CUint16,
        CUint32,
        CFloat,
        CBool,
        CString,

        Bits
    }
}
