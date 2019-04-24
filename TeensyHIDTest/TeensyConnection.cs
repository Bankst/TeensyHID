using Device.Net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeensyHIDTest
{
	public class TeensyConnection : IDisposable
	{
		private readonly List<FilterDeviceDefinition> _deviceDefinitions = new List<FilterDeviceDefinition>
		{
			new FilterDeviceDefinition {VendorId = 0x16C0},
			// new FilterDeviceDefinition {DeviceType = DeviceType.Hid, VendorId = 0x16C0, ProductId = 0x0486}
		};
		
		public event EventHandler TeensyInitialized;
		public event EventHandler TeensyDisconnected;

		public IDevice TeensyDevice { get; private set; }
		public DeviceListener DeviceListener { get; private set; }

		private void Teensy_TeensyInitialized(object sender, EventArgs e)
		{
			Console.WriteLine("Got Teensy!");
		}

		private void DevicePoller_DeviceInitialized(object sender, DeviceEventArgs e)
		{
			Console.WriteLine("Got Teensy!");
			TeensyDevice = e.Device;
			TeensyInitialized?.Invoke(this, new EventArgs());
		}

		private void DevicePoller_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			Console.WriteLine("Lost Teensy!");
			TeensyDevice = null;
			TeensyDisconnected?.Invoke(this, new EventArgs());
		}

		public void StartListening()
		{
			TeensyDevice?.Dispose();
			DeviceListener = new DeviceListener(_deviceDefinitions, 1000);
			DeviceListener.DeviceDisconnected += DevicePoller_DeviceDisconnected;
			DeviceListener.DeviceInitialized += DevicePoller_DeviceInitialized;
			DeviceListener.Start();
			TeensyInitialized = Teensy_TeensyInitialized;
		}

		public void GetDevices()
		{
			Console.WriteLine("Searching for TeensyHID Devices...");
			var devices = DeviceManager.Current.GetDevicesAsync(_deviceDefinitions).Result;
		}

		public async Task InitializeTeensyAsync()
		{
			Console.WriteLine("Searching for TeensyHID Devices...");
			var devices = DeviceManager.Current.GetDevicesAsync(_deviceDefinitions).Result;
			int devicesCount = devices.Count;
			Console.WriteLine($"Found {devices.Count} devices.");
			TeensyDevice = devices.FirstOrDefault();
			if (TeensyDevice != null) await TeensyDevice.InitializeAsync();
		}

		public async Task<TeensyPacket> WriteThenReadAsync(TeensyPacket txPacket)
		{
			return new TeensyPacket(await TeensyDevice?.WriteAndReadAsync(txPacket.GetByteArray()));
		}
		
		public void Dispose()
		{
			TeensyDevice?.Dispose();
		}
	}
}
