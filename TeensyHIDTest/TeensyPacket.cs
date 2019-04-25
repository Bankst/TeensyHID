using System;
using System.Text;

namespace TeensyHIDTest
{
	public class TeensyPacket
	{
		private readonly byte[] _dataBytes = new byte[63];
		public bool IsRx { get; }

		public TeensyOpcode Opcode { get; }

		public TeensyPacket(TeensyOpcode opcode, byte[] data)
		{
			Opcode = opcode;
			_dataBytes = data;
			IsRx = false;

			ByteArray[0] = (byte) Opcode; // set first byte to opcode
			Array.Resize(ref _dataBytes, 63); // ensure data is only 63 bytes

			_dataBytes.CopyTo(ByteArray, 1); // copy data to buffer
		}

		public TeensyPacket(TeensyOpcode opcode, string data) : this(opcode, Encoding.ASCII.GetBytes(data)) { }

		public TeensyPacket(byte[] rawData)
		{
			IsRx = true;
			if (rawData.Length != 64)
			{
				return;
			}

			Opcode = (TeensyOpcode) rawData[0];
			Buffer.BlockCopy(rawData, 1, _dataBytes, 0, 63);
		}

		public byte[] RawData => _dataBytes;

		public string DataString => Encoding.ASCII.GetString(RawData);

		public byte[] ByteArray { get; } = new byte[64];

		public override string ToString()
		{
			var rxStr = IsRx ? "RX" : "TX";
			return $"TeensyHID {rxStr} - Opcode: {Opcode}, Data: {DataString}";
		}
	}
}
