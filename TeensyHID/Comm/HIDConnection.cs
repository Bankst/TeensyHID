using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using TeensyHID.Comm.Packets;

namespace TeensyHID.Comm
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
	public class HIDConnection : Object
	{
		private const int RECEIVE_BUFFER_SIZE = 64;

        public TeensyHID Device { get; }

		public bool IsConnected => Device != null && Device.IsConnected;

		public string Path => Device?.DevicePath;

		private byte[] _receiveBuffer;

        private readonly MemoryStream _receiveStream;

		public HIDConnection(TeensyHID device)
		{
			Device = device;
			_receiveStream = new MemoryStream();
			_receiveBuffer = new byte[RECEIVE_BUFFER_SIZE];
			Device.DataReceived += DeviceOnDataReceived;

			new MSG_INIT().Send(this);
		}

		private void DeviceOnDataReceived(byte[] data)
		{
			ReceivedData(data);
			_receiveBuffer = new byte[RECEIVE_BUFFER_SIZE];
        }

		public void SendData(byte[] buffer) => Device.SendData(buffer);

		private bool TryParseMessage()
		{
			if (!IsConnected)
			{
				return false;
			}

			_receiveStream.Position = 0;

			if (_receiveStream.Length != RECEIVE_BUFFER_SIZE)
			{
				return false;
			}

			var messageBuffer = new byte[RECEIVE_BUFFER_SIZE];
			_receiveStream.Read(messageBuffer, 0, RECEIVE_BUFFER_SIZE);

			var message = new HIDMessage(messageBuffer);
			if (!HIDMessageHandler.TryFetch(message.Opcode, out var handler) || handler == null)
			{
				Debug.LogAssert("Unhandled message", message);
            }

			HIDMessageHandler.Invoke(handler, message, this);

			return true;
		}

		private void GetMessageFromBuffer(byte[] buffer)
		{
			if (!IsConnected)
			{
				return;
			}

			_receiveStream.Write(buffer, 0, buffer.Length);

			while (TryParseMessage())
			{

			}
		}
		private void ReceivedData(byte[] buffer)
		{
			if (!IsConnected)
			{
				return;
			}

			Array.Copy(_receiveBuffer, 0, buffer, 0, RECEIVE_BUFFER_SIZE);
			GetMessageFromBuffer(buffer);
		}

		protected override void Destroy()
		{
			Device?.Dispose();
			_receiveStream?.Dispose();
        }
    }
}
