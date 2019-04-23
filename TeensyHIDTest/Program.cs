using System;

namespace TeensyHIDTest
{
	class Program
	{
		private static TeensyConnection _teensyConnection = new TeensyConnection();

		static void Main(string[] args)
		{
			while (true)
			{
				_teensyConnection.InitializeTeensyAsync();
				Console.ReadLine();
			}
		}
	}
}
