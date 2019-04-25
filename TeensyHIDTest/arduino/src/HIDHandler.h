#include "HIDPacket.h"
#include "HIDOpcode.h"

#include <Arduino.h>

class HIDHandler {
    public:
        int receive();
        int send(HIDPacket* packet);
};