using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Earth
{
    public class FDSN
    {
        public enum SEEDChannel
        {
            BHZ=0,// Broadband High Gain Seismometer (Vertical component)
            BHN =1, // Broadband High Gain Seismometer(North-South component)
            BHE=2,//Broadband High Gain Seismometer(East-West component)
            HHZ=3,//High Frequency High Gain Seismometer(Vertical component)
            HHN=4,// High Frequency High Gain Seismometer(North-South component)
            HHE=5,//High Frequency High Gain Seismometer(East-West component)
            LHZ=6,//Low Frequency High Gain Seismometer(Vertical component)
            LHN=7,//Low Frequency High Gain Seismometer(North-South component)
            LHE=8,//Low Frequency High Gain Seismometer(East-West component)
            VHZ=9,//Very Broadband High Gain Seismometer(Vertical component)
            VHN=10,//Very Broadband High Gain Seismometer(North-South component)
            VHE=11,//Very Broadband High Gain Seismometer(East-West component)
        }

        public enum RestrictedStatus
        {
            CLOSED = 0,
            OPEN =1,
        }
    }
}
