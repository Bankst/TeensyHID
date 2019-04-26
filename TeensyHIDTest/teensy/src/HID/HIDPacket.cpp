#include <array>

#include "HIDPacket.h"

HIDPacket::HIDPacket() {
}

HIDPacket::~HIDPacket() {
}

HIDPacket::HIDPacket(const HIDPacket& p) {
	opcode = p.opcode;
	std::copy(std::begin(p.data), std::end(p.data), std::begin(data));
}

HIDPacket::HIDPacket(byte rawdata[]) {
	opcode = (HIDOpcode) rawdata[0];
	std::copy(rawdata + 1, rawdata + data.size(), data.begin());
}

HIDPacket::HIDPacket(HIDOpcode opcode, unsigned char newdata[]) {
	this->opcode = opcode;
	std::copy(newdata + 1, newdata + data.size(), data.begin());
}

HIDOpcode HIDPacket::getOpcode() {
	return opcode;
}

byte* HIDPacket::getBuffer() {
	return 0;
}

const char* HIDPacket::getOpcodeName() {
	return HIDOpcodeNames[(int)opcode];
}

char* HIDPacket::getData() {
	return reinterpret_cast<char*>(&data);
}