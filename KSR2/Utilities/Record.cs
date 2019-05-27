using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public class Record
    {
        public DateTime Date { get; set; }
        public float MinimalTemperature { get; set; }
        public float MaximalTemperature { get; set; }
        public float Rainfall { get; set; }
        public float Evaporation { get; set; }
        public float Sunshine { get; set; }
        public float WindGustSpeed { get; set; }
        public float WindSpeed { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }
        public int Cloud { get; set; }
        public float Temperature { get; set; }
        public float RiskMm { get; set; }
    }
}
