#ifndef _RGBLED_H
#define _RGBLED_H

#include <Arduino.h>
#include "Color.h"

class RGBLed {
  public:
	RGBLed(short redPin, short greenPin, short bluePin, bool isCommonCathode = false);

	void setColor(uint8_t red, uint8_t green, uint8_t blue);
	void setColor(Color color);
	void off();
  private:
	short _redPin, _greenPin, _bluePin;
	bool _isCommonCathode;
	void setOutputs(uint8_t red, uint8_t green, uint8_t blue);
};

#endif