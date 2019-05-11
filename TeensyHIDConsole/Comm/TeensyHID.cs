using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TeensyHID.HID;
using TeensyHID.Util;

namespace TeensyHID.Comm
{
	public class TeensyHID : IDisposable
	{
		public const int VendorId = 0x16C0;
		public const int ProductId = 0x0486;
		public const ushort UsagePage = 0xFFAB;
		public const ushort Usage = 0x0200;
		
		// these events are device-independent
		public static event InsertedEventHandler Inserted;
		public static event RemovedEventHandler Removed;

		// this event is per-device
        public event DataReceivedEventHandler DataReceived;

		public delegate void InsertedEventHandler(HidDevice device);
		public delegate void RemovedEventHandler(HidDevice device);
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
            var buffers = new List<byte[]>();
            // forces rounding-up of the count
            var pktCount = (buffer.Length + HIDMessage.MaxSmallMessageLength - 1)/ HIDMessage.MaxSmallMessageLength;

            for (var i = 0; i < pktCount; i++)
            {
                var splitBuf = buffer.Skip(i * HIDMessage.MaxSmallMessageLength).Take(HIDMessage.MaxSmallMessageLength).ToArray();
                buffers.Add(splitBuf);
            }

			for (var i = 0; i < pktCount; i++)
			{
				var report = _teensyHID.CreateReport();
				report.ReportId = 0x00;
				report.Data = buffers[i];

				var sent = _teensyHID.WriteReport(report);
				Debug.Log($"Packet {i + 1} of {pktCount}{(sent ? "" : " not")} sent.");
				Thread.Sleep(250);
			}

            /*

			var goodPacketTx = 0;
			var done = false;
			const bool requireAck = false;

			while (!done)
			{
				for (var i = 0; i < pktCount; i++)
				{
					var report = _teensyHID.CreateReport();
					report.ReportId = 0x00;
					report.Data = buffers[i];

					var sent = _teensyHID.WriteReport(report);
					Debug.Log(sent ? $"Sent Packet {i+1} of {pktCount}." : "NotSent");

					if (requireAck)
					{
						// blocking to wait for message ack
						var gotAck = false;
						while (!gotAck)
						{
							report = _teensyHID.ReadReport(15);
							gotAck = new HIDMessage(report.Data).Opcode == HIDOpcode.MESSAGE_ACK;
						}
						Debug.Log($"Got Ack {i + 1} of {pktCount}.");
					}
					
					goodPacketTx = i + 1;
					Thread.Sleep(15);
				}
				done = goodPacketTx >= 8;
			}
			*/
        }

        private static void TeensyHID_Inserted(IHidDevice hidDevice)
		{
			Inserted?.Invoke((HidDevice)hidDevice);
		}

		private static void TeensyHID_Removed(IHidDevice hidDevice)
		{
			Removed?.Invoke((HidDevice)hidDevice);
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
