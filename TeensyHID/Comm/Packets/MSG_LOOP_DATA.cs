namespace TeensyHID.Comm.Packets
{
    internal class MSG_LOOP_DATA : HIDMessage
    {
		public MSG_LOOP_DATA() : base(HIDOpcode.LOOP_DATA)
		{

		}
	}
}
