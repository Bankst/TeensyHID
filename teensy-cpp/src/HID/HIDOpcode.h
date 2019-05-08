#ifndef HIDOPCODE_H
#define HIDOPCODE_H

static const char* const HIDOpcodeNames[] = {"NULL", "INIT", "INIT_OK", "HEARTBEAT", "HEARTBEAT_OK" };

enum HIDOpcode : uint8_t {
	NULL_,
	MESSAGE_ACK,
	INIT,
	INIT_OK,
	HEARTBEAT,
	HEARTBEAT_OK, 	
	HIDOPCODE_LENGTH
};

#endif /* HIDOPCODE_H */
