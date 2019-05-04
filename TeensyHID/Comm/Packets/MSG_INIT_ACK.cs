namespace TeensyHID.Comm.Packets
{
    class MSG_INIT_ACK : HIDMessage
    {
		public MSG_INIT_ACK() : base(HIDOpcode.INIT_ACK)
		{
			
		}
    }
}
