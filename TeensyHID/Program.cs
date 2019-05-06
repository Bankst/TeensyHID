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

			// Only sub to events after initial scan
			TeensyHID.Inserted += TeensyHID_Inserted;
			TeensyHID.Removed += TeensyHID_Removed;

            Console.ReadLine();
		}

        private static void TeensyHID_Removed(HidDevice device)
		{
			var removedSerial = device.Serial;
			if (TeensyConnections.TryRemove(removedSerial, out var unused))
			{
				Debug.Log($"Device disconnected: {device}");
			}
		}

        private static void TeensyHID_Inserted(HidDevice device)
		{
			var addedSerial = device.Serial;
			var newConnection = new HIDConnection(new TeensyHID(device));
			if (TeensyConnections.TryAdd(addedSerial, newConnection))
			{
				Debug.Log($"Added device: {device}");
			}
		}

		private static void AddDevice(HidDevice device)
		{
			var teensyHid = new TeensyHID(device);
			var teensyConnection = new HIDConnection(teensyHid);

			if (!teensyConnection.IsConnected) return;
			if (TeensyConnections.ContainsKey(teensyHid.Serial)) return;

			if (!TeensyConnections.TryAdd(teensyHid.Serial, teensyConnection))
			{
				Debug.LogError($"Failed to add device: {teensyHid}");
            }
			Debug.Log($"Added device: {teensyHid}");
		}

        private static void AddDevices(List<HidDevice> devices)
        {
            foreach (var device in devices)
            {
                AddDevice(device);
            }
        }

		private static List<HidDevice> EnumerateTeensyHIDDevices()
		{
			return HidDevices.Enumerate(TeensyHID.VendorId, TeensyHID.ProductId, TeensyHID.UsagePage).ToList();
        }
	}
}
