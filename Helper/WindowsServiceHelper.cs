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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace Suflow.Common.Utils
{
    public class WindowsServiceHelper
    {
        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(serviceName);
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                var service = new ServiceController(serviceName);
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
        }

        public static void DeleteService(string serviceName)
        {
            try
            {
                var process = Process.Start("sc", string.Format("delete \"{0}\"", serviceName));
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException);
            }
        }

        public static ServiceController[] GetServices(string nameFilter, string directoryFilter)
        {
            var res = new List<ServiceController>();
            var services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                var add = true;

                if (!string.IsNullOrEmpty(nameFilter))
                    add = service.DisplayName.ToLower().Contains(nameFilter);

                if (add && !string.IsNullOrEmpty(directoryFilter))
                {
                    var dir = GetPathToTheExecutable(service.ServiceName);
                    add = dir.ToLower().Contains(directoryFilter.ToLower());
                }
                if (add)
                    res.Add(service);
            }
            return res.ToArray();
        }

        public static string GetPathToTheExecutable(string serviceName)
        {
            var machineName = Environment.MachineName;
            var registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            var keyHKLM = Registry.LocalMachine;

            RegistryKey key;
            if (machineName != "")
            {
                key = RegistryKey.OpenRemoteBaseKey
                  (RegistryHive.LocalMachine, machineName).OpenSubKey(registryPath);
            }
            else
            {
                key = keyHKLM.OpenSubKey(registryPath);
            }
            var value = key.GetValue("ImagePath").ToString();
            key.Close();
            return value;
        }
    }
}
