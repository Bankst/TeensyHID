#include "HIDManager.h"
#include "MSG\MSG_INIT.h"
#include "MSG\MSG_INIT_OK.h"
#include "MSG\MSG_HEARTBEAT_OK.h"

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
	switch((unsigned char)(packet.getOpcode())) {
		case HIDOpcode::INIT:
			// MSG_INIT message = packet.getData();
			Serial.println("got INIT");
			break;
	}
	packet = HIDPacket();
}