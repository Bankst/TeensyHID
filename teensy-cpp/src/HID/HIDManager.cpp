#include "HIDManager.h"

HIDManager::HIDReceive HIDManager::receive(HIDPacket& packet, int timeout)
{
	byte buff[64];
	bool valid = false;
	int n = RawHID.recv(buff, timeout);
	if (n > 0)
	{
		valid = (buff[0] < HIDOpcode::HIDOPCODE_LENGTH);	
		packet = HIDPacket(buff);
	}
	return HIDManager::HIDReceive(valid, n);
}

int HIDManager::send(HIDPacket& packet, int timeout)
{
	return RawHID.send(packet, timeout);
}

void HIDManager::handle(HIDPacket& packet) {
	switch(packet.getOpcode) {
		
	}
}