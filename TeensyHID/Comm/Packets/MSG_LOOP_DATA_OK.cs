namespace TeensyHID.Comm.Packets
{
    internal class MSG_LOOP_DATA_OK : HIDMessage
    {
		public MSG_LOOP_DATA_OK() : base(HIDOpcode.LOOP_DATA_OK)
		{

		}
    }
}
