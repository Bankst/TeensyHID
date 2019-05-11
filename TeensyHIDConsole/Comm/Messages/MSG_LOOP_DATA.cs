namespace TeensyHID.Comm.Messages
{
    internal class MSG_LOOP_DATA : HIDMessage
    {
		public MSG_LOOP_DATA() : base(HIDOpcode.LOOP_DATA, true)
        {
			// ensures we leave room for the header bytes (ALTHOUGH, this shouldn't matter because we shift it all over anyways)...
			// TODO: test that this is necessary
            Fill(MaxSmallMessageLength - HeaderByteCount, 0x01);
            Fill(64, 0x02);
            Fill(64, 0x03);
            Fill(64, 0x04);
            Fill(64, 0x05);
            Fill(64, 0x06);
            Fill(64, 0x07);
            Fill(64, 0x08);
			// test fill to ensure we stop at 512 bytes
			Fill(64, 0x09);
        }
	}
}
