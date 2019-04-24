#ifndef _HIDPACKET_h
#define _HIDPACKET_h
#pragma once

#include "HIDOpcode.h"
struct HIDPacket {
	HIDOpcode opcode;
	byte data[63];
};

#endif
