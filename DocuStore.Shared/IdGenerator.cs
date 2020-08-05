using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace DocuStore.Shared
{
    public class IdGenerator
    {
        public enum IdGenerationMethod
        {
            Guid,
            Timestamp,
            TimestampAndMac,
            IncrementingShort,
            IncrementingInteger,
            IncrementingLong,
            JulianTimeStamp,
            Custom
        }

        public static IdGenerationMethod GenerationMethod { get; set; } = IdGenerationMethod.Guid;

        public static Func<object> CustomGenerator { get; set; }

        public static Dictionary<IdGenerationMethod, string> Formats { get; set; } =
            new Dictionary<IdGenerationMethod, string>
            {
                { IdGenerationMethod.Guid, "D" },
                { IdGenerationMethod.Timestamp, "yyyyMMddHHmmssfffffff" },
                { IdGenerationMethod.IncrementingShort, "00000" },
                { IdGenerationMethod.IncrementingInteger, "0000000000" },
                { IdGenerationMethod.IncrementingLong, "00000000000000000000" },
                { IdGenerationMethod.Custom, "{0}" }
            };

        private static string _macAddress;
        public static string MacAddress
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_macAddress))
                    _macAddress =
                    (
                        from nic in NetworkInterface.GetAllNetworkInterfaces()
                        where nic.OperationalStatus == OperationalStatus.Up
                        select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();

                return _macAddress;
            }
        }

        private static readonly object LastTimestampLock = new object();
        private static string _lastTimestamp;

        private static string GetTimestamp()
        {
            var timestamp = DateTime.Now.ToString(Formats[IdGenerationMethod.Timestamp]);

            lock (LastTimestampLock)
            {
                while (timestamp == _lastTimestamp)
                {
                    Thread.Sleep(1);
                    timestamp = DateTime.Now.ToString(Formats[IdGenerationMethod.Timestamp]);
                }

                _lastTimestamp = timestamp;
            }

            return timestamp;
        }

        private static readonly object LastJulianTimestampLock = new object();
        private static string _lastJulianTimestamp;

        private static string GetJulianTimestamp()
        {
            var timestamp = $"{DateTime.Now.DayOfYear:000}-{(DateTime.Now.Year - 2000):00}-{DateTime.Now:HHmmssFFFF}";

            lock (LastJulianTimestampLock)
            {
                while (timestamp == _lastJulianTimestamp)
                {
                    Thread.Sleep(1);
                    timestamp =
                        $"{DateTime.Now.DayOfYear:000}-{(DateTime.Now.Year - 2000):00}-{DateTime.Now:HHmmssFFFF}";
                }

                _lastJulianTimestamp = timestamp;
            }

            return timestamp;
        }

        private static readonly object ShortLock = new object();
        private static ushort _short;

        public static void SeedShort(ushort seed)
        {
            lock (ShortLock)
                _short = seed;
        }

        private static ushort GetNextShort()
        {
            lock (ShortLock)
                return ++_short;
        }

        private static readonly object IntegerLock = new object();
        private static uint _integer;

        public static void SeedInteger(uint seed)
        {
            lock (IntegerLock)
                _integer = seed;
        }

        private static uint GetNextInteger()
        {
            lock (IntegerLock)
                return ++_integer;
        }

        private static readonly object LongLock = new object();
        private static ulong _long;

        public static void SeedLong(ulong seed)
        {
            lock (LongLock)
                _long = seed;
        }

        private static ulong GetNextLong()
        {
            lock (LongLock)
                return ++_long;
        }

        public static string NewId()
        {
            return NewId(GenerationMethod);
        }

        public static string NewId(IdGenerationMethod generationMethod)
        {
            switch (generationMethod)
            {
                case IdGenerationMethod.Guid:
                    return Guid.NewGuid().ToString(Formats[IdGenerationMethod.Guid]);
                case IdGenerationMethod.Timestamp:
                    return GetTimestamp();
                case IdGenerationMethod.TimestampAndMac:
                    return GetTimestamp() + MacAddress;
                case IdGenerationMethod.IncrementingShort:
                    return GetNextShort().ToString(Formats[IdGenerationMethod.IncrementingShort]);
                case IdGenerationMethod.IncrementingInteger:
                    return GetNextInteger().ToString(Formats[IdGenerationMethod.IncrementingInteger]);
                case IdGenerationMethod.IncrementingLong:
                    return GetNextLong().ToString(Formats[IdGenerationMethod.IncrementingLong]);
                case IdGenerationMethod.Custom:
                    return string.Format(Formats[IdGenerationMethod.Custom], CustomGenerator());
                case IdGenerationMethod.JulianTimeStamp:
                    return GetJulianTimestamp();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}