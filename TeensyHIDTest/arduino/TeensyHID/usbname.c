#include "usb_names.h"

#define PRODUCT_NAME    {'T','e','e','n','s','y','H','I','D'}
#define PRODUCT_NAME_LEN  9

struct usb_string_descriptor_struct usb_string_product_name = {
  2 + PRODUCT_NAME_LEN * 2,
  3,
  PRODUCT_NAME
};
