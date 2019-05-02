#ifndef _HIDPACKET_h
#define _HIDPACKET_h

#include <array>

#include "HIDOpcode.h"

class HIDPacket {
	public:
		HIDPacket();
		HIDPacket(byte rawdata[]);
		HIDPacket(HIDOpcode opcode, byte newdata[]);
		HIDPacket(HIDOpcode opcode, const char* newdata) : HIDPacket(opcode, (unsigned char*)newdata) {};

		HIDPacket& operator= (const HIDPacket& p);
		operator unsigned char*();

		HIDOpcode const getOpcode();
		const char* getOpcodeName();
		char* getData();
		byte* getBuffer();
	private:
		HIDOpcode opcode;
		std::array<unsigned char, 63> data;
};

#endif
