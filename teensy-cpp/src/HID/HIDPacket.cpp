#include <array>
#include <iterator>

#include "HIDPacket.h"

HIDPacket::HIDPacket() {
}

HIDPacket& HIDPacket::operator= (const HIDPacket& p) {
	opcode = p.opcode;
	data = p.data;
	return *this;
}

HIDPacket::HIDPacket(byte rawdata[]) {
	opcode =  static_cast<HIDOpcode>(rawdata[0]);
	std::copy(rawdata, rawdata + data.size(), data.begin());
}

HIDPacket::HIDPacket(HIDOpcode opcode, unsigned char newdata[]) {
	this->opcode = opcode;
	std::copy(newdata, newdata + data.size(), data.begin());
}

HIDOpcode const HIDPacket::getOpcode() {
	return opcode;
}

byte* HIDPacket::getBuffer() {
	return 0;
}

const char* HIDPacket::getOpcodeName() {
	return HIDOpcodeNames[static_cast<byte>(opcode)];
}

char* HIDPacket::getData() {
	return reinterpret_cast<char*>(&data);
}

HIDPacket::operator unsigned char*() {
	unsigned char tempData[64];
	tempData[0] = (unsigned char)opcode;
	std::copy(std::begin(data), std::end(data), std::begin(tempData));
	return (unsigned char*)tempData;
}