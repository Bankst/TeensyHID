#include <array>
#include <iterator>
#include <vector>

#include "HIDPacket.h"

HIDPacket::HIDPacket() {
}

HIDPacket& HIDPacket::operator= (const HIDPacket& p) {
	opcode = p.opcode;
	data = p.data;
	return *this;
}

HIDPacket::HIDPacket(uint8_t rawdata[]) {
	opcode = static_cast<HIDOpcode>(rawdata[0]);
	data.assign(rawdata + 2, rawdata + 62);
}

HIDPacket::HIDPacket(HIDOpcode opcode, uint8_t newdata[]) {
	this->opcode = opcode;
	data.assign(newdata, newdata + sizeof(newdata));
}

HIDOpcode const HIDPacket::getOpcode() {
	return opcode;
}

uint8_t* HIDPacket::getBuffer() {
	return 0;
}

const char* HIDPacket::getOpcodeName() {
	return HIDOpcodeNames[static_cast<uint8_t>(opcode)];
}

char* HIDPacket::getData() {
	return reinterpret_cast<char*>(&data);
}

HIDPacket::operator uint8_t*() {
	uint8_t tempData[64];
	tempData[0] = (uint8_t)opcode;
	tempData[1] = 1;
	std::copy(std::begin(data), std::end(data), std::begin(tempData));
	return (uint8_t*)tempData;
}