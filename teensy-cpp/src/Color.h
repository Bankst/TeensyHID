#ifndef COLOR_H
#define COLOR_H


#include <stdint.h>

struct Color
{
    static constexpr Color red() { return Color(255, 0, 0); }
    static constexpr Color green() { return Color(0, 255, 0); }
    static constexpr Color blue() { return Color(0, 0, 255); }
    static constexpr Color black() { return Color(0, 0, 0); }
    static constexpr Color white() { return Color(255, 255, 255); }
    static constexpr Color purple() { return Color(255, 0, 255); }
    static constexpr Color yellow() { return Color(255, 255, 0); }

    constexpr Color(uint8_t r, uint8_t g, uint8_t b, uint8_t a = 255)
        : r(r), g(g), b(b), a(a)
    {}

     uint8_t r;
     uint8_t g;
     uint8_t b;
     uint8_t a;
};

#endif /* COLOR_H */
