using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;

namespace PhoneApp2
{
    class PositionInformation
    {
        public PositionInformation(DateTimeOffset timeStamp, double latitude, double longitude) 
        {}

        public DateTimeOffset timeStamp { get; private set; }
        public double latitude { get; private set; }
        public double longitude { get; private set; }
    }
}
