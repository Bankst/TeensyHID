using System;
using TeensyHID.HID;

namespace TeensyHID
{
	public class TeensyHID : IDisposable
	{
		public const int VendorId = 0x16C0;
		public const int ProductId = 0x0486;
		public const ushort UsagePage = 0x0200;

		public event InsertedEventHandler Inserted;
		public event RemovedEventHandler Removed;
		public event DataReceivedEventHandler DataReceived;

		public delegate void InsertedEventHandler();
		public delegate void RemovedEventHandler();
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

		private void TeensyHID_Inserted()
		{
			Inserted?.Invoke();
		}

		private void TeensyHID_Removed()
		{
			Removed?.Invoke();
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
