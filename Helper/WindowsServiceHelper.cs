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
