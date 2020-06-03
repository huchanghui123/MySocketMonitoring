using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinet
{
    class HardwareSensors
    {
        public int temperature { get; set; }
        public int temperature_min { get; set; }
        public int temperature_max { get; set; }
        public float cpu_clock { get; set; }
        public int mem_load { get; set; }


        public HardwareSensors(int temperature, int temperature_min,
            int temperature_max, float cpu_clock, int mem_load)
        {
            this.temperature = temperature;
            this.temperature_min = temperature_min;
            this.temperature_max = temperature_max;
            this.cpu_clock = cpu_clock;
            this.mem_load = mem_load;
        }

        public override string ToString()
        {
            return "cpu package temperature:" + this.temperature + "°C CPU_SPEED:" + this.cpu_clock + "GHz Memory Load:" + this.mem_load + "%";
        }
    }
}
