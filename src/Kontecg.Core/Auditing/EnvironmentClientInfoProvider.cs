using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using Castle.Core.Logging;
using Hardware.Info;
using Kontecg.Extensions;
using Kontecg.Json;

namespace Kontecg.Auditing
{
    [DebuggerStepThrough]
    public class EnvironmentClientInfoProvider : IClientInfoProvider, IShouldInitialize
    {
        private readonly IHardwareInfo _hardwareInfo;
        private readonly Dictionary<string, object> _properties;
        private bool _wasInitalized;

        private readonly object SyncObj = new();

        public EnvironmentClientInfoProvider(
            IHardwareInfo hardwareInfo)
        {
            _hardwareInfo = hardwareInfo;
            Properties = new Dictionary<string, object>();
            Logger = NullLogger.Instance;
            _wasInitalized = false;
        }

        public ILogger Logger { get; set; }

        public string ClientId => GetClientId() ?? UuidGenerator.Instance.Create().ToString("X2");

        public string ClientInfo => GetClientInfo();

        public string ClientIpAddress => GetClientIpAddress()?.ToString();

        public string ComputerName => GetComputerName();

        public string DomainName => GetDomainName();

        public string Version => GetBaseVersion();

        /// <summary>
        ///     Shortcut to set/get <see cref="Properties" />.
        /// </summary>
        public object this[string key]
        {
            get => Properties[key];
            set => Properties[key] = value;
        }

        /// <summary>
        ///     Can be used to add custom properties for this client.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get => _properties;
            private init => _properties = value ?? throw new ArgumentNullException(nameof(value));
        }

        protected virtual string GetClientId()
        {
            var containMotherboardProperty = Properties.TryGetValue("Motherboard", out dynamic motherboard);
            if (containMotherboardProperty && motherboard != null) return motherboard.SerialNumber;
            return null;
        }

        protected virtual string GetClientInfo()
        {
            return $"{{{GetOSAndServicePack()},\"Properties\": {Properties.ToJsonString()}}}";
        }

        protected virtual IPAddress GetClientIpAddress()
        {
            try
            {
                return GetLocalIPAddressWithNetworkInterface();
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't get ip address", ex);
            }

            return null;
        }

        protected virtual string GetComputerName()
        {
            return Environment.MachineName;
        }

        protected virtual string GetDomainName()
        {
            return Environment.UserDomainName;
        }

        protected virtual string GetBaseVersion()
        {
            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var v = asm.GetName().Version?.ToString();
            return v;
        }

        private string GetOSAndServicePack()
        {
            var os = _hardwareInfo.OperatingSystem.Name;
            return
                $"\"OS\": \"{os} {(Environment.Is64BitOperatingSystem ? "(x64)" : "(x86)")}\",\"Version\": \"{_hardwareInfo.OperatingSystem.VersionString}\"";
        }

        private IPAddress GetLocalIPAddressWithNetworkInterface()
        {
            return (from @interface in NetworkInterface.GetAllNetworkInterfaces()
                where @interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                      @interface.OperationalStatus == OperationalStatus.Up
                from ip in @interface.GetIPProperties().UnicastAddresses
                where ip.Address.AddressFamily.HasFlag(AddressFamily.InterNetwork)
                select ip.Address).FirstOrDefault();
        }

        protected virtual void FillAdditionalProperties()
        {

            try
            {
                lock (SyncObj)
                {
                    if (!_wasInitalized)
                    {
                        _hardwareInfo.RefreshCPUList(false);
                        _hardwareInfo.RefreshDriveList();
                        _hardwareInfo.RefreshMemoryList();
                        _hardwareInfo.RefreshMotherboardList();
                        _hardwareInfo.RefreshOperatingSystem();
                        _hardwareInfo.RefreshPrinterList();

                        Properties.Clear();
                        Properties.Add("CPU", _hardwareInfo.CpuList.Select(cpu => new {cpu.Name, cpu.NumberOfCores}).ToArray());
                        Properties.Add("RAM", _hardwareInfo.MemoryList.Select(memory => new { memory.Manufacturer, Capacity = memory.Capacity.ToSize() }).ToArray());
                        Properties.Add("Motherboard", new { _hardwareInfo.MotherboardList.First().Manufacturer, _hardwareInfo.MotherboardList.First().Product, _hardwareInfo.MotherboardList.First().SerialNumber });
                        Properties.Add("Drives", _hardwareInfo.DriveList.Where(drive => drive.Size > 0).Select(drive => new { SerialHash = drive.SerialNumber.ToMd5(), Capacity = drive.Size.ToSize()}).ToArray());
                        Properties.Add("Printer", _hardwareInfo.PrinterList.Where(printer => printer.Default).Select(printer => new { printer.Name, printer.Network }).ToArray());

                        _wasInitalized = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Couldn't obtain hardware info client!", ex);
            }
        }

        public void Initialize()
        {
            FillAdditionalProperties();
        }
    }
}
