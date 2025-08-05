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

    public class Segment
    {
        public  Segment()
        {

        }
    }

    public class Trace
    {
        public Trace()
        {

        }
    }
    public class TraceList
    {
        public List<MS3TraceSeg> Segments = new();
        public List<MS3TraceID> Traces = new();
        public int TraceCount { get; private set; }
        public TraceList(MS3TraceList native)
        {
            
            this.TraceCount = (int)native.numtraceids;
            
            IntPtr sptr = IntPtr.Zero;
            
           
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
                        break;
                    case 'f':
                        float[] fsamples = new float[count];
                        Marshal.Copy(sptr, fsamples, 0, count);
                        return fsamples.Select(x=>(object)x).ToArray();
                        break;
                    case 'b':
                        byte[] bsamples = new byte[count];
                        Marshal.Copy(sptr, bsamples, 0, count);
                        return bsamples.Select(x => (object)x).ToArray();
                        break;
                    default:
                        return new Object[] { };

                }
            }
        }
    }
}
