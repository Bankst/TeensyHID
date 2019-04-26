#include "HIDHandler.h"

int HIDHandler::receive(const HIDPacket& packet, int timeout)
{
	byte buff[64];
	int n = RawHID.recv(buff, timeout);
	if (n > 0)
	{
		packet = HIDPacket(buff);
	}
	return n;
}

int HIDHandler::send(const HIDPacket& packet, int timeout)
{
	return RawHID.send(reinterpret_cast<const unsigned char*>(&packet), timeout);
}