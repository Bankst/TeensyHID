#ifndef _HIDPACKET_h
#define _HIDPACKET_h

#include "HIDOpcode.h"

class HIDPacket {
	public:
		HIDPacket();
		~HIDPacket();
		HIDPacket(byte rawdata[]);
		HIDPacket(HIDOpcode opcode, byte data[]);
		HIDOpcode getOpcode();
	private:
		HIDOpcode opcode;
		byte data[63];
		byte buff[64];
};

#endif
