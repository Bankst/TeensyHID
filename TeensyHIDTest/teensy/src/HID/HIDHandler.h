#include "HIDPacket.h"
#include "HIDOpcode.h"

#include <Arduino.h>

class HIDHandler
{
  public:
	static int receive(const HIDPacket& packet, int timeout);
	static int send(const HIDPacket& packet, int timeout);
};