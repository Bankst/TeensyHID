#include "HIDHandler.h"

int HIDHandler::receive() {
    return 0;
}

int HIDHandler::send(HIDPacket* packet) {
    packet::getBuffer();
    int n = RawHID.send()
}