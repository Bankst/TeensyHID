#ifndef HIDHANDLERS_H
#define HIDHANDLERS_H

#include "HIDOpcode.h"
#include "HIDHandler.h"

namespace Handlers {
    class HANDLER_INIT : public HIDHandler {
        void run() {
            Serial.println("HANDLER_INIT");
        }
    };

    class HANDLER_HEARTBEAT : public HIDHandler {
        void run() {
            Serial.println("HANDLER_HEARTBEAT");
        }
    };
}

#endif /* HIDHANDLERS_H */
