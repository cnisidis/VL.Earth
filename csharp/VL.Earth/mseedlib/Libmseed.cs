using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VL.Lib.Collections;



namespace VL.Earth.Libmseed;



/// <summary>
/// Contains delegates for C function pointers used in libmseed structs.
/// </summary>
public static class LibmseedDelegates
{
    public delegate double Ms3ToleranceFunc(IntPtr msr);
    public delegate void LogPrintCallback(string message);
    public delegate IntPtr MallocFunc(IntPtr size);
    public delegate IntPtr ReallocFunc(IntPtr ptr, IntPtr size);
    public delegate void FreeFunc(IntPtr ptr);
}

public static class Libmseed
{
    private const string DllName = "libs/libmseed.dll";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern ulong mstl3_unpack_recordlist(
        [In]IntPtr id,
        [In]IntPtr seg,
        [Out]IntPtr output,
        ulong outputsize,
        byte verbose);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 mstl3_readbuffer(
        ref IntPtr ppmstl, 
        [In] byte[] buffer, 
        int bufferlength,
        byte splitversion, 
        uint flags,
        IntPtr tolerance, 
        byte verbose); //MS3TraceList**

    

    //
    // extern int msr3_parse (const char *record, uint64_t recbuflen, MS3Record **ppmsr,
    //                        uint32_t flags, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_parse(
        [In] byte[] record,
        UInt64 recbuflen,
        ref IntPtr ppmsr,
        UInt32 flags,
        sbyte verbose);

    // extern int msr3_pack (const MS3Record *msr,
    //                       void (*record_handler) (char *, int, void *),
    //                       void *handlerdata, int64_t *packedsamples,
    //                       uint32_t flags, int8_t verbose);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void record_handler_delegate(
        IntPtr record,
        int record_length,
        IntPtr handlerdata);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_pack(
        IntPtr msr,
        record_handler_delegate record_handler,
        IntPtr handlerdata,
        ref Int64 packedsamples,
        UInt32 flags,
        sbyte verbose);

    // extern int msr3_repack_mseed3 (const MS3Record *msr, char *record, uint32_t recbuflen, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_repack_mseed3(
        IntPtr msr,
        [Out] byte[] record,
        UInt32 recbuflen,
        sbyte verbose);

    // extern int msr3_pack_header3 (const MS3Record *msr, char *record, uint32_t recbuflen, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_pack_header3(
        IntPtr msr,
        [Out] byte[] record,
        UInt32 recbuflen,
        sbyte verbose);

    // extern int msr3_pack_header2 (const MS3Record *msr, char *record, uint32_t recbuflen, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_pack_header2(
        IntPtr msr,
        [Out] byte[] record,
        UInt32 recbuflen,
        sbyte verbose);

    // extern int64_t msr3_unpack_data (MS3Record *msr, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 msr3_unpack_data(
        IntPtr msr,
        sbyte verbose);

    // extern int msr3_data_bounds (const MS3Record *msr, uint32_t *dataoffset, uint32_t *datasize);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_data_bounds(
        IntPtr msr,
        out UInt32 dataoffset,
        out UInt32 datasize);

    // extern int64_t ms_decode_data (const void *input, uint64_t inputsize, uint8_t encoding,
    //                                 uint64_t samplecount, void *output, uint64_t outputsize,
    //                                 char *sampletype, int8_t swapflag, const char *sid, int8_t verbose);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 ms_decode_data(
    [In] byte[] input,
    UInt64 inputsize,
    byte encoding,
    UInt64 samplecount,
    [Out] byte[] output,
    UInt64 outputsize,
    out char sampletype, // CORRECTED: Changed from [Out] ref to out
    sbyte swapflag,
    string sid,
    sbyte verbose);

    // extern MS3Record* msr3_init (MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr msr3_init(
        IntPtr msr);

    // extern void msr3_free (MS3Record **ppmsr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void msr3_free(
        ref IntPtr ppmsr);

    // extern MS3Record* msr3_duplicate (const MS3Record *msr, int8_t datadup);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr msr3_duplicate(
        IntPtr msr,
        sbyte datadup);

    // extern nstime_t msr3_endtime (const MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 msr3_endtime(
        IntPtr msr);

    // extern void msr3_print (const MS3Record *msr, int8_t details);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void msr3_print(
        IntPtr msr,
        sbyte details);

    // extern int msr3_resize_buffer (MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_resize_buffer(
        IntPtr msr);

    // extern double msr3_sampratehz (const MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double msr3_sampratehz(
        IntPtr msr);

    // extern nstime_t msr3_nsperiod (const MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 msr3_nsperiod(
        IntPtr msr);

    // extern double msr3_host_latency (const MS3Record *msr);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double msr3_host_latency(
        IntPtr msr);

    // extern int64_t ms3_detect (const char *record, uint64_t recbuflen, uint8_t *formatversion);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 ms3_detect(
        [In] byte[] record,
        UInt64 recbuflen,
        out byte formatversion);

    // extern int ms_parse_raw3 (const char *record, int maxreclen, int8_t details);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_parse_raw3(
        [In] byte[] record,
        int maxreclen,
        sbyte details);

    // extern int ms_parse_raw2 (const char *record, int maxreclen, int8_t details, int8_t swapflag);
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_parse_raw2(
        [In] byte[] record,
        int maxreclen,
        sbyte details,
        sbyte swapflag);


    // --- Record-level functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr ms3_initmsr(IntPtr msr);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void msr3_freemsr(ref IntPtr msr);


    //ms3_readmsr(MS3Record** ppmsr, const char* mspath, uint32_t flags, int8_t verbose);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_readmsr(
    ref IntPtr ppmsr,
    [MarshalAs(UnmanagedType.LPStr)] string mspath,
    uint flags,
    int verbose);

    /*
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_readmsr(
    ref MS3Record ppmsr,
    [MarshalAs(UnmanagedType.LPStr)] string mspath,
    uint flags,
    int verbose);
    */


    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int msr3_pack(IntPtr msr, IntPtr pbuffer, int buflen, uint flags, int verbose);



    // --- Trace-list functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr mstl3_init(IntPtr mstl);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void mstl3_free(ref IntPtr mstl, int freeprvtptr);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_readtracelist(out IntPtr ppmstl, string mspath, int reclen, uint flags, int verbose);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_readtracelist_timewin(
        out IntPtr ppmstl, string mspath, ref MS3Tolerance tolerance,
        Int64 starttime, Int64 endtime, sbyte splitversion, uint flags, sbyte verbose);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_readtracelist_selection(
        out IntPtr ppmstl, string mspath, ref MS3Tolerance tolerance,
        ref MS3Selections selections, sbyte splitversion, uint flags, sbyte verbose);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern Int64 mstl3_writemseed(
        IntPtr mst, string mspath, sbyte overwrite, int maxreclen, sbyte encoding,
        uint flags, sbyte verbose);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mstl3_unpack_recordlist(IntPtr ms3seg, int full, IntPtr buffer, uint buffer_size);

    // --- URL functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_url_useragent(string program, string version);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_url_userpassword(string userpassword);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms3_url_addheader(string header);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void ms3_url_freeheaders();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int libmseed_url_support();

    // --- String manipulation functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_sid2nslc(
        string sid, [MarshalAs(UnmanagedType.LPStr)] StringBuilder net,
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder sta,
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder loc,
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder chan);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_nslc2sid(
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder sid, int sidlen,
        ushort flags, string net, string sta, string loc, string chan);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_seedchan2xchan(
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder xchan,
        string seedchan);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_xchan2seedchan(
        [MarshalAs(UnmanagedType.LPStr)] StringBuilder seedchan,
        string xchan);

    // --- Extra headers functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mseh_add_calibration_r(
        IntPtr msr, string ptr, ref MSEHCalibration calibration,
        out IntPtr parsestate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mseh_add_timing_exception_r(
        IntPtr msr, string ptr, ref MSEHTimingException exception,
        out IntPtr parsestate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mseh_add_recenter_r(
        IntPtr msr, string ptr, ref MSEHRecenter recenter,
        out IntPtr parsestate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mseh_add_event_detection_r(
        IntPtr msr, string ptr, ref MSEHEventDetection eventdetection,
        out IntPtr parsestate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mseh_free_parsestate(ref IntPtr parsestate);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int mseh_replace(IntPtr msr, string jsonstring);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int mseh_print(IntPtr msr, int indent);

    // --- Logging functions ---
    // Note: Variadic arguments are not directly supported. The C# version must take a pre-formatted string.
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_rlog(string function, int level, string format);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int ms_rlog_l(
        ref MSLogParam logp, string function, int level, string format);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void ms_rloginit(
        LibmseedDelegates.LogPrintCallback log_print, string logprefix,
        LibmseedDelegates.LogPrintCallback diag_print, string errprefix,
        int maxmessages);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ms_rlog_emit(ref MSLogParam logp, int count, int context);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ms_rlog_free(ref MSLogParam logp);

    // --- Utility functions ---
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte ms_samplesize(char sampletype);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ms_encoding_sizetype(byte encoding, out byte samplesize, out char sampletype);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern string ms_encodingstr(byte encoding);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern string ms_errorstr(int errorcode);

    
    



   
}


public static class MSeedUtil
{
    
    public static IEnumerable<MS3Record> RecordsFromBytes(Spread<byte> Bytes)
    {
        var _bytes = Bytes.ToArray();
        uint count = (uint)_bytes.Count();
        long retcode = Constants.MS_NOERROR;
        IntPtr msr = IntPtr.Zero;
        uint flags = (uint)(Constants.MSF_SKIPNOTDATA | Constants.MSF_UNPACKDATA);
        while (retcode == Constants.MS_NOERROR)
        {
            retcode = Libmseed.msr3_parse(_bytes, (ulong)count, ref msr, flags, 0);
            MS3Record record = Marshal.PtrToStructure<MS3Record>(msr);
            
            yield return record;
        }

        yield break;
    }


    public static TraceList FromBytes(IEnumerable<byte> Bytes, out int count)
    {
        IntPtr trace = IntPtr.Zero;
        IntPtr tlc = IntPtr.Zero;
        byte splitVersion = 0;
        uint flags = (uint)(Constants.MSF_SKIPNOTDATA | Constants.MSF_UNPACKDATA); //Constants.MSF_RECORDLIST

        count = (int)Libmseed.mstl3_readbuffer(ref trace, Bytes.ToArray(), Bytes.Count(), splitVersion, flags, tlc, 0);
        MS3TraceList traceList = Marshal.PtrToStructure<MS3TraceList>(trace);
        TraceList tList = new TraceList(traceList);

        IntPtr currentTraceIdPtr = traceList.traces.next[0];
        
        while (currentTraceIdPtr != IntPtr.Zero )
        {
            MS3TraceID currentTraceId = Marshal.PtrToStructure<MS3TraceID>(currentTraceIdPtr);

            // Create a new Trace object to hold the ID and its segments.
            Trace newTrace = new Trace() { Id = currentTraceId };

            // --- THIS IS THE NEW CODE TO GET THE SEGMENTS ---
            IntPtr currentSegPtr = currentTraceId.first;
           
            while (currentSegPtr != IntPtr.Zero)
            {
                // Marshal the current MS3TraceSeg pointer.
                MS3TraceSeg currentSeg = Marshal.PtrToStructure<MS3TraceSeg>(currentSegPtr);

                // Add the segment to our new Trace object.
                newTrace.Segments.Add(new ManagedTraceSeg(currentSeg));

                // Move to the next segment in the linked list.
                currentSegPtr = currentSeg.next;
              
            }
            // --- END OF NEW CODE ---

            tList.Traces.Add(newTrace);

            // Move to the next actual MS3TraceID in the lowest level of the skip list.
            currentTraceIdPtr = currentTraceId.next[0];
        }
        return tList;
    }
    
    public static IEnumerable<MSRecord> ReadMSEEDFile(string filename)
    {

        IntPtr msr = IntPtr.Zero;
        
        
        uint flags = (uint)(Constants.MSF_SKIPNOTDATA | Constants.MSF_UNPACKDATA | Constants.MSSWAP_HEADER);
        int verbose = 0;
        int retcode = Constants.MS_NOERROR;

        if (!File.Exists(filename))
        {
            Console.WriteLine($"ERROR: The specified file '{filename}' was not found.");
            Console.WriteLine("Please ensure the file exists in the same directory as the executable.");
            yield break;
        }
        else
        {

            Console.WriteLine($"Reading miniSEED file: {filename}");
            Console.WriteLine("---------------------------------------------");

            // ----------------------------------------------------------------
            // C Equivalent:
            //   while ((retcode = ms3_readmsr (&msr, filename, flags, verbose)) == MS_NOERROR)
            // ----------------------------------------------------------------

            while (retcode == Constants.MS_NOERROR)
            {
                retcode = Libmseed.ms3_readmsr(ref msr, filename, flags, verbose);

                if (retcode == Constants.MS_NOERROR)
                {

                    //Libmseed.msr3_print(msr, verbose);
                    MS3Record record = Marshal.PtrToStructure<MS3Record>(msr);
                    
                    
                    yield return new MSRecord(record);
                    //Console.WriteLine("SID: {0:G} Compression: {1:G}",record.sid, record.compression);

                }

            }
            // ----------------------------------------------------------------
            // C Equivalent:
            //   if (retcode != MS_ENDOFFILE)
            //     ms_log (2, "Cannot read %s: %s\n", filename, ms_errorstr (retcode));
            // ----------------------------------------------------------------
            if (retcode != Constants.MS_ENDOFFILE)
            {

                string errorMessage = Libmseed.ms_errorstr(retcode);
                Console.WriteLine($"\nCannot read {filename}: {errorMessage}");
            }
            else
            {
                Console.WriteLine($"End of Readining miniSEED file: {filename}");
                Console.WriteLine("---------------------------------------------");
            }
            Libmseed.ms3_readmsr(ref msr, null, flags, verbose);
        }

        
    }
    public static object GetSamplesFromRecord(MS3Record record)
    {
        if (record.datasamples == IntPtr.Zero || record.samplecnt <= 0)
        {
            return null;
        }

        switch (record.encoding)
        {
            // MiniSEED 32-bit integer encodings (Steim 1, Steim 2, etc.)
            case Constants.DE_INT32:
            case Constants.DE_STEIM1:
            case Constants.DE_STEIM2:
                // Create a new managed array to hold the samples
                int[] intSamples = new int[record.samplecnt];

                // Copy the data from the unmanaged pointer to the managed array
                Marshal.Copy(record.datasamples, intSamples, 0, (int)record.samplecnt);
                return intSamples;

            // MiniSEED 32-bit float encoding
            case Constants.DE_FLOAT32:
                float[] floatSamples = new float[record.samplecnt];
                Marshal.Copy(record.datasamples, floatSamples, 0, (int)record.samplecnt);
                return floatSamples;

            // MiniSEED 64-bit float/double encoding
            case Constants.DE_FLOAT64:
                double[] doubleSamples = new double[record.samplecnt];
                Marshal.Copy(record.datasamples, doubleSamples, 0, (int)record.samplecnt);
                return doubleSamples;

            // Add other encoding types as needed, such as DE_INT16, DE_GEOSCOPE24

            default:
                // Handle unsupported encodings
                Console.WriteLine($"Warning: Unsupported encoding type: {record.encoding}");
                return null;
        }
    }






}
