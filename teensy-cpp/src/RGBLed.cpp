#include "RGBLed.h"

RGBLed::RGBLed(short redPin, short bluePin, short greenPin, bool isCommonCathode) {
	_redPin = redPin;
	_bluePin = bluePin;
	_greenPin = greenPin;
	_isCommonCathode = isCommonCathode;

	pinMode(_redPin, OUTPUT);
	pinMode(_bluePin, OUTPUT);
	pinMode(_greenPin, OUTPUT);
}

void RGBLed::setColor(uint8_t red, uint8_t green, uint8_t blue) {
	setOutputs(red, green, blue);
}

void RGBLed::setColor(Color color) {
	setColor(color.r, color.g, color.b);
}

void RGBLed::off() {
	setColor(Color::black());
}

void RGBLed::setOutputs(uint8_t red, uint8_t green, uint8_t blue) {
	if (_isCommonCathode) red = 255 - red;
	if (_isCommonCathode) green = 255 - green;
	if (_isCommonCathode) blue = 255 - blue;
	analogWrite(_redPin, red);
	analogWrite(_greenPin, green);
	analogWrite(_bluePin, blue);
}