namespace TeensyHID.Comm.Packets
{
    internal class MSG_LOOP_DATA : HIDMessage
    {
		public MSG_LOOP_DATA() : base(HIDOpcode.LOOP_DATA, true)
        {
            Fill(62, 0x01);
            Fill(64, 0x02);
            Fill(64, 0x03);
            Fill(64, 0x04);
            Fill(64, 0x05);
            Fill(64, 0x06);
            Fill(64, 0x07);
            Fill(64, 0x08);
            // this should give us 125 bytes, then add the 3 bytes for opcode and message length, and we're at 128. exactly 2 packets.
        }
	}
}
