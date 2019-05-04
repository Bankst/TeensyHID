using System;
using TeensyHID.HID;

namespace TeensyHID
{
	public class TeensyHID : IDisposable
	{
		public const int VendorId = 0x16C0;
		public const int ProductId = 0x0486;
		public const ushort UsagePage = 0x0200;

		public static event InsertedEventHandler Inserted;
		public static event RemovedEventHandler Removed;
		public static event DataReceivedEventHandler DataReceived;

		public delegate void InsertedEventHandler(HidDevice device);
		public delegate void RemovedEventHandler(HidDevice device);
		public delegate void DataReceivedEventHandler(byte[] data);

		private readonly HidDevice _teensyHID;

		public TeensyHID(string devicePath) : this(HidDevices.GetDevice(devicePath)) { }

		public TeensyHID(HidDevice hidDevice)
		{
			_teensyHID = hidDevice;
			_teensyHID.Inserted += TeensyHID_Inserted;
			_teensyHID.Removed += TeensyHID_Removed;

			if (!_teensyHID.IsOpen) _teensyHID.OpenDevice();
			_teensyHID.MonitorDeviceEvents = true;
		}

		public string DevicePath => _teensyHID.DevicePath;

		public bool IsConnected => _teensyHID.IsConnected;

        public string Serial => _teensyHID.Serial;

		public void SendData(byte[] buffer)
		{
			var report = _teensyHID.CreateReport();

			report.ReportId = 0x00; // TODO: maybe right?
			report.Data = buffer;

			_teensyHID.WriteReport(report);
		}

		private static void TeensyHID_Inserted(IHidDevice hidDevice)
		{
			Inserted?.Invoke((HidDevice)hidDevice);
		}

		private static void TeensyHID_Removed(IHidDevice hidDevice)
		{
			Removed?.Invoke((HidDevice)hidDevice);
		}

        public override string ToString() => _teensyHID.ToString();

        public void Dispose()
		{
			_teensyHID.CloseDevice();
			GC.SuppressFinalize(this);
		}

		~TeensyHID()
		{
			Dispose();
		}
	}
}
