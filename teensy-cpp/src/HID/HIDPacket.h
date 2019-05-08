#ifndef HIDPACKET_H
#define HIDPACKET_H

#include <vector>

#include "HIDOpcode.h"

class HIDPacket {
	public:
		HIDPacket();
		HIDPacket(uint8_t rawdata[]);
		HIDPacket(HIDOpcode opcode);
		HIDPacket(HIDOpcode opcode, uint8_t newdata[]);
		HIDPacket(HIDOpcode opcode, const char* newdata) : HIDPacket(opcode, (unsigned char*)newdata) {};

		HIDPacket& operator= (const HIDPacket& p);
		operator unsigned char*();

		HIDOpcode const getOpcode();
		const char* getOpcodeName();
		char* getData();
		uint8_t* getBuffer();
	private:
		HIDOpcode opcode;
		std::vector<unsigned char> data;
};

#endif /* HIDPACKET_H */
