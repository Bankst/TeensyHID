using System;

namespace TeensyHID.Comm
{
    internal static class HIDHandlers
    {
		internal static void INIT_OK(HIDMessage message, HIDConnection connection)
		{
			Console.WriteLine("GOTEM");
		}
    }
}
