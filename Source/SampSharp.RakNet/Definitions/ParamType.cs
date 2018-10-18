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
        CompressedInt8,
        CompressedInt16,
        CompressedInt32,
        CompressedUInt8,
        CompressedUInt16,
        CompressedUInt32,
        CompressedFloat,
        CompressedBool,
        CompressedString,

        Bits
    }
}
