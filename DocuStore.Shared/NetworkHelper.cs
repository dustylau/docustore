using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DocuStore.Shared
{
    public class NetworkHelper
    {
        public static string GetHostName()
        {
            return Dns.GetHostEntry("").HostName;
        }

        public static IPEndPoint IPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
        {
            var hostAddresses = Dns.GetHostAddresses(hostName);

            if (hostAddresses.Length == 0)
            {
                throw new ArgumentException(
                    "Unable to retrieve address from specified host name.",
                    "hostName"
                );
            }

            var ipAddresses = new List<IPAddress>();

            foreach (var address in hostAddresses)
            {
                if (Regex.IsMatch(address.ToString(), @"\d+\.\d+\.\d+\.\d+", RegexOptions.IgnoreCase))
                    ipAddresses.Add(address);
            }

            if (throwIfMoreThanOneIP && hostAddresses.Length > 1)
            {
                throw new ArgumentException(
                    "There is more that one IP address to the specified host.",
                    "hostName"
                );
            }

            if (!ipAddresses.Any() && hostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                ipAddresses.Add(IPAddress.Loopback);

            return new IPEndPoint(ipAddresses[0], port); // Port gets validated here.
        }

        public static IPEndPoint IPEndPointFromHostNameOrIPAddress(string address, int port, bool throwIfMoreThanOneIP)
        {
            return Regex.IsMatch(address, @"\d+\.\d+\.\d+\.\d+", RegexOptions.IgnoreCase)
                ? IPEndPointFromIPAddress(address, port)
                : IPEndPointFromHostName(address, port, throwIfMoreThanOneIP);
        }

        public static IPEndPoint IPEndPointFromIPAddress(string address, int port)
        {
            var ipAddress = IPAddress.Parse(address);

            return new IPEndPoint(ipAddress, port); // Port gets validated here.
        }
    }
}