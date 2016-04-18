using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace X64Tester
{
    public sealed class SensorData
    {
        public SensorData()
        {
            TimeStamp = DateTime.Now;
        }
        public double SoilHumidity { get; set; }
        public double SoilTemp { get; set; }
        public double AirHumidity { get; set; }
        public double AirTemp { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

        public sealed override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
