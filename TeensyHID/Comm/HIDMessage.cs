using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace TeensyHID.Comm
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class HIDMessage : Object
    {
        public HIDOpcode Opcode { get; }

        private readonly bool _largeMessage;

        private const ushort HeaderByteCount = 2;
        public const ushort MaxSmallMessageLength = 64;
        public const ushort MaxLargeMessageLength = 512;

        private readonly BinaryReader _reader;

        private readonly MemoryStream _stream;

        private readonly BinaryWriter _writer;

        public HIDMessage(byte[] buffer)
        {
            _largeMessage = buffer.Length > MaxSmallMessageLength;
            _stream = new MemoryStream(buffer);
            _reader = new BinaryReader(_stream);
            Opcode = (HIDOpcode)ReadByte();
        }

        public HIDMessage(HIDOpcode opcode, bool largeMessage = false)
        {
            _largeMessage = largeMessage;
            _stream = new MemoryStream(largeMessage ? MaxLargeMessageLength : MaxSmallMessageLength);

            _writer = new BinaryWriter(_stream);
            Opcode = opcode;

            Write((byte)opcode);
            Write((byte)0); // placeholder for packetCount byte
        }

        public int RemainingSpace() => _stream.Capacity - (int)_stream.Length;

        /// <summary>
        /// Fills the number of bytes with the value.
        /// </summary>
        /// <param name="count">The number of bytes to fill.</param>
        /// <param name="value">The value to fill with.</param>
        public void Fill(int count, byte value)
        {
            count = DataLengthClip(count);
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
            return _reader.ReadBoolean();
        }

        /// <summary>
        /// Reads a byte value from the current stream.
        /// </summary>
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        /// <summary>
        /// Reads an array of bytes from the current stream.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        /// <summary>
        /// Reads a character value from the current stream.
        /// </summary>
        public char ReadChar()
        {
            return _reader.ReadChar();
        }

        /// <summary>
        /// Reads a decimal value from the current stream.
        /// </summary>
        public decimal ReadDecimal()
        {
            return _reader.ReadDecimal();
        }

        /// <summary>
        /// Reads a double value from the current stream.
        /// </summary>
        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        /// <summary>
        /// Reads a 16-bit integer value from the current stream.
        /// </summary>
        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        /// <summary>
        /// Reads a 32-bit integer value from the current stream.
        /// </summary>
        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        /// <summary>
        /// Reads a 64-bit integer value from the current stream.
        /// </summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            return _reader.ReadInt64();
        }

        /// <summary>
        /// Reads a float value from the current stream.
        /// </summary>
        /// <returns></returns>
        public float ReadSingle()
        {
            return _reader.ReadSingle();
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
            length = DataLengthClip(length);
            var ret = string.Empty;
            var buffer = new byte[length];
            var count = 0;

            _stream.Read(buffer, 0, buffer.Length);

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
            return _reader.ReadUInt16();
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer value from the current stream.
        /// </summary>
        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        /// <summary>
        /// Reads an unsigned 64-bit integer value from the current stream.
        /// </summary>
        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        /// <summary>
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
            var buffer = _stream.ToArray();
            Array.Resize(ref buffer, MaxLargeMessageLength);

            var dataLength = buffer.Length - HeaderByteCount;

            // forces rounding-up of the count
            var pktCount = (byte) ((buffer.Length + MaxSmallMessageLength - 1) / MaxSmallMessageLength);

            var dataBytes = buffer.Skip(HeaderByteCount).Take(dataLength).ToArray();
            Array.Resize(ref dataBytes, DataLengthClip(dataLength));

            var ret = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, ret, HeaderByteCount, buffer.Length - HeaderByteCount);
            
            buffer[1] = pktCount;

            return buffer;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns a string representing the message.
        /// </summary>
        public override string ToString()
        {
            return $"Command=0x{Opcode:X} ({Opcode}), Length={_stream.Length}";
        }

        /// <summary>
        /// Writes a boolean value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(bool value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a byte value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(byte value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a signed byte value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(sbyte value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a byte array to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(byte[] value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a 16-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(short value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a 32-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(int value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a 64-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(long value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 16-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(ushort value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(uint value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes an unsigned 64-bit integer value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(ulong value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a double value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(double value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a decimal value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(decimal value)
        {
            _writer.Write(value);
        }

        /// <summary>
        /// Writes a float value to the current stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void Write(float value)
        {
            _writer.Write(value);
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
            length = DataLengthClip(length);
            var buffer = Encoding.ASCII.GetBytes(value);

            Write(buffer);

            for (var i = 0; i < length - buffer.Length; i++)
            {
                Write((byte)0);
            }
        }

        private int DataLengthClip(int length)
        {
            var maxDataLength = (_largeMessage ? MaxLargeMessageLength : MaxSmallMessageLength);
            return length > maxDataLength ? maxDataLength : length;
        }

        protected override void Destroy()
        {
            // Reader and writer are not always initialized, so we need to check
            // for null before attempting to close them.
            _reader?.Close();
            _writer?.Close();
            _stream.Close();
        }
    }
}
