using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;

namespace PhoneApp2
{
    public class PositionInformation
    {
        public PositionInformation(DateTimeOffset timeStamp, GeoCoordinate coord)
        {
            this.timeStamp = timeStamp;
            this.latitude = coord.Latitude;
            this.longitude = coord.Longitude;
        }

        public PositionInformation(DateTimeOffset timeStamp, double latitude, double longitude) 
        {
            this.timeStamp = timeStamp;
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public DateTimeOffset timeStamp { get; private set; }
        public double latitude { get; private set; }
        public double longitude { get; private set; }
    }
}
