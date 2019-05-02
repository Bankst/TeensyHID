#include "HIDHandler.h"

HIDHandler::HIDReceive HIDHandler::receive(HIDPacket& packet, int timeout)
{
	byte buff[64];
	bool valid = false;
	int n = RawHID.recv(buff, timeout);
	if (n > 0)
	{
		valid = (buff[0] < HIDOpcode::HIDOPCODE_LENGTH);	
		packet = HIDPacket(buff);
	}
	return HIDHandler::HIDReceive(valid, n);
}

int HIDHandler::send(HIDPacket& packet, int timeout)
{
	return RawHID.send(packet, timeout);
}