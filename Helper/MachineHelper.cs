////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Suflow.Common.Utils {
    public class MachineHelper {

        public static String GetLocalIpAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return null;
        }

        public static Dictionary<string, string> GetLocalMacAddresses() {
            var result = new Dictionary<string, string>();
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces) {
                result.Add(networkInterface.Name, networkInterface.GetPhysicalAddress().ToString());
            }
            return result;
        }

        public static String GetProcessorId() {
            var processorID = "";
            var searcher = new ManagementObjectSearcher("Select * FROM WIN32_Processor");
            var mObject = searcher.Get();
            foreach (var obj in mObject) {
                processorID = obj["ProcessorId"].ToString();
                break;
            }
            return processorID;
        }

        public static String GetLogicalDiskVolumSerialNumber() {
            var processorID = "";
            var searcher = new ManagementObjectSearcher("Select * FROM Win32_LogicalDisk");
            var mObject = searcher.Get();
            foreach (var obj in mObject) {
                processorID = obj["VolumeSerialNumber"].ToString();
                break;
            }
            return processorID;
        }
    }
}
