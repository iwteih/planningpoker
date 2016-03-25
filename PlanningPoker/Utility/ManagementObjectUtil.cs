using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace PlanningPoker.Utility
{
    static class ManagementObjectUtil
    {
        public static string GetBiosSerialNumber()
        {
            string serialNumber = string.Empty;

            ManagementObjectSearcher MOS = new ManagementObjectSearcher(" Select * From Win32_BIOS ");

            foreach (ManagementObject getserial in MOS.Get())
            {
                serialNumber = getserial["SerialNumber"].ToString();
                break;
            }

            return serialNumber;
        }
    }
}
