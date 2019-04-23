using System;
using System.Collections.Generic;
using System.Linq;
using Hid.Net.Windows;

using System.Threading.Tasks;
using Device.Net;
using Usb.Net.Windows;

namespace TeensyHIDTest
{
	public class TeensyConnection : IDisposable
	{
		private readonly List<FilterDeviceDefinition> _deviceDefinitions = new List<FilterDeviceDefinition>
		{
			new FilterDeviceDefinition {DeviceType = DeviceType.Hid, VendorId = 0x16C0, ProductId = _productId, UsagePage = 0xFFAB}
		};
		
		public event EventHandler TeensyInitialized;
		public event EventHandler TeensyDisconnected;

		public IDevice TeensyDevice { get; private set; }
		public DeviceListener DeviceListener { get; private set; }

		private void DevicePoller_DeviceInitialized(object sender, DeviceEventArgs e)
		{
			TeensyDevice = e.Device;
			TeensyInitialized?.Invoke(this, new EventArgs());
		}

		private void DevicePoller_DeviceDisconnected(object sender, DeviceEventArgs e)
		{
			TeensyDevice = null;
			TeensyDisconnected?.Invoke(this, new EventArgs());
		}

		public void StartListening()
		{
			TeensyDevice?.Dispose();
			DeviceListener = new DeviceListener(_deviceDefinitions, 1000);
			DeviceListener.DeviceDisconnected += DevicePoller_DeviceDisconnected;
			DeviceListener.DeviceInitialized += DevicePoller_DeviceInitialized;
		}

		public async Task InitializeTeensyAsync()
		{
			var devices = await DeviceManager.Current.GetDevicesAsync(_deviceDefinitions);
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
