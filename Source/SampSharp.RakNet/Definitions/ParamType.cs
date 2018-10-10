using System;
using System.Collections.Generic;
using System.Text;

namespace SampSharp.RakNet.Definitions
{
    public enum ParamType
    {
        INT8,
        INT16,
        INT32,
        UINT8,
        UINT16,
        UINT32,
        FLOAT,
        BOOL,
        STRING,

        // compressed
        CINT8,
        CINT16,
        CINT32,
        CUINT8,
        CUINT16,
        CUINT32,
        CFLOAT,
        CBOOL,
        CSTRING,

        BITS
    }
}
