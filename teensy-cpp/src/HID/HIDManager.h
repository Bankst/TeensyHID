#include "HIDPacket.h"
#include "HIDOpcode.h"

#include <Arduino.h>

class HIDManager
{
  public:
		struct HIDReceive {
			HIDReceive(bool valid, int len) {this->valid = valid; this->len	= len;}
			bool valid;
			int len;
		};
		static HIDManager::HIDReceive receive(HIDPacket& packet, int timeout);
		static int send(HIDPacket& packet, int timeout);
		static void handle(HIDPacket& packet);
	private:
		HIDManager() {}
};