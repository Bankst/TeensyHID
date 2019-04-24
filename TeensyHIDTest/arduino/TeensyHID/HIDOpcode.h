#ifndef _HIDOPCODE_h
#define _HIDOPCODE_h
#pragma once

enum class HIDOpcode : byte {
	OK,
	FAIL,
	INIT,
	INIT_OK,
	HEARTBEAT,
	HEARTBEAT_OK 
};

#endif
