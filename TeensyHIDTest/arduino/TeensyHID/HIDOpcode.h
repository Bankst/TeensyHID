#ifndef _HIDOPCODE_h
#define _HIDOPCODE_h
#pragma once

const char *HIDOpcodeNames[] = {"OK", "FAIL", "INIT", "INIT_OK", "HEARTBEAT", "HEARTBEAT_OK" };

enum class HIDOpcode : byte {
	OK,
	FAIL,
	INIT,
	INIT_OK,
	HEARTBEAT,
	HEARTBEAT_OK 
};

#endif
