#include "TeensyThreads.h"

#include "HIDPacket.h"
#include "HIDOpcode.h"

#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 15
#define RX_LOOP_MS 40

#define STATUS_LED LED_BUILTIN
#define RX_LED 0

HIDPacket latestRXPacket, latestTXPacket;

volatile bool isRx = false;
volatile bool shouldTx = false;

void hidThread() {
	while(1) {
		byte buff[64];
		int n;
		n = RawHID.recv(buff, 0); // 0 timeout, do not wait
		if (n > 0) {
      isRx = true;
      latestRXPacket = HIDPacket();
			latestRXPacket.opcode = (HIDOpcode)(int)buff[0];
      memcpy(latestRXPacket.data, buff + 1, 63);
      printPacket(true);
      shouldTx = true;
		}
		if (shouldTx) {
      latestTXPacket = HIDPacket();
      latestTXPacket.opcode = (HIDOpcode)((byte)latestRXPacket.opcode + (byte)1);
      String respData = "";
      switch(latestRXPacket.opcode) {
        case HIDOpcode::INIT:          
          respData = "Hello world!";
          break;
        default:
          respData = "No Data";  
          break;
      }
      respData.getBytes(latestTXPacket.data, 63);
      buff[0] = (byte)latestTXPacket.opcode;
      memcpy(buff + 1, latestTXPacket.data, 63);

      n = RawHID.send(buff, 100);
      if (n > 0) {
        printPacket(false);
      } else {
        Serial.println(F("Unable to transmit packet"));
      }
      shouldTx = false;
		}
		threads.yield();
	}
}

void printPacket(bool rx) {
  HIDPacket packet = rx ? latestRXPacket : latestTXPacket;
  Serial.print(F("TeensyHID "));
  Serial.print(rx ? F("RX") : F("TX"));
  Serial.print(F("- Opcode: "));
  Serial.print(HIDOpcodeNames[(int)packet.opcode]);
  Serial.print(F(", Data: "));
  for (int i = 0; i < 63; i++) {
    Serial.write(packet.data[i]);
  }
  Serial.println();
}

void statusThread() {
	while(1) {
		digitalWrite(STATUS_LED, HIGH);
		threads.delay(STATUS_BLINK_MS);
		digitalWrite(STATUS_LED, LOW);
		threads.delay(STATUS_BLINK_MS);
		digitalWrite(STATUS_LED, HIGH);
		threads.delay(STATUS_BLINK_MS);
		digitalWrite(STATUS_LED, LOW);
		threads.delay(STATUS_DELAY_BLINK_MS);
	}
	threads.yield();
}

void rxLedThread() {
  while(1) {
    if (isRx) {
      for (int i = 0; i < 5; i++) { // blink 3 times quickly on RX
        digitalWrite(RX_LED, HIGH);
        threads.delay(RX_BLINK_MS + (RX_BLINK_MS));
        digitalWrite(RX_LED, LOW);
        threads.delay(RX_BLINK_MS + (RX_BLINK_MS / 2));
      }
      isRx = false;
    }
    threads.yield();    
  }
}

void setup() {
  Serial.begin(9600);
  Serial.println(F("TeensyHID Running"));
	
	pinMode(RX_LED, OUTPUT);
	pinMode(STATUS_LED, OUTPUT);

	threads.addThread(statusThread);
  threads.addThread(hidThread);
  threads.addThread(rxLedThread);
}

void loop() { }
