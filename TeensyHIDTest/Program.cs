using System;

namespace TeensyHIDTest
{
	internal class Program
	{
		private static readonly TeensyConnection _teensyConnection = new TeensyConnection();

		private static void Main(string[] args)
		{
			while (true)
			{
				_teensyConnection.GetDevices();
				// _teensyConnection.StartListening();
				// _teensyConnection.InitializeTeensyAsync();
				Console.ReadLine();
			}
		}
	}
}
