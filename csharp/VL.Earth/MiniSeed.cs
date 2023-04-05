using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace VL.Earth
{
    public class MiniSeed
    {
        public static int EXTRACTBITRANGE(int value, int startbit, int length)
        {
            return (int)((value >> startbit) & ((1U << length) - 1));
        }

        public static uint EXTRACTBITRANGE(uint value, int startbit, int length)
        {
            return ((value >> startbit) & ((1U << length) - 1));
        }

        public static int EXTRACTBITRANGE(byte value, int startbit, int length)
        {
            return (int)((value >> startbit) & ((1U << length) - 1));
        }

        
        public static void SwitchNibble(int frame, int nibble, bool debug, out List<int> diff)
        {
            byte[] dword = new byte[8];
            int diffcount = 0;

            diff = new List<int>();

            switch (nibble)
            {
                case 0: /* nibble=00: Special flag, no differences */
                    if (debug)
                        Console.WriteLine("  W{0:G}: 00=special\n");
                    break;
                case 1: /* nibble=01: Four 1-byte differences */
                    diffcount = 4;



                    dword = BitConverter.GetBytes(frame).ToArray();
                    for (int idx = 0; idx < diffcount; idx++)
                    {
                        diff.Add(dword[idx]);
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
                            int semask = (int)(1ul << (30 - 1)); /* Sign extension from bit 30 */
                            diff.Add((int)EXTRACTBITRANGE(frame, 0, 30));
                            //diff[0] = (diff[0] ^ semask) - semask;

                            if (debug)
                                Console.WriteLine("  W{0:G}: 10,01=1x30b  {1:G}\n", diff[0]);
                            break;

                        case 2: /* nibble=10, dnib=10: Two 15-bit differences */
                            diffcount = 2;
                            semask = (int)(1ul << (15 - 1)); /* Sign extension from bit 15 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                diff.Add((int)EXTRACTBITRANGE(frame, (idx * 15), 15)); //15 - 
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
                                diff.Add((int)EXTRACTBITRANGE(frame, (idx * 10), 10)); //20 -
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
                            int semask = (int)(1ul << (6 - 1)); /* Sign extension from bit 6 */
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                diff.Add((int)EXTRACTBITRANGE(frame, (idx * 6), 6)); //24 -
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
                                diff.Add((int)EXTRACTBITRANGE(frame, (25 - idx * 5), 5));
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
                                diff.Add((int)EXTRACTBITRANGE(frame, (idx * 4), 4)); //24 - 
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
            
            if(diffcount > 0)
            {
                for (int idx = 0; idx < diffcount && samplecount<numberOfSamples; idx++)
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

        public static void msr_decode_steim2(int[] input, int samplecount, ref List<int> output, int inputLength, bool swapflag, ref int maxframes)
        {
            UInt32[] frame = new UInt32[16];
            int X0 = 0;
            int Xn = 0;
            Int32[] diff = new Int32[7];
            Int32 semask = 0;
            maxframes = input.Length / 64 ;
            uint startnibble = 3;
            uint nibble = 0;
            int diffcount = 0;
            int outputptr = 0;
            //List<int> output = new List<int>(); 

            for (int frameidx = 0; frameidx < maxframes && samplecount > 0; frameidx++)
            {
                /* Copy frame, each is 16x32-bit quantities = 64 bytes */
                //memcpy(frame, input + (16 * frameidx), 64);
                Buffer.BlockCopy(input, 16 * frameidx, frame, 0, 64);



                if (frameidx == 0)
                {

                    if (swapflag)
                    {
                        frame[1] = MsGswap4(frame[1]);
                        frame[2] = MsGswap4(frame[2]);
                    }

                    X0 = (int)frame[1];
                    Xn = (int)frame[2];

                    startnibble = 3; /* First frame: skip nibbles, X0, and Xn */


                }
                {
                    startnibble = 1; /* Subsequent frames: skip nibbles */


                }
                for (int widx = (int)startnibble; widx < 16 && samplecount > 0; widx++)
                {
                    nibble = EXTRACTBITRANGE(frame[0], (30 - (2 * widx)), 2);
                    diffcount = 0;
                    switch (nibble)
                    {
                        case 0: /* nibble=00: Special flag, no differences */
                            //if (libmseed_decodedebug > 0)
                            //    ms_log(0, "  W%02d: 00=special\n", widx);

                            break;
                        case 1: /* nibble=01: Four 1-byte differences */
                            diffcount = 4;

                            byte[] word = BitConverter.GetBytes(frame[widx]);
                            for (int idx = 0; idx < diffcount; idx++)
                            {
                                diff[idx] = word[idx];
                            }
                            break;

                        case 2: /* nibble=10: Must consult dnib, the high order two bits */
                            if (swapflag)
                                frame[widx] = MsGswap4(frame[widx]);
                            uint dnib = EXTRACTBITRANGE(frame[widx], 30, 2);

                            switch (dnib)
                            {
                                case 0: /* nibble=10, dnib=00: Error, undefined value */
                                    //ms_log(2, "%s: Impossible Steim2 dnib=00 for nibble=10\n", srcname);


                                    break;

                                case 1: /* nibble=10, dnib=01: One 30-bit difference */
                                    diffcount = 1;
                                    semask = (Int32)1ul << (30 - 1); /* Sign extension from bit 30 */
                                    diff[0] = (int)EXTRACTBITRANGE(frame[widx], 0, 30);
                                    diff[0] = (diff[0] ^ semask) - semask;
                                    break;

                                case 2: /* nibble=10, dnib=10: Two 15-bit differences */
                                    diffcount = 2;
                                    semask = (Int32)1ul << (15 - 1); /* Sign extension from bit 15 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (15 - idx * 15), 15);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }


                                    break;

                                case 3: /* nibble=10, dnib=11: Three 10-bit differences */
                                    diffcount = 3;
                                    semask = (Int32)1ul << (10 - 1); /* Sign extension from bit 10 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (20 - idx * 10), 10);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }

                                    break;
                            }

                            break;

                        case 3: /* nibble=11: Must consult dnib, the high order two bits */
                            if (swapflag)
                                frame[widx] = MsGswap4(frame[widx]);
                            dnib = EXTRACTBITRANGE(frame[widx], 30, 2);

                            switch (dnib)
                            {
                                case 0: /* nibble=11, dnib=00: Five 6-bit differences */
                                    diffcount = 5;
                                    semask = (byte)1ul << (6 - 1); /* Sign extension from bit 6 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (24 - idx * 6), 6);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }

                                    break;

                                case 1: /* nibble=11, dnib=01: Six 5-bit differences */
                                    diffcount = 6;
                                    semask = (Int32)1ul << (5 - 1); /* Sign extension from bit 5 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (25 - idx * 5), 5);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }


                                    break;

                                case 2: /* nibble=11, dnib=10: Seven 4-bit differences */
                                    diffcount = 7;
                                    semask = (byte)1ul << (4 - 1); /* Sign extension from bit 4 */
                                    for (int idx = 0; idx < diffcount; idx++)
                                    {
                                        diff[idx] = (int)EXTRACTBITRANGE(frame[widx], (24 - idx * 4), 4);
                                        diff[idx] = (diff[idx] ^ semask) - semask;
                                    }


                                    break;

                                case 3: /* nibble=11, dnib=11: Error, undefined value */
                                    //ms_log(2, "%s: Impossible Steim2 dnib=11 for nibble=11\n", srcname);

                                    Console.WriteLine("Impossible Steim2 dnib = 11 for nibble = 11");
                                    break;
                            }

                            break;
                    } /* Done with decoding 32-bit word based on nibble */

                    /* Apply differences to calculate output samples */
                    if (diffcount > 0)
                    {
                        
                        for (int idx = 0; idx < diffcount && samplecount > 0; idx++)
                        {
                            if (outputptr == 0) /* Ignore first difference, instead store X0 */
                                output.Add(X0);
                            else /* Otherwise store difference from previous sample */
                                output.Add(output[outputptr - 1] + diff[idx]);

                            
                            samplecount--;
                            outputptr++;
                           
                        }

                    }
                } /* Done looping over nibbles and 32-bit word */

                //outputptr = output.Count-1;

            }/* End of msr_decode_steim2() */

        }

    }

    
}

