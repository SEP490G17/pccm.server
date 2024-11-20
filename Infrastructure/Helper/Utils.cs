using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace API.Helper
{
    public class Utils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }


        // có chế biến cho .NET Core MVC
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return "Invalid IP:" + ex.Message;
            }

            return "127.0.0.1";
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CultureInfo.InvariantCulture.CompareInfo;
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }

    public static class TimeExtensions
    {
        public static string To24HourFormat(this string time)
        {
            if (DateTime.TryParseExact(time, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime.ToString("HH:mm");
            }
            return time;
        }
    }


}