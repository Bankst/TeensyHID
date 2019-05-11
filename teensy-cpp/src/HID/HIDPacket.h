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
		HIDPacket(HIDOpcode opcode, const char* newdata) : HIDPacket(opcode, (uint8_t*)newdata) {};

		static const uint8_t PacketLength = 64;

		HIDPacket& operator= (const HIDPacket& p);
		operator uint8_t*();

		HIDOpcode const getOpcode();
		const char* getOpcodeName();
		char* getData();
		uint8_t* getBuffer();
	private:
		HIDOpcode opcode;
		uint8_t packetCount;
		std::vector<uint8_t> data;
};

#endif /* HIDPACKET_H */
