#include "TeensyThreads.h"

#include "HIDPacket.h"
#include "HIDOpcode.h"

#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 20
#define RX_LOOP_MS 40

HIDPacket latestPacket;

volatile bool isRx = false;
volatile bool shouldTx = false;

void hidThread() {
	while(1) {
		byte buff[64];
		int n;
		n = RawHID.recv(buff, 0); // 0 timeout, do not wait
		if (n > 0) {
      isRx = true;
			Serial.print(F("Received packet, first byte: "));
			Serial.println((int)buff[0]);
			latestPacket.opcode = (HIDOpcode)(int)buff[0];
			memcpy(buff, latestPacket.data, 1);\
      shouldTx = true;
		}
		if (shouldTx) {
      buff[0] = (byte)HIDOpcode::OK;
		}
		threads.delay(RX_LOOP_MS);
		threads.yield();
	}
}

void statusThread() {
	while(1) {
		if (!isRx){
			digitalWrite(LED_BUILTIN, HIGH);
			threads.delay(STATUS_BLINK_MS);
			digitalWrite(LED_BUILTIN, LOW);
			threads.delay(STATUS_BLINK_MS);
			digitalWrite(LED_BUILTIN, HIGH);
			threads.delay(STATUS_BLINK_MS);
			digitalWrite(LED_BUILTIN, LOW);
			threads.delay(STATUS_DELAY_BLINK_MS);
		}
		else {
      for (int i = 0; i < 5; i++) { // blink 5 times quickly on RX
			  digitalWrite(LED_BUILTIN, HIGH);
			  threads.delay(RX_BLINK_MS);
			  digitalWrite(LED_BUILTIN, LOW);
			  threads.delay(RX_BLINK_MS);
      }
      isRx = false;
		}
		threads.yield();
	}
}

void setup() {
	pinMode(LED_BUILTIN, OUTPUT);
	threads.addThread(statusThread);
}

void loop() { }
