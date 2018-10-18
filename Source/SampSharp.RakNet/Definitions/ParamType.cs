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
        UInt8,
        UInt16,
        UInt32,
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
