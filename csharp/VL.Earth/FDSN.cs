


using System.Xml.Serialization;
using VL.Lib.Collections;
using Stride.Core.Mathematics;
using System.Runtime.CompilerServices;

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
            extent
            

    }

        public enum RequestServiceType
        {
            station,
            dataselect,
            availability,
            @event,
            wfcatalog
    }

        public enum RequestFormat
        {
            xml,
            text,
            geocsv,
            mseed,
            json
            
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
        if (this != null)
            return this.code;
        else
            return string.Empty;
    }

    public string GetDescription()
    {
        if (this != null)
            return this.Description;
        else
            return string.Empty;
    }

    public Spread<CommentType> GetComments()
    {
        
        return this.Comment.ToSpread();
    }

    public string GetSourceID()
    {
        return this.sourceID;
    }

    public Spread<OperatorType> GetOperators()
    {
        return this.Operator.ToSpread();
    }

    public void GetDates(out DateTime Start, out DateTime End)
    {
        Start = this.startDate;
        End = this.endDate;
    }
    public void Split(out string code,  out string SourceID, out DataAvailabilityType DataAvailability)
    {
        code = this.code;
        SourceID = this.sourceID;
        DataAvailability = this.DataAvailability;
        
        
    }
}

public partial class CommentType
{
    public void Split(out string Id, out string Subject, out string Value, out string Author, out DateTime BeginTime, out DateTime EndTime)
    {
        Id = this.id;
        Value = this.Value;
        Subject = this.subject;
        Author = string.Join(" ", this.Author.SelectMany(x => x.Name));
        BeginTime = this.BeginEffectiveTime;
        EndTime = this.EndEffectiveTime;
    }

    public string GetValue()
    {
        return this.Value;
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

    public Spread<ChannelType> GetChannels()
    {
        return this.Channel.ToSpread();
    }
}

public partial class ChannelType
{
    public string GetCode()
    {

        return this.code;
    }

    public void Split(out string Code, out RestrictedStatusType RestrictedStatus, out string Comment)
    {
        Code = this.code;
        RestrictedStatus = this.restrictedStatus;
        Comment = String.Join(",", this.Comment.SelectMany(x => x.GetValue()));



    }
}
    public partial class DataAvailabilityType
{
    
}

public partial class EquipmentType
{
    public void Split(out DateTime InstallationDate, out string Type)
    {
        InstallationDate = this.InstallationDate;
        Type = this.Type;
    }
}
    


