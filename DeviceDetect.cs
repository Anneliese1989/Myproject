using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace Jingge.Common
{
    public static class HardDeviceDetect
    {
        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <returns></returns>
        static UInt16[] getWorkStationType ( )
        {

            UInt16[] devices = new UInt16[] { };

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher ( @"Select * From Win32_SystemEnclosure" ))
                collection = searcher.Get ( );

            foreach (var device in collection)
            {
                devices= ( UInt16[] )device.GetPropertyValue ( "ChassisTypes" );
            }
            collection.Dispose ( );
            return devices;
        }
        /// <summary>
        /// 获取USB设备列表
        /// </summary>
        /// <returns></returns>
        static List<USBDeviceInfo> GetUSBDevices ( )
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo> ( );

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher ( @"Select * From Win32_USBHub" ))
                collection = searcher.Get ( );

            foreach (var device in collection)
            {
                devices.Add ( new USBDeviceInfo (
                ( string )device.GetPropertyValue ( "DeviceID" ),
                ( string )device.GetPropertyValue ( "PNPDeviceID" ),
                ( string )device.GetPropertyValue ( "Description" )
                ) );
            }

            collection.Dispose ( );
            return devices;
        }
        class USBDeviceInfo
        {
            public USBDeviceInfo ( string deviceID, string pnpDeviceID, string description )
            {
                this.DeviceID = deviceID;
                this.PnpDeviceID = pnpDeviceID;
                this.Description = description;
            }
            public string DeviceID { get; private set; }
            public string PnpDeviceID { get; private set; }
            public string Description { get; private set; }
        }
    }
}
