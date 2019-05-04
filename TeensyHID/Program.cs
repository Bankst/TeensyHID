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

		private static void Main()
		{
            HIDMessageHandler.Store(HIDOpcode.INIT_ACK, HIDHandlers.INIT_ACK);

            HIDDetection.InsertEvent += HIDDetection_InsertEvent;
            HIDDetection.DetachEvent += HIDDetection_DetachEvent;
            HIDDetection.StartDeviceDetection();

            Debug.Log("~~~~TeensyHID Monitor~~~~");

			var detectedDevices = EnumerateTeensyHIDDevices();
			if (detectedDevices.Any())
            {
                Debug.Log($"Found {detectedDevices.Count} device(s).");
				AddDevices(detectedDevices);
            }
			else
			{
                Debug.Log("No devices found, will search in background...");
			}
            Console.ReadLine();
		}

        private static void HIDDetection_DetachEvent(object sender, System.Management.EventArrivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void HIDDetection_InsertEvent(object sender, System.Management.EventArrivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void AddDevices(List<HidDevice> devices)
        {
            foreach (var device in devices)
            {
                var teensyDevice = new TeensyHID(device);
                var teensyConnection = new HIDConnection(teensyDevice);
                if (!teensyConnection.IsConnected) continue;
                if (!TeensyConnections.TryAdd(teensyDevice.DevicePath, teensyConnection))
                {
                    Debug.LogError("Failed to add device.");
                    continue;
                }
                Debug.Log($"Added device: {teensyDevice}");
            }
        }

		private static List<HidDevice> EnumerateTeensyHIDDevices()
		{
			return HidDevices.Enumerate(TeensyHID.VendorId, TeensyHID.ProductId, TeensyHID.UsagePage).ToList();
        }
	}
}
