using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinet
{
    internal sealed class CpuTemperatureReader : IDisposable
    {
        private readonly Computer _computer;
        private HardwareSensors hs = null;
        private int temperature = 0;
        private int temperature_min = 0;
        private int temperature_max = 0;
        private float cpu_clock = 0.00f;
        private int mem_load = 0;

        public CpuTemperatureReader()
        {
            _computer = new Computer
            {
                CPUEnabled = true,
                RAMEnabled = true
            };
            _computer.Open();
        }

        public HardwareSensors GetTemperaturesInCelsius()
        {
            int i = 0;
            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update(); //use hardware.Name to get CPU model
                //遍历CPU传感器
                if (hardware.HardwareType == HardwareType.CPU)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        //温度
                        if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                        {
                            Console.WriteLine("{0}, Value={1}, Min Value={2}, Max Value={3}",
                                sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                            if (sensor.Name.Contains("Package"))
                            {
                                temperature = Convert.ToInt32(sensor.Value);
                                temperature_min = Convert.ToInt32(sensor.Min);
                                temperature_max = Convert.ToInt32(sensor.Max);
                            }
                        }
                        //时钟
                        if (sensor.SensorType == SensorType.Clock && sensor.Value.HasValue)
                        {
                            Console.WriteLine("{0}, Value={1}, Min Value={2}, Max Value={3}",
                                sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                            if (!sensor.Name.Contains("Bus"))
                            {
                                i++;
                                cpu_clock += (float)sensor.Value;
                            }
                        }
                    }
                }
                //遍历内存
                if (hardware.HardwareType == HardwareType.RAM)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                        {
                            Console.WriteLine("{0}, Value={1}", sensor.Name, sensor.Value.Value);
                            mem_load = Convert.ToInt32(sensor.Value);
                        }
                    }
                }
            }
            cpu_clock = cpu_clock / i / 1000f;
            hs = new HardwareSensors(temperature, temperature_min, temperature_max, cpu_clock, mem_load);
            return hs;
        }

        public void Dispose()
        {
            try
            {
                _computer.Close();
            }
            catch (Exception)
            {
                //ignore closing errors
            }
        }
    }
}
