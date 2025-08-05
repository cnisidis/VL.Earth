


using System.Xml.Serialization;
using VL.Lib.Collections;
using Stride.Core.Mathematics;

namespace VL.Earth.FDSN;

    public static class FDSN
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

        public enum RequestType
        {
            query,
            queryauth,
            version,
            
        }

        public enum RequestServiceType
        {
            station,
            dataselect,


        }

        public enum RequestFormat
        {
            txt,
            xml,
            miniseed
        }

        public enum RequestLevel
        {
            network,
            station,
            channel, 
            response
        }

        public enum RequestDataQuality
        {
            D,
            R,
            Q,
            M,
            B

        }


        public static void ParseRoot(string XMLString, out RootType stationData)
        {
            stationData = new RootType();
            try
            {
                // Create a StringReader to read the XML string
                using (StringReader reader = new StringReader(XMLString))
                {
                    // Create an XmlSerializer for your root class (RootType)
                    XmlSerializer serializer = new XmlSerializer(typeof(RootType));

                    // Deserialize the XML and cast it to your root class
                    stationData = (RootType)serializer.Deserialize(reader);

                    // Now you can access the deserialized data
                    Console.WriteLine("Deserialization successful!");
                    Console.WriteLine($"Source: {stationData.Source}");

                    // The 'Network' property is an array, so let's iterate through it
                    if (stationData.Network != null && stationData.Network.Length > 0)
                    {
                        foreach (var network in stationData.Network)
                        {
                            Console.WriteLine($"Network Code: {network.code}");
                            Console.WriteLine($"Network Description: {network.Description}");

                            if (network.Station != null && network.Station.Length > 0)
                            {
                                Console.WriteLine($"Number of Stations: {network.Station.Length}");
                                // Access the first station as an example
                                Console.WriteLine($"First Station Code: {network.Station[0].code}");
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Deserialization failed: {ex.Message}");
                // The InnerException often contains more specific details about the error
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
            
        }
    }

public partial class RootType
{
    public void Split(out Spread<NetworkType> Networks)
    {
        Networks = this.Network.ToSpread();
        
    }

    public void Stations(out Spread<StationType> Stations)
    {
        Stations = this.Network.SelectMany(x => x.Station).ToSpread();
    }
}


public partial class NetworkType
{
    public string GetCode()
    {
        return this.code;
    }
}

public partial class StationType
{
    public void Split(out string Code,  out double Elevation, out int ChannelsCount, out Vector2 LongLat)
    {
        Code = this.code;
        ChannelsCount = int.Parse(this.TotalNumberChannels);
        //Channels = this.Channel.Length > 0 ? this.Channel.ToSpread() : null;
        Elevation = this.Elevation.Value;
        LongLat = new Vector2 ((float)this.Longitude.Value, (float)this.Latitude.Value);
    }

    public string GetCode()
    {
        return this.code;
    }

    public Vector2 GetLongLat()
    {
        return new Vector2((float)this.Longitude.Value, (float)this.Latitude.Value);
        
    }
}
    


