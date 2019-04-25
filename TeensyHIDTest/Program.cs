using System;

namespace TeensyHIDTest
{
	internal class Program
	{
		private static readonly TeensyConnection _teensyConnection = new TeensyConnection();

		private static void Main(string[] args)
		{
			_teensyConnection.StartListening();

			while (true)
			{
				if (_teensyConnection.TeensyDevice == null || !_teensyConnection.TeensyDevice.IsInitialized) continue;

				Console.ReadLine();

				var initPacket = new TeensyPacket(TeensyOpcode.HEARTBEAT, "Hello World!");

				var responsePacket = _teensyConnection.WriteThenReadAsync(initPacket);
			}
		}
	}
}
