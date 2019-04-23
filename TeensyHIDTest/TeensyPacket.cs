using System;
using System.Text;

namespace TeensyHIDTest
{
	public class TeensyPacket
	{
		private readonly byte[] _buffer = new byte[64];
		private byte[] _dataBytes = new byte[63];

		public TeensyOpcode Opcode { get; }

		public TeensyPacket(TeensyOpcode opcode, byte[] data)
		{
			Opcode = opcode;
			_dataBytes = data;

			_buffer[0] = (byte) Opcode; // set first byte to opcode
			Array.Resize(ref _dataBytes, 63); // ensure data is only 63 bytes

			_buffer.CopyTo(_dataBytes, 1); // copy data to buffer
		}

		public TeensyPacket(TeensyOpcode opcode, string data) : this(opcode, Encoding.ASCII.GetBytes(data)) { }

		public TeensyPacket(byte[] rawData)
		{
			if (rawData.Length != 64)
			{
				return;
			}

			Opcode = (TeensyOpcode) rawData[0];
			Buffer.BlockCopy(rawData, 1, _dataBytes, 0, 63);
		}


		public byte[] GetByteArray() => _buffer;
	}
}
