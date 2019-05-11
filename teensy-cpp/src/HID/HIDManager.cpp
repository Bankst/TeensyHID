#include "ardtrace.h"
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
	return 0;
	//return RawHID.send(packet, timeout);
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

HIDManager::HIDReceive HIDManager::receiveLarge(std::vector<unsigned char> buffer, int timeout) {
	uint8_t buff[64];
	uint8_t pktCount = 0;
	bool valid = false;

	int n = RawHID.recv(buff, timeout);

	if (n > 0) {	
		valid = buff[0] < (byte)HIDOpcode::HIDOPCODE_LENGTH;
		pktCount = buff[1];
		DUMP(pktCount);
	}

	largeAck();

	if (pktCount == 1) {
		return HIDManager::HIDReceive(valid, pktCount);
	} else if (pktCount <= 8) {
		// for (int i = 0; i < pktCount - 1; i++) {			
		// 	DUMP(i);
		// 	bool got;
		// 	while (!got)  {
		// 		n = RawHID.recv(buff, timeout + 150);	
		// 		got = n > 0;
		// 	}
		// 	if (got && i != pktCount) largeAck();
		// 	DUMP(buff[1]);
		// }
	}

	// uint8_t fullBuff[pktCount * 64];
	// for (uint8_t i = 0; i < pktCount; i++) {
	// 	uint8_t offset = pktCount * 63;

	// 	n = RawHID.recv(buff, timeout);
	// 	if (n > 0) {
	// 		DUMP(buff[0])
	// 		std::copy(std::begin(buff), buff + 64, fullBuff + offset);
	// 	}
	// }

	return HIDManager::HIDReceive(valid, pktCount);
}

void HIDManager::largeAck() {
	uint8_t ack[1] = {(uint8_t)HIDOpcode::MESSAGE_ACK};
	RawHID.send(ack, 0);
}