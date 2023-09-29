using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VL.Earth
{
    public static class FEarthDateTime
    {
        public static void TryParseExact(string datetime, string format, DateTimeStyles style, out DateTime parsedDate, out bool result)
        {
            
            if(DateTime.TryParseExact(datetime, format, null, style, out parsedDate ))
                result = true;
            else
                result = false;
            
        }
    }
}
