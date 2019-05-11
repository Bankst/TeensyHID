using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TeensyHIDWindows.Hid;
using TeensyHIDWindows.Util;

namespace TeensyHIDWindows.Comm
{
    public class HIDHost
    {
		private static readonly ConcurrentDictionary<string, HIDConnection> ConnectedDevices = new ConcurrentDictionary<string, HIDConnection>();

		public readonly int VendorId;
		public readonly int ProductId;
		public readonly ushort UsagePage;

		public HIDHost(int vendorId, int productId, ushort usagePage)
		{
			VendorId = vendorId;
			ProductId = productId;
			UsagePage = usagePage;
		}

		private IEnumerable<HidDevice> GetAvailableDevices()
		{
			return HidDevices.Enumerate(VendorId, ProductId, UsagePage);
		}

		private static HIDConnectionAttempt ConnectToDevice(HidDevice device)
		{
			var retval = new HIDConnectionAttempt(device.DevicePath);
			var teensyHid = new TeensyHID(device);
			var teensyConnection = new HIDConnection(teensyHid);

			if (!teensyConnection.IsConnected)
			{
				retval.Result = HIDConnectionResult.DEVICE_NO_LONGER_CONNECTED;
				return retval;
			}

			if (ConnectedDevices.ContainsKey(teensyHid.DevicePath))
			{
				retval.Result = HIDConnectionResult.DEVICE_ALREADY_CONNECTED;
				return retval;
			}

			if (!ConnectedDevices.TryAdd(teensyHid.DevicePath, teensyConnection))
			{
				retval.Result = HIDConnectionResult.FAILED_TO_ADD;
				return retval;
			}

			retval.Result = HIDConnectionResult.OK;
			return retval;
		}

		private static List<HIDConnectionAttempt> ConnectToDevices(IEnumerable<HidDevice> devices)
		{
			return devices.Select(ConnectToDevice).ToList();
		}
    }
}
