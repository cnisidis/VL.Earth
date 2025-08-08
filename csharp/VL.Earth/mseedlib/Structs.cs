using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.Earth.Libmseed;

namespace VL.Earth.Libmseed;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct MS3RecordList
{
    public ulong recordcnt;   //Count of records in the list(for convenience)
    public IntPtr first;   //Pointer to first entry, NULL if the none.
    public IntPtr last; //Pointer to last entry, NULL if the none. 
}

// The MS3Record struct is defined in a more detailed form than the pointer-only version
// provided earlier. It's an internal representation, but its size is required for P/Invoke
// functions that allocate/free it. It is recommended to work with the IntPtr to this struct.
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct MS3Record
{
    // Pointers
    public IntPtr record;      // const char *

    // Common header fields
    public Int32 reclen;        // int32_t
    public byte swapflag;      // uint8_t

    // Use 'ByValTStr' for the fixed-size character array (sid).
    // The size must match LM_SIDLEN from your C header.
     // Example, please verify this value from your C header.
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.LM_SIDLEN)]
    public string sid;         // char[LM_SIDLEN]

    public byte formatversion; // uint8_t
    public byte flags;         // uint8_t
    public Int64 starttime;     // nstime_t
    public double samprate;    // double
    public Int16 encoding;      // int16_t
    public byte pubversion;    // uint8_t
    public Int64 samplecnt;     // int64_t
    public UInt32 crc;         // uint32_t
    public UInt16 extralength;   // uint16_t
    public UInt32 datalength;    // uint32_t
    public IntPtr extra;       // char *

    // Data sample fields
    public IntPtr datasamples;  // void *
    public UInt64 datasize;      // uint64_t
    public Int64 numsamples;    // int64_t

    // C's 'char' is 8-bit, so we use 'byte' in C#.
    public byte sampletype;    // char
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MS3TraceSeg
    {
        public Int64 starttime;      // nstime_t
        public Int64 endtime;        // nstime_t
        public double samprate;      // double
        public Int64 samplecnt;      // int64_t
        public IntPtr datasamples;   // void *
        public UInt64 datasize;      // uint64_t
        public Int64 numsamples;     // int64_t

        // C#'s 'char' is 16-bit, but C's 'char' is 8-bit. We use 'byte' for this.
        public byte sampletype;      // char
        public IntPtr prvtptr;       // void *
        public IntPtr recordlist;    // struct MS3RecordList *
        public IntPtr prev;          // struct MS3TraceSeg *
        public IntPtr next;          // struct MS3TraceSeg *
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MS3TraceID
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.LM_SIDLEN)]
        public string sid;
        public byte pubversion;
        public Int64 earliest; // nstime_t
        public Int64 latest;   // nstime_t
        public IntPtr prvtptr;
        public uint numsegments;
        public IntPtr first;  // struct MS3TraceSeg *
        public IntPtr last;   // struct MS3TraceSeg *

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.MSTRACEID_SKIPLIST_HEIGHT)]
        public IntPtr[] next; // struct MS3TraceID *next[MSTRACEID_SKIPLIST_HEIGHT]

        public byte height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MS3TraceList
    {
        public uint numtraceids;
        // This is the head node of the skip list, not a pointer.
        public MS3TraceID traces;
        public UInt64 prngstate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MS3SelectTime
    {
        public Int64 starttime; // nstime_t
        public Int64 endtime;   // nstime_t
        public IntPtr next;     // Pointer to the next MS3SelectTime struct
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MS3Selections
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string sidpattern;
        public IntPtr timewindows; // Pointer to the MS3SelectTime list
        public IntPtr next;        // Pointer to the next MS3Selections struct
        public byte pubversion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MS3Tolerance
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.Ms3ToleranceFunc time;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.Ms3ToleranceFunc samprate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSEHEventDetection
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string detector;
        public double signalamplitude;
        public double signalperiod;
        public double backgroundestimate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string wave;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string units;
        public Int64 onsettime; // nstime_t
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] medsnr;
        public int medlookback;
        public int medpickalgorithm;
        public IntPtr next; // Pointer to next MSEHEventDetection
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSEHCalibration
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string type;
        public Int64 begintime; // nstime_t
        public Int64 endtime;   // nstime_t
        public int steps;
        public int firstpulsepositive;
        public int alternatesign;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string trigger;
        public int continued;
        public double amplitude;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string inputunits;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string amplituderange;
        public double duration;
        public double sineperiod;
        public double stepbetween;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string inputchannel;
        public double refamplitude;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string coupling;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string rolloff;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string noise;
        public IntPtr next; // Pointer to next MSEHCalibration
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSEHTimingException
    {
        public Int64 time; // nstime_t
        public float vcocorrection;
        public int usec; // DEPRECATED
        public int receptionquality;
        public uint count;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string clockstatus;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSEHRecenter
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string type;
        public Int64 begintime; // nstime_t
        public Int64 endtime;   // nstime_t
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string trigger;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLogEntry
    {
        public int level;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public string function;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string message;
        public IntPtr next;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLogRegistry
    {
        public int maxmessages;
        public int messagecnt;
        public IntPtr messages;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLogParam
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.LogPrintCallback log_print;
        [MarshalAs(UnmanagedType.LPStr)]
        public string logprefix;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.LogPrintCallback diag_print;
        [MarshalAs(UnmanagedType.LPStr)]
        public string errprefix;
        public MSLogRegistry registry;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LeapSecond
    {
        public Int64 leapsecond; // nstime_t
        public Int32 TAIdelta;
        public IntPtr next; // Pointer to next LeapSecond
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LIBMSEED_MEMORY
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.MallocFunc malloc;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.ReallocFunc realloc;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public LibmseedDelegates.FreeFunc free;
    }

