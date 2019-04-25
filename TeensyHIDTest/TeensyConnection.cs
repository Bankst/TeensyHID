using Device.Net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hid.Net.Windows;

namespace TeensyHIDTest
{
	public class TeensyConnection : IDisposable
	{
		private readonly List<FilterDeviceDefinition> _deviceDefinitions = new List<FilterDeviceDefinition>
		{
//			new FilterDeviceDefinition {VendorId = 0x16C0},
//			new FilterDeviceDefinition { DeviceType =  DeviceType.Hid},
			new FilterDeviceDefinition {DeviceType = DeviceType.Hid, VendorId = 0x16C0, ProductId = 0x0486, UsagePage = 0xFFAB}
		};
		
		public event EventHandler TeensyInitialized;
		public event EventHandler TeensyDisconnected;

		public IDevice TeensyDevice { get; private set; }
		public DeviceListener DeviceListener { get; private set; }

		public TeensyConnection()
		{
			WindowsHidDeviceFactory.Register();
		}


		private void Teensy_TeensyInitialized(object sender, EventArgs e)
		{
			Console.WriteLine("TeensyHID Connected.");
		}

		private void Teensy_TeensyDisconnected(object sender, EventArgs e)
		{
			Console.WriteLine("Lost TeensyHID Connection.");
		}

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

			DeviceListener = new DeviceListener(_deviceDefinitions, 500);
			DeviceListener.DeviceDisconnected += DevicePoller_DeviceDisconnected;
			DeviceListener.DeviceInitialized += DevicePoller_DeviceInitialized;

			TeensyDisconnected = Teensy_TeensyDisconnected;
			TeensyInitialized = Teensy_TeensyInitialized;

			Console.WriteLine("Waiting for TeensyHID connection...");
			DeviceListener.Start();
		}

		public TeensyPacket WriteThenReadAsync(TeensyPacket txPacket)
		{
			Console.WriteLine(txPacket.ToString());
			try
			{
				var rxPacket = new TeensyPacket(TeensyDevice?.WriteAndReadAsync(txPacket.ByteArray).Result);
				Console.WriteLine(rxPacket.ToString());
				return rxPacket;
			}
			catch
			{
				Console.WriteLine("TeensyHID RX - Failed.");
				return null;
			}
		}
		
		public void Dispose()
		{
			TeensyDevice?.Dispose();
		}
	}
}
