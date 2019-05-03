using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TeensyHID.Comm;
using TeensyHID.HID;

namespace TeensyHID
{
	internal class Program
	{
		private static readonly ConcurrentDictionary<string, HIDConnection> TeensyConnections = new ConcurrentDictionary<string, HIDConnection>();

		private static void Main(string[] args)
		{
            HIDMessageHandler.Store(HIDOpcode.INIT_OK, HIDHandlers.INIT_OK);
			
			var detectedDevices = EnumerateTeensyHIDDevices();
			if (detectedDevices.Any())
			{
				foreach (var device in detectedDevices)
				{
					var teensyDevice = new TeensyHID(device);
					var teensyConnection = new HIDConnection(teensyDevice);
					if (!teensyConnection.IsConnected) continue;
					if (!TeensyConnections.TryAdd(teensyDevice.DevicePath, teensyConnection))
					{
						Debug.LogError("Failed to add device.");
					}
                }
            }
			else
			{
				// while (!EnumerateTeensyHIDDevices().Any())
				// {
				// 	// ???
				// }
			}
            Console.ReadLine();
		}

		private static List<HidDevice> EnumerateTeensyHIDDevices()
		{
			return HidDevices.Enumerate(TeensyHID.VendorId, TeensyHID.ProductId, TeensyHID.UsagePage).ToList();
        }
	}
}
