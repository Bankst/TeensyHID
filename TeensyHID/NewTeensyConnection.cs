using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeensyHID
{
	class NewTeensyConnection
	{
		private HidUtility hidUtil;
		private ushort vid;
		private ushort pid;

		private byte lastCommand = 0x00;
		private bool waitingForDevice;
		private DateTime connectedTimestamp = DateTime.Now;
		private uint txCount;
		private uint rxCount;

		public NewTeensyConnection(ushort vid, ushort pid)
		{

		}
	}
}
