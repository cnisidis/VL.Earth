using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}

