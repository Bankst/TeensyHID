using System;
using System.Threading;

namespace TeensyHID.HID
{
	internal class HidDeviceEventMonitor
	{
		public event InsertedEventHandler Inserted;
		public event RemovedEventHandler  Removed;

		public delegate void InsertedEventHandler(HidDevice device);
		public delegate void RemovedEventHandler(HidDevice device);

		private readonly HidDevice _device;
		private          bool      _wasConnected;

		public HidDeviceEventMonitor(HidDevice device)
		{
			_device = device;
		}

		public void Init()
		{
			var eventMonitor = new Action(DeviceEventMonitor);
			eventMonitor.BeginInvoke(DisposeDeviceEventMonitor, eventMonitor);
		}

		private void DeviceEventMonitor()
		{
			var isConnected = _device.IsConnected;

			if (isConnected != _wasConnected)
			{
				if (isConnected && Inserted != null) Inserted(_device);
				else if (!isConnected) Removed?.Invoke(_device);
				_wasConnected = isConnected;
			}
			//TODO: fix this trash
			Thread.Sleep(500);

			if (_device.MonitorDeviceEvents) Init();
		}

		private static void DisposeDeviceEventMonitor(IAsyncResult ar)
		{
			((Action)ar.AsyncState).EndInvoke(ar);
		}
	}
}
