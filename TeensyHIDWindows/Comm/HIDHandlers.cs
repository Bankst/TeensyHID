﻿using System;

namespace TeensyHIDWindows.Comm
{
    internal static class HIDHandlers
    {
		internal static void INIT_ACK(HIDMessage message, HIDConnection connection)
		{
			Console.WriteLine("GOTEM");
		}
    }
}
