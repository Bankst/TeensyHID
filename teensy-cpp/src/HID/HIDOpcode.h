#ifndef _HIDOPCODE_h
#define _HIDOPCODE_h

#include <Arduino.h>

static const char* const HIDOpcodeNames[] = {"OK", "FAIL", "INIT", "INIT_OK", "HEARTBEAT", "HEARTBEAT_OK" };

enum HIDOpcode : byte {
	OK,
	FAIL,
	INIT,
	INIT_OK,
	HEARTBEAT,
	HEARTBEAT_OK, 
	HIDOPCODE_LENGTH
};

#endif
