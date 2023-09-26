using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using static VL.Earth.MiniSeed;

namespace VL.Earth
{
    public class MiniSeed
    {
        public static int EXTRACTBITRANGE(int value, int startbit, int length)
        {
            return (int)((value >> startbit) & ((1U << length) - 1));
        }



        public static int EXTRACTBITRANGE(uint value, int startbit, int length)
        {
            return (int)((value >> startbit) & ((1U << length) - 1));
        }

        public static byte[] EXTRACTBITRANGEBytes(int value, int startbit, int length)
        {
            return BitConverter.GetBytes(((value >> startbit) & ((1U << length) - 1)));
        }


        public static void SwitchNibble(int frame, int nibble, bool debug, out List<int> diff)
        {
            byte[] dword = new byte[8];
            int diffcount = 0;
            int semask;
            diff = new List<int>();


            switch (nibble)
            {
                case 0: /* nibble=00: Special flag, no differences */
                    if (debug)
                        Console.WriteLine("  W{0:G}: 00=special\n");
                    break;
                case 1: /* nibble=01: Four 1-byte differences */
                    diffcount = 4;



                    dword = BitConverter.GetBytes(frame).Take(32).ToArray();
                    for (int idx = 0; idx < diffcount; idx++)
                    {

                        diff.Add((int)dword[idx]);
                    }

                    if (debug)
                        Console.WriteLine("  W{0:G}: 01=4x8b  {1:G}  {2:G}  {3:G}  {4:G}", diff[0], diff[1], diff[2], diff[3]);
                    break;

                case 2: /* nibble=10: Must consult dnib, the high order two bits */

                    int dnib = (int)EXTRACTBITRANGE(frame, 2, 2);
                    switch (dnib)
                    {
                        case 0: /* nibble=10, dnib=00: Error, undefined value */
                            //ms_log(2, "%s: Impossible Steim2 dnib=00 for nibble=10\n", srcname);

                            break;

                        case 1: /* nibble=10, dnib=01: One 30-bit difference */
                            diffcount = 1;
                            semask = (int)(1ul << (30 - 1)); /* Sign extension from bit 30 */
                            var temp = (int)EXTRACTBITRANGE(frame, 0, 30);
                            temp = (temp ^ semask) - semask;
                            diff.Add(temp);


                            if (debug)
                                Console.WriteLine("  W{0:G}: 10,01=1x30b  {1:G}\n", diff[0]);
                            break;

                        case 2: /* nibble=10, dnib=10: Two 15-bit differences */
                            diffcount = 2;
                            semask = (int)(1ul << (15 - 1)); /* Sign extension from bit 15 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                temp = (int)EXTRACTBITRANGE(frame, (15 - idx * 15), 15);
                                temp = (temp ^ semask) - semask;
                                diff.Add(temp); //15 - 
                                //diff[idx] = (diff[idx] ^ semask) - semask;
                            }

                            if (debug)
                                Console.WriteLine("  W%02d: 10,10=2x15b  %d  %d\n", diff[0], diff[1]);
                            break;

                        case 3: /* nibble=10, dnib=11: Three 10-bit differences */
                            diffcount = 3;
                            semask = (int)(1ul << (10 - 1)); /* Sign extension from bit 10 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                temp = (int)EXTRACTBITRANGE(frame, (20 - idx * 10), 10);

                                temp = (temp ^ semask) - semask;
                                diff.Add(temp); 
                                //diff[idx] = (diff[idx] ^ semask) - semask;
                            }

                            if (debug) Console.WriteLine("  W%02d: 10,11=3x10b  %d  %d  %d\n", diff[0], diff[1], diff[2]);

                            break;
                    }

                    break;
                case 3: /* nibble=11: Must consult dnib, the high order two bits */

                    dnib = (int)EXTRACTBITRANGE(frame, 30, 2);
                    switch (dnib)
                    {
                        case 0: /* nibble=11, dnib=00: Five 6-bit differences */
                            diffcount = 5;
                            semask = (int)(1ul << (6 - 1)); /* Sign extension from bit 6 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                var temp = (int)EXTRACTBITRANGE(frame, (24 - idx * 6), 6);
                                temp = (temp ^ semask) - semask;
                                diff.Add(temp); //24 -

                                //diff[idx] = (diff[idx] ^ semask) - semask;
                            }

                            if (debug)
                                Console.WriteLine("  W%02d: 11,00=5x6b  %d  %d  %d  %d  %d\n", diff[0], diff[1], diff[2], diff[3], diff[4]);
                            break;
                        case 1: /* nibble=11, dnib=01: Six 5-bit differences */
                            diffcount = 6;
                            semask = (int)(1ul << (5 - 1)); /* Sign extension from bit 5 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                var temp = (int)EXTRACTBITRANGE(frame, (25 - idx * 5), 5);
                                temp = (temp ^ semask) - semask;
                                diff.Add(temp);
                                //diff[idx] = (diff[idx] ^ semask) - semask;
                            }

                            if (debug)
                                Console.WriteLine("  W%02d: 11,01=6x5b  %d  %d  %d  %d  %d  %d\n",
                                         diff[0], diff[1], diff[2], diff[3], diff[4], diff[5]);
                            break;
                        case 2: /* nibble=11, dnib=10: Seven 4-bit differences */
                            diffcount = 7;
                            semask = (int)(1ul << (4 - 1)); /* Sign extension from bit 4 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                var temp = (int)EXTRACTBITRANGE(frame, (24 - idx * 4), 4);
                                temp = (temp ^ semask) - semask;
                                diff.Add(temp); //24 - 
                                //diff[idx] = (diff[idx] ^ semask) - semask;
                            }

                            if (debug)
                                Console.WriteLine("  W%02d: 11,10=7x4b  %d  %d  %d  %d  %d  %d  %d\n",
                                         diff[0], diff[1], diff[2], diff[3], diff[4], diff[5], diff[6]);
                            break;
                        case 3: /* nibble=11, dnib=11: Error, undefined value */
                            Console.WriteLine("%s: Impossible Steim2 dnib=11 for nibble=11\n"); //srcname


                            break;
                    }

                    break;

            }/* Done with decoding 32-bit word based on nibble */
        }
        public static void CalculateDifferences(int[] differences, int wordIndex, int frameIndex, int X0, ref int[] values, int outputptr, int numberOfSamples, int samplecount)
        {
            int diffcount = differences.Length;

            if (diffcount > 0)
            {
                for (int idx = 0; idx < diffcount && samplecount < numberOfSamples; idx++)
                {
                    if (wordIndex == 0 && frameIndex == 0) /* Ignore first difference, instead store X0 */
                        values[0] = X0;
                    else /* Otherwise store difference from previous sample */
                        values[outputptr - 1] += differences[idx];

                    samplecount--;
                }
            }


        }

        public static uint MsGswap4(uint data4)
        {


            uint dat = ((data4 & 0xff000000) >> 24) | ((data4 & 0x000000ff) << 24) |
                  ((data4 & 0x00ff0000) >> 8) | ((data4 & 0x0000ff00) << 8);

            return (dat & 0xff);
        }

        public static int MsGswap4(int data4)
        {


            int dat = (int)((data4 & 0xff000000) >> 24) | ((data4 & 0x000000ff) << 24) |
                  ((data4 & 0x00ff0000) >> 8) | ((data4 & 0x0000ff00) << 8);

            return (dat & 0xff);
        }
        internal class DWord
        {
            byte[] word { get; set; }
            byte[] d8 { set; get; }
            Int32 d32 { set; get; }
        }
        public static void msr_decode_steim2(int[] input, int samplecount, ref int[] output, int inputLength, bool swapflag, out int maxframes, bool debug)
        {

            int outputptr = 0;
            //int outputptr=0;
            /*
            unsafe
            {
                int* outputptr = &output[0];
            }
            */
            UInt32[] frame = new UInt32[16];

            int X0 = 0;
            int Xn = 0;
            int[] diff = new Int32[7];
            int[] diffs = new int[7];
            Int32 semask = 0;
            maxframes = inputLength / 16;
            uint startnibble = 3;
            int nibble = 0;
            int diffcount = 0;
            //int outputptr = 0;
            //DWord dword = new DWord();
            byte[] dword = new byte[4];
            //List<int> output = new List<int>(); 
            if (debug)
                Console.WriteLine("TotalFrames: {0:G}", maxframes);
            for (int frameidx = 0; frameidx < maxframes && samplecount > 0; frameidx++)
            {

                /* Copy frame, each is 16x32-bit quantities = 64 bytes */
                //memcpy(frame, input + (16 * frameidx), 64);
                //Buffer.BlockCopy(input,  frameidx * 16, frame, 0, 64);
                //frame must be 16 bytes not 64 bits
                Array.Copy(input, frameidx * 16, frame, 0, 16);
                //frame = input[frameidx*16]
                //Buffer.BlockCopy(input, frameidx * 16, frame, 0, 16);
                if (frameidx == 0)
                {

                    if (swapflag)
                    {
                        frame[1] = MsGswap4(frame[1]);
                        frame[2] = MsGswap4(frame[2]);
                    }

                    X0 = (Int32)frame[1];
                    Xn = (Int32)frame[2];

                    startnibble = 3; /* First frame: skip nibbles, X0, and Xn */
                    if (debug)
                        Console.WriteLine("Frame 0, X0 = {0:G} Xn = {1:G}", X0, Xn);
                }
                else
                {
                    startnibble = 1; /* Subsequent frames: skip nibbles */

                }
                /* Swap 32-bit word containing the nibbles */
                if (swapflag)
                    frame[0] = MsGswap4(frame[0]);
                if (debug)
                    Console.WriteLine("Frame: {0:G}", frameidx);
                /* Decode each 32-bit word according to nibble */
                for (int widx = (int)startnibble; widx < 16 && samplecount > 0; widx++)
                {
                    if (debug)
                        Console.WriteLine("  Word:{0:G}", widx);
                    /* W0: the first 32-bit quantity contains 16 x 2-bit nibbles */
                    nibble = EXTRACTBITRANGE(frame[0], (30 - (2 * widx)), 2);
                    diffcount = 0;
                    if (debug)
                        Console.WriteLine("\tnibble={0:G}", nibble);
                    switch (nibble)
                    {
                        case 0: /* nibble=00: Special flag, no differences */
                            //if (libmseed_decodedebug > 0)
                            if(debug)
                                Console.WriteLine("\t  00=special", widx, frameidx);

                            break;
                        case 1: /* nibble=01: Four 1-byte differences */
                            diffcount = 4;

                            
                            dword = BitConverter.GetBytes(frame[widx]);
                            dword = dword.Reverse().ToArray();
                            int counter = diffcount - 1;

                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                //Buffer.BlockCopy(dword, 0, diff, widx, 4);
                                diff[idx] = (SByte)dword[idx];
                                counter--;

                            }
                            if (debug)
                                Console.WriteLine("word: [{0}]", string.Join(", ", dword));
                            break;

                        case 2: /* nibble=10: Must consult dnib, the high order two bits */
                            if (swapflag)
                                frame[widx] = MsGswap4(frame[widx]);
                            int dnib = EXTRACTBITRANGE(frame[widx], 30, 2);
                            if (debug)
                                Console.WriteLine("\tdnib={0:G}", dnib);
                            switch (dnib)
                            {
                                case 0: /* nibble=10, dnib=00: Error, undefined value */
                                    if(debug)
                                    {
                                        Console.WriteLine(" -- ERROR -- Impossible Steim2 dnib=00 for nibble=10\n");
                                    }
                                        
                                    break;

                                case 1: /* nibble=10, dnib=01: One 30-bit difference */
                                    diffcount = 1;
                                    semask = (int)1ul << (30 - 1); /* Sign extension from bit 30 */
                                    diff[0] = EXTRACTBITRANGE(frame[widx], 0, 30);
                                    diff[0] = (diff[0] ^ semask) - semask;
                                    break;

                                case 2: /* nibble=10, dnib=10: Two 15-bit differences */
                                    diffcount = 2;
                                    semask = (int)1ul << (15 - 1); /* Sign extension from bit 15 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = EXTRACTBITRANGE(frame[widx], (15 - idx * 15), 15);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }
                                    if (debug)
                                        Console.WriteLine("  W{0:G}: 10,10=2x15b  {1:G}  {2:G}", widx, diff[0], diff[1]);

                                    break;

                                case 3: /* nibble=10, dnib=11: Three 10-bit differences */
                                    diffcount = 3;
                                    semask = (int)1ul << (10 - 1); /* Sign extension from bit 10 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = EXTRACTBITRANGE(frame[widx], (20 - idx * 10), 10);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }
                                    if (debug)
                                        Console.WriteLine("  W{0:G}: 10,10=3x10b  {1:G}  {2:G}  {3:G}", widx, diff[0], diff[1], diff[2]);
                                    break;

                            }

                            break;

                        case 3: /* nibble=11: Must consult dnib, the high order two bits */
                            if (swapflag)
                                frame[widx] = MsGswap4(frame[widx]);
                            dnib = EXTRACTBITRANGE(MsGswap4(frame[widx]), 30, 2);
                            if (debug)
                                Console.WriteLine("\tdnib={0:G}", dnib);
                            switch (dnib)
                            {
                                case 0: /* nibble=11, dnib=00: Five 6-bit differences */
                                    diffcount = 5;
                                    semask = (Int32)1ul << (6 - 1); /* Sign extension from bit 6 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (24 - idx * 6), 6);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }
                                    if (debug)
                                        Console.WriteLine("  W{0:G}: 10,10=5x6b  {1:G}  {2:G}  {3:G}  {4:G}  {5:G}", 
                                        widx, diff[0], diff[1], diff[2], diff[3], diff[4]);
                                    break;

                                case 1: /* nibble=11, dnib=01: Six 5-bit differences */
                                    diffcount = 6;
                                    semask = (Int32)1ul << (5 - 1); /* Sign extension from bit 5 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (25 - idx * 5), 5);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }
                                    if (debug)
                                        Console.WriteLine("  W{0:G}: 10,10=6x5b  {1:G}  {2:G}  {3:G}  {4:G}  {5:G}  {6:G}",
                                        widx, diff[0], diff[1], diff[2], diff[3], diff[4], diff[5]);

                                    break;

                                case 2: /* nibble=11, dnib=10: Seven 4-bit differences */
                                    diffcount = 7;
                                    semask = (Int32)1ul << (4 - 1); /* Sign extension from bit 4 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (24 - idx * 4), 4);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }
                                    if (debug)
                                        Console.WriteLine("  W{0:G}: 10,10=7x4b  {1:G}  {2:G}  {3:G}  {4:G}  {5:G}  {6:G}  {7:G}",
                                        widx, diff[0], diff[1], diff[2], diff[3], diff[4], diff[5], diff[6]);

                                    break;

                                case 3: /* nibble=11, dnib=11: Error, undefined value */
                                    //ms_log(2, "%s: Impossible Steim2 dnib=11 for nibble=11\n", srcname);
                                    if (debug)
                                        Console.WriteLine("F:{0:G} - W:{1:G} - DC:{2:G} Impossible Steim2 dnib = 11 for nibble = 11", frameidx, widx, diffcount);
                                    break;
                            }
                            //diffcounts.Add(diffcount);
                            break;
                    } /* Done with decoding 32-bit word based on nibble */

                    /* Apply differences to calculate output samples */
                    if (diffcount > 0)
                    {

                        for (int idx = 0; idx < diffcount && samplecount > 0; idx++)
                        {
                            if (outputptr == 0) /* Ignore first difference, instead store X0 */
                            {
                                //diff[0] = X0;
                                output[outputptr] = (X0);
                                //output.Add(X0);
                            }
                            /* Otherwise store difference from previous sample */
                            else

                            {
                                output[outputptr] = (output[outputptr - 1] + diff[idx]);
                                //output.Add(output[outputptr-1] + diff[idx]);
                            }
                            //diffs.Add(diffcount);
                            if (debug)
                                Console.WriteLine("s:{0:G} d:{1:G}", outputptr, diff[idx]);
                            samplecount--;
                            outputptr++;

                        }

                    }




                } /* Done looping over nibbles and 32-bit word */

                //outputptr = output.Count-1;

            }/* End of msr_decode_steim2() */

        }

    }

    public enum BlocketteType
    {
        SAMPLE_RATE = 100,
        
        DATA_ONLY_SEED = 1000,
        DATA_EXTENSION = 1001,
        GENERIC_EVENT_DETECTION = 200,
        MURDOCK_EVENT_DETECTION = 201,
        VARIABLE_LENGTH_OPAQUE_DATA = 2000,

        STEP_CALLIBRATION = 300,
        SINE_CALLIBRATION = 310,
        PSEUDO_RANDOM_CALLIBRATION = 320,
        GENERIC_CALLIBRATION = 390,
        CALLIBRATION_ABORT = 395,

        BEAM = 400,
        BEAM_DELAY = 405,

        TIMING = 500,


        FIELD_VOLUME_IDENTIFIER = 005,
        TELEMETRY_VOLUME_IDENTIFIER = 008,
        VOLUME_IDENTIFIER = 010,
        VOLUME_STATION_HEADER_INDEX = 011,
        VOLUME_TIME_SPAN_INDEX = 012,
        DATA_FORMAT_DICTIONARY = 030,
        COMMENT_DESCRIPTION = 031,
        CITED_SOURCE_DICTIONARY = 032,
        GENERIC_ABBREVIATION = 033,
        UNITS_ABBREVIATION = 034,
        BEAM_CONFIGURATION = 035,
        FIR_DICTIONARY = 041,
        RESPONSE_POLYNOMIAL_DICT = 042,
        RESPONSE_POLES_AND_ZEROS_DICT = 043,
        RESPONSE_COEFFICIENTS_DICT = 044,
        RESPONSE_LIST_DICT = 045,
        GENERIC_RESPONSE_DICT = 046,
        DECIMATION_DICTIONARY = 047,
        CHANNEL_SENSITIVITY_GAIN_DICT = 048,
        RESPONSE_POLYNOMIAL_DICT_v23 = 049,
        
        STATION_IDENTIFIER = 050,
        STATION_COMMENT = 051,
        CHANNEL_IDENTIFIER = 052,
        STA_RESPONSE_POLES_AND_ZEROS



    }

    public enum QualityIndicator
    {
        D, //The state of quality control of the data is indeterminate.
        R, //Raw Waveform Data with no Quality Control
        Q, //Quality Controlled Data, some processes have been applied to the data.
        M  //Data center modified, time-series values have not been changed.
    }

    public enum ActivityFlags {
        CalibrationSignalPresent,
        TimeCorrectionApplied,
        BeginningOfEvent,
        EndOfEvent,
        PositiveLeap,
        NegativeLeap,
        EventInProgress
    
    }

    public enum IOAndClockFlags
    {
        PossibleParrityError,
        LongRecordRead,
        ShortRecordRead,
        StartOfTimeSeries,
        EndOfTimeSeries,
        ClockLocked
    }

    public enum DataQualityFlags 
    {
        AmplifierSaturation,
        DigitizerClipping,
        SpikesDetected,
        Glitches,
        MissingData,
        TelemetrySyncError,
        DigitalFilterCharging,
        QuestionableTimetag
    }

    public enum SampleRateFactor
    {
        SamplesPerSecond =0,
        SecondsPerSample =1
    }

    public enum EncodingFormat
    {
        ASCII =0,
        Int16 =1,
        Int24 =2,
        Int32 =3,
        Float =4,
        Double =5,

        STEIM1 = 10,
        STEIM2 = 11,
        GEOSCOPE24bit =12,
        GEOSCOPE16bit3bitExponent = 13,
        GEOSCOPE16bit4bitExponent = 14,
        USNationalNetworkCompression = 15,
        CDSN = 16,
        Graefenberg = 17,
        IPG = 18,
        STEIM3 = 19,

        SRO = 30,
        HGLP =31,
        DWWSSN =32,
        RSTN = 33
    }

    public enum ByteOrder
    {
        LittleEndian = 0,
        BigEndian =1
    }
}

