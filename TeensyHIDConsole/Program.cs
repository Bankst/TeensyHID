using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TeensyHID.Comm;
using TeensyHID.HID;
using TeensyHID.Util;

namespace TeensyHID
{
	[SuppressMessage("ReSharper", "FunctionNeverReturns")]
	internal class Program
	{
		private static readonly ConcurrentDictionary<string, HIDConnection> TeensyConnections = new ConcurrentDictionary<string, HIDConnection>();

		private static void Main()
		{
            HIDMessageHandler.Store(HIDOpcode.INIT_ACK, HIDHandlers.INIT_ACK);

            Debug.Log("+===========TeensyHID Monitor===========+");
			Debug.Log("+============Press Q to Exit============+");
			Console.WriteLine();

			var detectedDevices = EnumerateTeensyHIDDevices();
			if (detectedDevices.Any())
            {
                Debug.Log($"Found {detectedDevices.Count} device(s).");
				AddDevices(detectedDevices);
            }
			else
			{
                Debug.Log("No devices found, will search in background...");
				// TODO: this doesnt actually work lmao
			}

			// Only sub to events after initial scan
			Comm.TeensyHID.Inserted += TeensyHID_Inserted;
			Comm.TeensyHID.Removed += TeensyHID_Removed;

			// keep it goin...
			while (true)
			{
				if (!Console.KeyAvailable) continue;

				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Q)
				{
					Environment.Exit(0);
				}
			}
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
			var newConnection = new HIDConnection(new Comm.TeensyHID(device));
			if (TeensyConnections.TryAdd(addedSerial, newConnection))
			{
				Debug.Log($"Added device: {device}");
			}
		}

		private static void AddDevice(HidDevice device)
		{
			var teensyHid = new Comm.TeensyHID(device);
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
			return HidDevices.Enumerate(Comm.TeensyHID.VendorId, Comm.TeensyHID.ProductId, Comm.TeensyHID.UsagePage).ToList();
        }
	}
}
