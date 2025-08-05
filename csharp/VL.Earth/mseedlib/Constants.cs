using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Earth.Libmseed;


public static class Constants
{
    // Return values
    public const int MS_ENDOFFILE = 1;
    public const int MS_NOERROR = 0;
    public const int MS_GENERROR = -1;
    public const int MS_NOTSEED = -2;
    public const int MS_WRONGLENGTH = -3;
    public const int MS_OUTOFRANGE = -4;
    public const int MS_UNKNOWNFORMAT = -5;
    public const int MS_STBADCOMPFLAG = -6;
    public const int MS_INVALIDCRC = -7;

    // Control flags
    public const int MSF_UNPACKDATA = 0x0001;
    public const int MSF_SKIPNOTDATA = 0x0002;
    public const int MSF_VALIDATECRC = 0x0004;
    public const int MSF_PNAMERANGE = 0x0008;
    public const int MSF_ATENDOFFILE = 0x0010;
    public const int MSF_SEQUENCE = 0x0020;
    public const int MSF_FLUSHDATA = 0x0040;
    public const int MSF_PACKVER2 = 0x0080;
    public const int MSF_RECORDLIST = 0x0100;
    public const int MSF_MAINTAINMSTL = 0x0200;

    // Data encoding types
    public const byte DE_TEXT = 0;
    public const byte DE_ASCII = DE_TEXT;
    public const byte DE_INT16 = 1;
    public const byte DE_INT32 = 3;
    public const byte DE_FLOAT32 = 4;
    public const byte DE_FLOAT64 = 5;
    public const byte DE_STEIM1 = 10;
    public const byte DE_STEIM2 = 11;
    public const byte DE_GEOSCOPE24 = 12;
    public const byte DE_GEOSCOPE163 = 13;
    public const byte DE_GEOSCOPE164 = 14;
    public const byte DE_CDSN = 16;
    public const byte DE_SRO = 30;
    public const byte DE_DWWSSN = 32;

    // Byte-swap flags
    public const int MSSWAP_HEADER = 0x01;
    public const int MSSWAP_PAYLOAD = 0x02;


    public const int MSTRACEID_SKIPLIST_HEIGHT = 8;
    public const int LM_SIDLEN = 64;

}



