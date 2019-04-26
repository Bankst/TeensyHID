#include <Arduino.h>
#include "TeensyThreads.h"

#include "ardprintf.h"

#include "HID\HIDOpcode.h"
#include "HID\HIDPacket.h"
#include "HID\HIDHandler.h"

#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 15
#define RX_LOOP_MS 40

#define STATUS_LED LED_BUILTIN
#define RX_LED 0

HIDPacket lastRXPacket;
HIDPacket lastTXPacket;

volatile bool isRx = false;

byte buff[64];
volatile int n;

void printPacket(bool tx)
{
	HIDPacket *packet = tx ? &lastTXPacket : &lastRXPacket;
	const char *mode = tx ? "TX" : "RX";
	ardprintf("TeensyHID %s - Opcode: %s, Data %s", mode, packet->getOpcodeName(), packet->getData());
}

// void printPacket(bool rx) {
//   HIDPacket packet = rx ? lastRXPacket : lastTXPacket;
//   Serial.print(F("TeensyHID "));
//   Serial.print(rx ? F("RX") : F("TX"));
//   Serial.print(F("- Opcode: "));
//   Serial.print(HIDOpcodeNames[(int)packet.opcode]);
//   Serial.print(F(", Data: "));
//   for (int i = 0; i < 63; i++) {
//     Serial.write(packet.data[i]);
//   }
//   Serial.println();
// }

void ledOn(int led)
{
	digitalWriteFast(led, HIGH);
}

void ledOff(int led)
{
	digitalWriteFast(led, LOW);
}

void hidThread()
{
	bool shouldTx = false;
	while (1)
	{
		int n = HIDHandler::receive(lastRXPacket, 0);
		if (n > 0)
		{
			isRx = true;
			shouldTx = true;
		}

		if (shouldTx)
		{
			HIDOpcode newOpcode = (HIDOpcode)((byte)lastRXPacket.getOpcode() + 1);
			lastTXPacket = HIDPacket(newOpcode, "Hello World!");
			n = HIDHandler::send(lastTXPacket, 100);
			shouldTx = false;
		}
		threads.yield();
	}
}

// void hidThread() {
// while(1) {
//     HIDHandler.receive(0)
//     if (shouldTx) {
//       lastTXPacket = HIDPacket();
//       lastTXPacket.getOpcode = (HIDOpcode)((byte)lastRXPacket.opcode + (byte)1);
//       String respData = "";
//       switch(lastRXPacket.opcode) {
//         case HIDOpcode::INIT:
//           respData = "Hello world!";
//           break;
//         default:
//           respData = "No Data";
//           break;
//       }
//       respData.getBytes(lastTXPacket.data, 63);
//       buff[0] = (byte) lastTXPacket::getOpcode;
//       memcpy(buff + 1, lastTXPacket.data, 63);

//       n = RawHID.send(buff, 100);
//       if (n > 0) {
//         printPacket(false);
//       } else {
//         Serial.println(F("Unable to transmit packet"));
//       }
//       shouldTx = false;
// 		}
// threads.yield();
// }
// }

void statusThread()
{
	ledOff(STATUS_LED);
	while (1)
	{
		ledOn(STATUS_LED);
		threads.delay(STATUS_BLINK_MS);
		ledOff(STATUS_LED);
		threads.delay(STATUS_BLINK_MS);
		ledOn(STATUS_LED);
		threads.delay(STATUS_BLINK_MS);
		ledOff(STATUS_LED);
		threads.delay(STATUS_DELAY_BLINK_MS);
	}
	threads.yield();
}

void rxLedThread()
{
	ledOff(RX_LED);
	while (1)
	{
		if (isRx)
		{
			for (int i = 0; i < 5; i++)
			{ // blink 3 times quickly on RX
				ledOn(RX_LED);
				threads.delay(RX_BLINK_MS + (RX_BLINK_MS));
				ledOff(RX_LED);
				threads.delay(RX_BLINK_MS + (RX_BLINK_MS / 2));
			}
			isRx = false;
		}
		threads.yield();
	}
}

void setup()
{
	pinMode(RX_LED, OUTPUT);
	pinMode(STATUS_LED, OUTPUT);

	ledOn(RX_LED);
	ledOn(STATUS_LED);

	Serial.begin(9600);
	Serial.println(F("TeensyHID Running"));

	threads.addThread(statusThread);
	// threads.addThread(hidThread);
	threads.addThread(rxLedThread);
}

void loop() {}
