using System;

namespace TeensyHID.Comm.Packets
{
    internal class MSG_INIT : HIDMessage
    {
		// Sends current time in Unix Epoch milliseconds
		public MSG_INIT() : base(HIDOpcode.INIT)
		{
			var unixEpoch = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            Write(unixEpoch);
		}
	}
}
