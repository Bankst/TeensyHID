#ifndef HIDMANAGER_H
#define HIDMANAGER_H

#include "HIDPacket.h"
#include "HIDOpcode.h"

#include <Arduino.h>
#include <vector>

class HIDManager
{
  public:
		struct HIDReceive {
			HIDReceive(bool valid, int len) {this->valid = valid; this->len	= len;}
			bool valid;
			int len;
		};
		static HIDManager::HIDReceive receive(HIDPacket& packet, int timeout);
		static HIDManager::HIDReceive receiveLarge(std::vector<unsigned char> buffer, int timeout);
		static int send(HIDPacket& packet, int timeout);
		static void handle(HIDPacket& packet);
	private:
		HIDManager() {}
		static void largeAck();
};


#endif /* HIDMANAGER_H */
