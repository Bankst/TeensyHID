#include "usb_names.h"

#define MANUFACTURER_NAME                                                                                  \
	{                                                                                                      \
		'M', 'e', 's', 'h', 'N', 'e', 't', ' ', 'T', 'e', 'c', 'h', 'n', 'o', 'l', 'o', 'g', 'i', 'e', 's' \
	}
#define MANUFACTURER_NAME_LEN 20
#define PRODUCT_NAME                                \
	{                                               \
		'T', 'e', 'e', 'n', 's', 'y', 'H', 'I', 'D' \
	}
#define PRODUCT_NAME_LEN 9

struct usb_string_descriptor_struct usb_string_manufacturer_name = {
	2 + MANUFACTURER_NAME_LEN * 2,
	3,
	MANUFACTURER_NAME};
struct usb_string_descriptor_struct usb_string_product_name = {
	2 + PRODUCT_NAME_LEN * 2,
	3,
	PRODUCT_NAME};
struct usb_string_descriptor_struct usb_string_serial_number = {
	12,
	3,
	{'4', '2', '0', '6', '9'}};
