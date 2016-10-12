using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {

 
  
        static void Main ( string[] args )
        {
            start:
            Console.Write ( "Input 1 for getWorkStationType Input 2 for  GetUSBDevices Input 3 for GetAllDevices  Input 4 for GetPnPEntity:\n" );
            int i= Console.Read ( );
            
            switch (i)
            {
                case 49:
                    foreach (UInt16 item in getWorkStationType ( ))
                    {
                        Console.WriteLine ( "workstation type: {0}\n", item );
                    }
                    break;
                case 50:
                    foreach (USBDeviceInfo item in GetUSBDevices ( ))
                    {
                        Console.WriteLine ( "DeviceID {0},PnpDeviceID {1}, Description {2}\n", item.DeviceID, item.PnpDeviceID, item.Description );
                    }
                    break;

                case 51:
                    foreach (String item in GetAllDevices ( ))
                    {
                        Console.WriteLine ( "{0}\n", item );
                    }
                    break;
                case 52:
                   
                        Console.WriteLine ( "{0}\n", GetPnPEntity ( ) );
                   
                    break;
            }
            goto start;
          
        }

        public static string GetPnPEntity ( )
        {
            StringBuilder sbDevHst = new StringBuilder ( );
            ManagementObjectSearcher searcher = new ManagementObjectSearcher ( "SELECT * FROM Win32_PnPEntity" );
          //  List<string> list_
            foreach (ManagementObject mgt in searcher.Get ( ))
            {
                sbDevHst.AppendLine ( Convert.ToString ( mgt["Name"] ) );
                sbDevHst.AppendLine ( "" );
            }
            return sbDevHst.ToString ( );//获取的字符串  
        }
   
        static UInt16[] getWorkStationType ( )
        {

            UInt16[] devices = new UInt16[] { };

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher ( @"Select * From Win32_SystemEnclosure" ))
                collection = searcher.Get ( );

            foreach (var device in collection)
            {
                devices = ( UInt16[] )device.GetPropertyValue ( "ChassisTypes" );
            }
            collection.Dispose ( );
            return devices;
        }
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

        static List<String> GetAllDevices ( )
        {
            List<String> devices = new List<String> ( );

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher ( @"Select * From Win32_SystemDevices" ))
                collection = searcher.Get ( );
       
            int i = 1;
            foreach (var device in collection)
            {
                string temp = string.Empty;
                foreach (var item in device.Properties)
                {
                    temp += item.Name + ":" + item.Value + "\n"; 
                }
                temp += "\n";
                devices.Add (temp );
                ++i;
            }

            collection.Dispose ( );
            return devices;
        }
        public static string[] GetComList ( )
        {
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey ( "Hardware\\DeviceMap\\SerialComm" );
            if (keyCom != null)
            {
                string[] sSubKeys = keyCom.GetValueNames ( );
                
                foreach (string sName in sSubKeys)
                {
                    string sValue = ( string )keyCom.GetValue ( sName );
                   
                }
                return sSubKeys;
            }
            else
            {
                return null;
            }
        }
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

