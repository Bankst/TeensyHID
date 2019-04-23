#include "TeensyThreads.h"
#include "RawHID.h"

#define STATUS_BLINK_MS 60
#define STATUS_DELAY_BLINK_MS 1000
#define RX_BLINK_MS 20

volatile bool isRx = false;

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
      digitalWrite(LED_BUILTIN, HIGH);
      threads.delay(RX_BLINK_MS);
      digitalWrite(LED_BUILTIN, LOW);
      threads.delay(RX_BLINK_MS);
    }
    threads.yield();
  }
}

void setup() {
  pinMode(LED_BUILTIN, OUTPUT);
  threads.addThread(statusThread);
}

void loop() {
  // put your main code here, to run repeatedly:

}
