#include <Arduino.h>
#include "TeensyThreads.h"

#include "ardprintf.h"

#include "HID\HIDOpcode.h"
#include "HID\HIDPacket.h"
#include "HID\HIDHandler.h"

#include "version_num.h"
#include "build_defs.h"

#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 15
#define RX_LOOP_MS 40

#define STATUS_LED LED_BUILTIN
#define RX_LED 0

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

volatile bool isRx = false;

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

void hidThread()
{
	while (1)
	{
		HIDHandler::HIDReceive rx = HIDHandler::receive(lastRXPacket, 0);
		isRx = rx.len > 0;
		if (isRx) { printRXPacket(rx.valid); }

		if (rx.valid)
		{			
			HIDOpcode newOpcode = static_cast<HIDOpcode>(lastRXPacket.getOpcode() + 1);
			lastTXPacket = HIDPacket(newOpcode, "Hello World!");
			n = HIDHandler::send(lastTXPacket, 100);
		}
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

	Serial.begin(115200);
	delay(500);
	ardprintf("TeensyHID %s", completeVersion);

	threads.addThread(statusThread);
	threads.addThread(hidThread);
	threads.addThread(rxLedThread);
}

void loop() {}
