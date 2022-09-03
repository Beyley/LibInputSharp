using System;
using System.Runtime.InteropServices;

namespace LibInputSharp;

public enum bustype {
    BT_UNKNOWN,
    BT_USB,
    BT_BLUETOOTH,
    BT_PS2,
    BT_RMI,
    BT_I2C,
    BT_SPI,
};

/**
 * Contains the combined set of matches for one section or the values for
 * one device.
 *
 * bits defines which fields are set, the rest is zero.
 */
public unsafe struct match {
    uint bits;

    char*          name;
    public bustype bus;
    uint           vendor;
    uint           product;
    uint           version;

    char* dmi;/* dmi modalias with preceding "dmi:" */

    /* We can have more than one type set, so this is a bitfield */
    uint udev_type;

    char* dt;/* device tree compatible (first) string */
};

/**
 * Represents one section in the .quirks file.
 */
public unsafe struct section {
    public list link;

    bool has_match;/* to check for empty sections */
    bool has_property;/* to check for empty sections */

    char*        name;/* the [Section Name] */
    public match match;
    public list  properties;
};

public struct quirk_dimensions {
    nint x, y;
};

public struct quirk_range {
    int lower, upper;
};

public unsafe struct quirk_tuples {
    private fixed int tuples[64];
    nint              ntuples;
};

/**
 * Quirks known to libinput
 */
public enum quirk {
    QUIRK_MODEL_ALPS_SERIAL_TOUCHPAD = 100,
    QUIRK_MODEL_APPLE_TOUCHPAD,
    QUIRK_MODEL_APPLE_TOUCHPAD_ONEBUTTON,
    QUIRK_MODEL_BOUNCING_KEYS,
    QUIRK_MODEL_CHROMEBOOK,
    QUIRK_MODEL_CLEVO_W740SU,
    QUIRK_MODEL_HP_PAVILION_DM4_TOUCHPAD,
    QUIRK_MODEL_HP_STREAM11_TOUCHPAD,
    QUIRK_MODEL_HP_ZBOOK_STUDIO_G3,
    QUIRK_MODEL_INVERT_HORIZONTAL_SCROLLING,
    QUIRK_MODEL_LENOVO_SCROLLPOINT,
    QUIRK_MODEL_LENOVO_T450_TOUCHPAD,
    QUIRK_MODEL_LENOVO_X1GEN6_TOUCHPAD,
    QUIRK_MODEL_LENOVO_X230,
    QUIRK_MODEL_SYNAPTICS_SERIAL_TOUCHPAD,
    QUIRK_MODEL_SYSTEM76_BONOBO,
    QUIRK_MODEL_SYSTEM76_GALAGO,
    QUIRK_MODEL_SYSTEM76_KUDU,
    QUIRK_MODEL_TABLET_MODE_NO_SUSPEND,
    QUIRK_MODEL_TABLET_MODE_SWITCH_UNRELIABLE,
    QUIRK_MODEL_TOUCHPAD_VISIBLE_MARKER,
    QUIRK_MODEL_TRACKBALL,
    QUIRK_MODEL_WACOM_TOUCHPAD,
    QUIRK_MODEL_DELL_CANVAS_TOTEM,

    _QUIRK_LAST_MODEL_QUIRK_,/* Guard: do not modify */


    QUIRK_ATTR_SIZE_HINT = 300,
    QUIRK_ATTR_TOUCH_SIZE_RANGE,
    QUIRK_ATTR_PALM_SIZE_THRESHOLD,
    QUIRK_ATTR_LID_SWITCH_RELIABILITY,
    QUIRK_ATTR_KEYBOARD_INTEGRATION,
    QUIRK_ATTR_TRACKPOINT_INTEGRATION,
    QUIRK_ATTR_TPKBCOMBO_LAYOUT,
    QUIRK_ATTR_PRESSURE_RANGE,
    QUIRK_ATTR_PALM_PRESSURE_THRESHOLD,
    QUIRK_ATTR_RESOLUTION_HINT,
    QUIRK_ATTR_TRACKPOINT_MULTIPLIER,
    QUIRK_ATTR_THUMB_PRESSURE_THRESHOLD,
    QUIRK_ATTR_USE_VELOCITY_AVERAGING,
    QUIRK_ATTR_THUMB_SIZE_THRESHOLD,
    QUIRK_ATTR_MSC_TIMESTAMP,
    QUIRK_ATTR_EVENT_CODE_DISABLE,
    QUIRK_ATTR_EVENT_CODE_ENABLE,
    QUIRK_ATTR_INPUT_PROP_DISABLE,
    QUIRK_ATTR_INPUT_PROP_ENABLE,

    _QUIRK_LAST_ATTR_QUIRK_,/* Guard: do not modify */
};

[StructLayout(LayoutKind.Explicit)]
public unsafe struct value {
    [FieldOffset(0)] public bool             b;
    [FieldOffset(0)] public uint             u;
    [FieldOffset(0)] public int              i;
    [FieldOffset(0)] public char*            s;
    [FieldOffset(0)] public double           d;
    [FieldOffset(0)] public quirk_dimensions dim;
    [FieldOffset(0)] public quirk_range      range;
    [FieldOffset(0)] public quirk_tuples     tuples;
    [FieldOffset(0)] public quirk_array      array;
}

public unsafe struct quirk_array {
    fixed uint data[32];
    nint       nelements;
};

/**
 * Generic value holder for the property types we support. The type
 * identifies which value in the union is defined and we expect callers to
 * already know which type yields which value.
 */
public unsafe struct property {
    IntPtr      refcount;
    public list link;/* struct sections.properties */

    public quirk         id;
    public property_type type;
    public value         value;
/*
 *    union {
        bool                    b;
        uint32_t                u;
        int32_t                 i;
        char *                  s;
        double                  d;
        struct quirk_dimensions dim;
        struct quirk_range      range;
        struct quirk_tuples     tuples;
        struct quirk_array      array;
    } value;
 */
};

public enum property_type {
    PT_UINT,
    PT_INT,
    PT_STRING,
    PT_BOOL,
    PT_DIMENSION,
    PT_RANGE,
    PT_DOUBLE,
    PT_TUPLES,
    PT_UINT_ARRAY,
};

/**
 * The struct returned to the caller. It contains the
 * properties for a given device.
 */
public unsafe struct quirks {
    IntPtr      refcount;
    public list link;/* struct quirks_context.quirks */

    /* These are not ref'd, just a collection of pointers */
    public property** properties;
    IntPtr            nproperties;
};

public enum quirks_log_type {
    QLOG_LIBINPUT_LOGGING,
    QLOG_CUSTOM_LOG_PRIORITIES,
};

/**
 * Quirk matching context, initialized once with quirks_init_subsystem()
 */
public unsafe struct quirks_context {
    IntPtr refcount;

    IntPtr                 log_handler;//libinput_log_handler
    public quirks_log_type log_type;
    public libinput*       libinput;/* for logging */

    char* dmi;
    char* dt;

    public list sections;

    /* list of quirks handed to libinput, just for bookkeeping */
    public list quirks;
};
