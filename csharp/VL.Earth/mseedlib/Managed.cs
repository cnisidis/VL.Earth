using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VL.Lib.Collections;

namespace VL.Earth.Libmseed
{
    public enum SampleType
    {
        t,
        i,
        f,
        d
    }
    public class ManagedTraceSeg
    {
        public Int64 StartTime { get; }
        public Int64 EndTime { get; }
        public double SampleRate { get; }
        public Int64 SampleCount { get; }
        public byte SampleType { get; }

        // The key property: The actual sample data as a C# array
        public object[] Samples { get; }

        public ManagedTraceSeg(MS3TraceSeg nativeSeg)
        {
            this.StartTime = nativeSeg.starttime;
            this.EndTime = nativeSeg.endtime;
            this.SampleRate = nativeSeg.samprate;
            this.SampleCount = nativeSeg.samplecnt;
            this.SampleType = nativeSeg.sampletype;

            // This is the core logic: convert the pointer to an array
            this.Samples = ConvertSamples(nativeSeg);
        }

        public Spread<T> GetSamples<T>()
        {
            return this.Samples.Select(x => (T)x).ToSpread();
        }

        private static object[] ConvertSamples(MS3TraceSeg native)
        {
            IntPtr sptr = native.datasamples;
            int count = (int)native.samplecnt; 
            
            if (sptr == IntPtr.Zero || count <= 0)
            {
                return null;
            }

            byte sampleTypeChar = native.sampletype;

            switch (sampleTypeChar)
            {
                case (byte)'t': // 32-bit integer (Steim)
                case (byte)'i': // 32-bit integer
                    int[] intSamples = new int[count];
                    Marshal.Copy(sptr, intSamples, 0, count);
                    return intSamples.Select(x => (object)x).ToArray();

                case (byte)'f': // 32-bit float
                    float[] floatSamples = new float[count];
                    Marshal.Copy(sptr, floatSamples, 0, count);
                    return floatSamples.Select(x => (object)x).ToArray(); ;

                case (byte)'d': // 64-bit float
                    double[] doubleSamples = new double[count];
                    Marshal.Copy(sptr, doubleSamples, 0, count);
                    return doubleSamples.Select(x => (object)x).ToArray(); ;

                default:
                    Console.WriteLine($"Warning: Unsupported sample type char: {(char)sampleTypeChar}");
                    return null;
            }
        }

        
    }
    public class Segment
    {
        public  Segment()
        {

        }
    }

    
    public class Trace
    {
        public MS3TraceID Id { get; set; }
        public List<ManagedTraceSeg> Segments { get; set; } = new List<ManagedTraceSeg>();
    }

    public class TraceList
    {
        public MS3TraceList NativeList { get; }
        public List<Trace> Traces { get; set; } = new List<Trace>();

        public TraceList(MS3TraceList nativeList)
        {
            this.NativeList = nativeList;
        }
    }
    public class MSRecord
    {
        public string SID { get; }
        public DateTime StartTime {get;}
        public double SampleRate { get; }
        private Int64 SampleCount { get; }

        private short Encoding { get; }

        private char SampleType { get; }

        public Object[] Samples { get; set; }
        public MSRecord(MS3Record native) 
        {
            this.SID = native.sid;
            this.StartTime = DateTime.FromFileTimeUtc(native.starttime);
            this.SampleRate = native.samprate;
            this.SampleCount = native.samplecnt;
            this.Encoding = native.encoding;
            this.SampleType = (char)native.sampletype;
            this.Samples = ConvertSamples(native);
        }

        public void Split(out string SID, out DateTime StartTime, out int SampleRate, out int SampleCount, out int Encoding, out string SampleType)
        {
            SID = this.SID;
            StartTime = this.StartTime;
            SampleRate = (int)this.SampleRate;
            SampleCount = (int)this.SampleCount;
            Encoding = this.Encoding;
            SampleType = this.SampleType.ToString();
        }

        public Spread<T> GetSamples<T>()
        {
            return this.Samples.Select(x=>(T)x).ToSpread();
        }

        private object[] ConvertSamples(MS3Record native)
        {
            IntPtr sptr = native.datasamples;
            int count = (int)SampleCount;
            if (sptr == IntPtr.Zero || SampleCount ==0)
            {

                return new Object[] { };
                
            }
            else
            {
                
                switch(SampleType)
                {
                    case 't':
                    case 'i':
                        int[] isamples = new int[count];
                        Marshal.Copy(sptr, isamples,0, count);
                        return isamples.Select(x => (object)x).ToArray();
                        
                    case 'f':
                        float[] fsamples = new float[count];
                        Marshal.Copy(sptr, fsamples, 0, count);
                        return fsamples.Select(x=>(object)x).ToArray();
                        
                    case 'b':
                        byte[] bsamples = new byte[count];
                        Marshal.Copy(sptr, bsamples, 0, count);
                        return bsamples.Select(x => (object)x).ToArray();
                        
                    default:
                        return new Object[] { };

                }
            }
        }
    }
}
