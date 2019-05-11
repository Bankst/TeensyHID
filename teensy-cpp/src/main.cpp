#include <Arduino.h>
#include <TeensyThreads.h>

#include "ardprintf.h"
#include "ardtrace.h"
#include "build_defs.h"
#include "Color.h"
#include "HID\HIDOpcode.h"
#include "HID\HIDPacket.h"
#include "HID\HIDMessages.h"
#include "HID\HIDManager.h"
#include "model.h"
#include "RGBLed.h"
#include "version_num.h"


#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 15
#define RX_LOOP_MS 40

#define STATUS_LED LED_BUILTIN
#define RX_LED 0

#define RGB_RED_PIN 23
#define RGB_GREEN_PIN 22
#define RGB_BLUE_PIN 21

const unsigned char completeVersion[] =
{
	'V',
	VERSION_MAJOR_INIT,
	'.',
	VERSION_MINOR_INIT,
	'-',
	BUILD_YEAR_CH0, BUILD_YEAR_CH1, BUILD_YEAR_CH2, BUILD_YEAR_CH3,
	'-',
	BUILD_MONTH_CH0, BUILD_MONTH_CH1,
	'-',
	BUILD_DAY_CH0, BUILD_DAY_CH1,
	'-',
	BUILD_HOUR_CH0, BUILD_HOUR_CH1,
	':',
	BUILD_MIN_CH0, BUILD_MIN_CH1,
	':',
	BUILD_SEC_CH0, BUILD_SEC_CH1,
	'\0'
};

HIDPacket lastRXPacket;
HIDPacket lastTXPacket;

RGBLed rgbLed(RGB_RED_PIN, RGB_BLUE_PIN, RGB_GREEN_PIN);

volatile bool isRx = false;
volatile bool isRxLarge = false;

byte buff[64];
volatile int n;

void printRXPacket(bool valid)
{
	ardprintf("TeensyHID-DEV RX - Opcode: %s, Data: %s", valid ? lastRXPacket.getOpcodeName() : "UNKNOWN", lastRXPacket.getData());
}

void printTXPacket() {
	ardprintf("TeensyHID-DEV TX - Opcode: %s, Data: %s", lastTXPacket.getOpcodeName(), lastTXPacket.getData());
}

void ledOn(int led)
{
	digitalWriteFast(led, HIGH);
}

void ledOff(int led)
{
	digitalWriteFast(led, LOW);
}

void largeRX() {
	uint8_t singleBuff[64]; // per-packet buffer
	std::vector<uint8_t> fullBuff; // master buffer for entire message

	uint8_t opcode;
	uint8_t packetCount;

	// check for first packet
	int n = RawHID.recv(singleBuff, 0);
	
	// if packet received, set opcode and packetCount
	if(n > 0) {
		opcode = singleBuff[0];
		packetCount = singleBuff[1];
	} else return; // just return if no packet received

	// now check to see if the opcode is even valid
	bool valid = opcode < (uint8_t)HIDOpcode::HIDOPCODE_LENGTH;
	if (!valid) {
		Serial.println("Invalid opcode!");
		return;
	}
	// this is where the magic happens

	if (packetCount == 1) { // only 1 packet
		// normal packet junk
		// not concerned with this, 64-byte messages work fine
		Serial.println("SinglePacket");
		// lastRXPacket = HIDPacket(singleBuff);
		return;
	} else if (packetCount > 1) { // if we have atleast 1 more packets coming
		ardprintf("Got MultiPacket %d of %d", 0, packetCount);
		fullBuff.assign(singleBuff, singleBuff + 64); // copy first packet's data in to the fullBuff

		// iterate the packetCount (starting at 1) and receive all packets
		for (int i = 1; i < packetCount; i++) {
			int n2;
			bool got;

			// keep trying to receive until we get the next packet
			// (did we get to this step fast enough? should I slow down the PC app?)
			// also look at adding a timeout (give up on message after x millis)
			Serial.print("Waiting for packet");
			while (!got) {
				n2 = RawHID.recv(singleBuff, 25);
				if (n2 < 1) Serial.print('.');
				got = n2 > 0;
			}
			Serial.println();
			ardprintf("Got MultiPacket %d of %d", i, packetCount);
			// append each packet to the end of the vector (is this right?)
			// fullBuff.insert(fullBuff.end, std::begin(singleBuff), std::end(singleBuff));
		}
	}
}

void hidThread()
{
	while (1)
	{
		threads.yield();
	}
}

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
		TRACE();
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
	rgbLed.setColor(Color::white());
	

	Serial.begin(115200);
	delay(500);
	ardprintf("TeensyHID %s", completeVersion);

	rgbLed.off();

	threads.addThread(statusThread);
	//threads.addThread(hidThread);
	threads.addThread(rxLedThread);
}

void loop()
{
	largeRX();
}
