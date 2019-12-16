using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Central.Models
{
    public class CosmeticsGroup
    {
        public string MainColor { get; set; } = "#21618C";
        public string SecondaryColor { get; set; } = "#5D6D7E";
        public string AccentColor { get; set; } = "#F4D03F";

        public string LightChassis { get; set; } = "Default";
        public string MediumChassis { get; set; } = "Default";
        public string HeavyChassis { get; set; } = "Default";

        public void Deserialize(string input)
        {
            var toks = input.Split(';', 6);
            if (toks.Length == 6)
            {
                MainColor = toks[0];
                SecondaryColor = toks[1];
                AccentColor = toks[2];
                LightChassis = toks[3];
                MediumChassis = toks[4];
                HeavyChassis = toks[5];
            }
        }

        public string Serialize()
        {
            return MainColor + ";" + SecondaryColor + ";" + AccentColor + ";" + LightChassis + ";" + MediumChassis + ";" + HeavyChassis;
        }
    }
}
