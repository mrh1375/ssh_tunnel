using System.Management;
using System.Net.NetworkInformation;

class DnsChanger
{
    static void Main()
    {

        List<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces().
            Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
        ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)).ToList();
        foreach (NetworkInterface adapter in adapters)
        {
            string networkAdapterName = adapter.Description; // نام شبکه اکتیو را وارد کنید
            string dns1 = "8.8.8.8"; // DNS اول را وارد کنید
            string dns2 = "8.8.4.4"; // DNS دوم را وارد کنید

            // Get the network adapter configuration object
            ManagementClass networkConfigClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection networkConfigs = networkConfigClass.GetInstances();
            ManagementObject networkConfig = null;
            foreach (ManagementObject config in networkConfigs)
            {
                if ((bool)config["IPEnabled"] && config["Caption"].ToString().Contains(networkAdapterName))
                {
                    networkConfig = config;
                    break;
                }
            }
            if (networkConfig == null)
            {
                Console.WriteLine($"Could not find network adapter '{networkAdapterName}'.");
                continue;
            }

            // Set the DNS servers
            ManagementBaseObject dnsConfig = networkConfig.GetMethodParameters("SetDNSServerSearchOrder");
            dnsConfig["DNSServerSearchOrder"] = new string[] { dns1, dns2 };
            ManagementBaseObject setDnsResult = networkConfig.InvokeMethod("SetDNSServerSearchOrder", dnsConfig, null);
            if ((uint)setDnsResult["ReturnValue"] != 0)
            {
                Console.WriteLine($"Failed to set DNS servers: {setDnsResult["ReturnValue"]} ON {networkAdapterName}");
                continue;
            }

            Console.WriteLine("DNS servers updated successfully.");
        }


    }
}


