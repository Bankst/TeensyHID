namespace TeensyHID.Comm.Messages
{
    internal class MSG_LOOP_DATA_ACK : HIDMessage
    {
		public MSG_LOOP_DATA_ACK() : base(HIDOpcode.LOOP_DATA_ACK)
		{

		}
    }
}
