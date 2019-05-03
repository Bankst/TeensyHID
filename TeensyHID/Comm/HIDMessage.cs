using System;
using System.IO;
using System.Text;

namespace TeensyHID.Comm
{
    public class HIDMessage : Object
    {
        public HIDOpcode Opcode { get; }

        private readonly BinaryReader reader;

        private readonly MemoryStream stream;

        private readonly BinaryWriter writer;

        public HIDMessage(byte[] buffer)
        {
            stream = new MemoryStream(buffer);
            reader = new BinaryReader(stream);
            Opcode = (HIDOpcode)ReadByte();
        }

        public HIDMessage(HIDOpcode opcode)
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
            Opcode = opcode;

            Write((byte)opcode);
        }

        /// <summary>
        /// Fills the number of bytes with the value.
        /// </summary>
        /// <param name="count">The number of bytes to fill.</param>
        /// <param name="value">The value to fill with.</param>
        public void Fill(int count, byte value)
        {
            if (count > 63) count = 63;
            for (var i = 0; i < count; i++)
            {
                Write(value);
            }
        }

        /// <summary>
        /// Reads a boolean value from the current stream.
        /// </summary>
        public bool ReadBoolean()
        {
            return reader.ReadBoolean();
        }

        /// <summary>
        /// Reads a byte value from the current stream.
        /// </summary>
        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Reads an array of bytes from the current stream.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        /// <summary>
        /// Reads a character value from the current stream.
        /// </summary>
        public char ReadChar()
        {
            return reader.ReadChar();
        }

        /// <summary>
        /// Reads a decimal value from the current stream.
        /// </summary>
        public decimal ReadDecimal()
        {
            return reader.ReadDecimal();
        }

        /// <summary>
        /// Reads a double value from the current stream.
        /// </summary>
        public double ReadDouble()
        {
            return reader.ReadDouble();
        }

        /// <summary>
        /// Reads a 16-bit integer value from the current stream.
        /// </summary>
        public short ReadInt16()
        {
            return reader.ReadInt16();
        }

        /// <summary>
        /// Reads a 32-bit integer value from the current stream.
        /// </summary>
        public int ReadInt32()
        {
            return reader.ReadInt32();
        }

        /// <summary>
        /// Reads a 64-bit integer value from the current stream.
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            return reader.ReadInt64();
        }

        /// <summary>
        /// Reads a float value from the current stream.
        /// </summary>
        /// <returns></returns>
        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        /// <summary>
        /// Reads a string value from the current stream.
        /// </summary>
        public string ReadString()
        {
            return ReadString(ReadByte());
        }

        /// <summary>
        /// Reads a string value from the current stream.
        /// </summary>
        /// <param name="length">The length of the stream.</param>
        public string ReadString(int length)
        {
            if (length > 63) length = 63;
            var ret = string.Empty;
            var buffer = new byte[length];
            var count = 0;

            stream.Read(buffer, 0, buffer.Length);

            if (buffer[length - 1] != 0)
            {
                count = length;
            }
            else
            {
                while (buffer[count] != 0 && count < length)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                ret = Encoding.ASCII.GetString(buffer, 0, count);
            }

            return ret;
        }

        /// <summary>
        /// Reads an unsigned 16-bit integer value from the current stream.
        /// </summary>
        public ushort ReadUInt16()
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer value from the current stream.
        /// </summary>
        public uint ReadUInt32()
        {
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Reads an unsigned 64-bit integer value from the current stream.
        /// </summary>
        public ulong ReadUInt64()
        {
            return reader.ReadUInt64();
        }

		// <summary>
		/// Sends the message to the connection.
		/// </summary>
		/// <param name="connection">The connection to send the message to.</param>
		public void Send(HIDConnection connection)
		{
			connection?.SendData(ToArray(connection));
			Destroy(this);
		}

		/// <summary>
		/// Returns a byte array representing the message.
		/// </summary>
		/// <returns></returns>
		public byte[] ToArray(HIDConnection connection = null)
		{
			byte[] ret;
			var buffer = stream.ToArray();

			Array.Resize(ref buffer, 63);

			ret = new byte[buffer.Length + 1];

			Buffer.BlockCopy(buffer, 0, ret, 1, buffer.Length);
			ret[0] = (byte) Opcode;

			return ret;
		}

        /// <summary>
        /// Returns a string representing the message.
        /// </summary>
        public override string ToString()
        {
            return $"Command=0x{Opcode:X} ({Opcode}), Length={stream.Length - 1}";
        }

        /// <summary>
        /// Writes a boolean value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(bool value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a byte value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(byte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a signed byte value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(sbyte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a byte array to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(byte[] value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a 16-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(short value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a 32-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(int value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a 64-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(long value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 16-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(ushort value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(uint value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 64-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(ulong value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a double value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(double value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a decimal value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(decimal value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a float value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(float value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes a string value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(string value)
        {
            Write(value, value.Length);
        }

        /// <summary>
        /// Writes a string value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="length">The length of the string.</param>
        public void Write(string value, int length)
        {
            if (length > 63) length = 63;
            var buffer = Encoding.ASCII.GetBytes(value);

            Write(buffer);

            for (var i = 0; i < length - buffer.Length; i++)
            {
                Write((byte)0);
            }
        }

		protected override void Destroy()
		{
			// Reader and writer are not always initialized, so we need to check
			// for null before attempting to close them.
			reader?.Close();
			writer?.Close();
			stream.Close();
        }
    }
}
