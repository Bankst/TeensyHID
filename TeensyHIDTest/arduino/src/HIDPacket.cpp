#include "HIDPacket.h"

HIDPacket::HIDPacket() {
}

HIDPacket::~HIDPacket() {
    memset(buff, 0, sizeof(buff));
    memset(data, 0, sizeof(data));
}

HIDPacket::HIDPacket(byte rawdata[]) {
    memcpy(buff, rawdata, 64);
    opcode = (HIDOpcode) buff[0];
    memcpy(data, buff + 1, 63);
}

HIDPacket::HIDPacket(HIDOpcode opcode, byte newdata[]) {
    memset(buff, 0, sizeof(buff));
    memset(data, 0, sizeof(data));
    buff[0] = (byte) opcode;
    memcpy(data, newdata, 63);
    memcpy(buff + 1, newdata, 63);
}

HIDOpcode HIDPacket::getOpcode() {
    return opcode;
}