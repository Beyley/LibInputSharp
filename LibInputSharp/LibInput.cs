using System;
using System.Runtime.InteropServices;

namespace LibInputSharp;

public class LibInput {
    private const string LIBRARY_NAME = "libinput";
    private const int    EV_VERSION   = 0x010001;
    
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe libinput* libinput_udev_create_context(libinput_interface* libinputInterface, void* userData, udev* udev);
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe int libinput_udev_assign_seat(libinput* li, [MarshalAs(UnmanagedType.LPStr)]string seat0);
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe int libinput_dispatch(libinput* libinput);
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe libinput_event* libinput_get_event(libinput* li);
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe void libinput_event_destroy(libinput_event* @event);
    [DllImport(LIBRARY_NAME)]
    public static extern unsafe libinput* libinput_unref(libinput* li);
}

/**
 * @ingroup base
 *
 * Event type for events returned by libinput_get_event().
 */
public enum libinput_event_type {
    /**
	 * This is not a real event type, and is only used to tell the user that
	 * no new event is available in the queue. See
	 * libinput_next_event_type().
	 */
    LIBINPUT_EVENT_NONE = 0,

    /**
	 * Signals that a device has been added to the context. The device will
	 * not be read until the next time the user calls libinput_dispatch()
	 * and data is available.
	 *
	 * This allows setting up initial device configuration before any events
	 * are created.
	 */
    LIBINPUT_EVENT_DEVICE_ADDED,

    /**
	 * Signals that a device has been removed. No more events from the
	 * associated device will be in the queue or be queued after this event.
	 */
    LIBINPUT_EVENT_DEVICE_REMOVED,

    LIBINPUT_EVENT_KEYBOARD_KEY = 300,

    LIBINPUT_EVENT_POINTER_MOTION = 400,
    LIBINPUT_EVENT_POINTER_MOTION_ABSOLUTE,
    LIBINPUT_EVENT_POINTER_BUTTON,
    LIBINPUT_EVENT_POINTER_AXIS,

    LIBINPUT_EVENT_TOUCH_DOWN = 500,
    LIBINPUT_EVENT_TOUCH_UP,
    LIBINPUT_EVENT_TOUCH_MOTION,
    LIBINPUT_EVENT_TOUCH_CANCEL,
    /**
	 * Signals the end of a set of touchpoints at one device sample
	 * time. This event has no coordinate information attached.
	 */
    LIBINPUT_EVENT_TOUCH_FRAME,

    /**
	 * One or more axes have changed state on a device with the @ref
	 * LIBINPUT_DEVICE_CAP_TABLET_TOOL capability. This event is only sent
	 * when the tool is in proximity, see @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_PROXIMITY for details.
	 *
	 * The proximity event contains the initial state of the axis as the
	 * tool comes into proximity. An event of type @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_AXIS is only sent when an axis value
	 * changes from this initial state. It is possible for a tool to
	 * enter and leave proximity without sending an event of type @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_AXIS.
	 *
	 * An event of type @ref LIBINPUT_EVENT_TABLET_TOOL_AXIS is sent
	 * when the tip state does not change. See the documentation for
	 * @ref LIBINPUT_EVENT_TABLET_TOOL_TIP for more details.
	 *
	 * @since 1.2
	 */
    LIBINPUT_EVENT_TABLET_TOOL_AXIS = 600,
    /**
	 * Signals that a tool has come in or out of proximity of a device with
	 * the @ref LIBINPUT_DEVICE_CAP_TABLET_TOOL capability.
	 *
	 * Proximity events contain each of the current values for each axis,
	 * and these values may be extracted from them in the same way they are
	 * with @ref LIBINPUT_EVENT_TABLET_TOOL_AXIS events.
	 *
	 * Some tools may always be in proximity. For these tools, events of
	 * type @ref LIBINPUT_TABLET_TOOL_PROXIMITY_STATE_IN are sent only once after @ref
	 * LIBINPUT_EVENT_DEVICE_ADDED, and events of type @ref
	 * LIBINPUT_TABLET_TOOL_PROXIMITY_STATE_OUT are sent only once before @ref
	 * LIBINPUT_EVENT_DEVICE_REMOVED.
	 *
	 * If the tool that comes into proximity supports x/y coordinates,
	 * libinput guarantees that both x and y are set in the proximity
	 * event.
	 *
	 * When a tool goes out of proximity, the value of every axis should be
	 * assumed to have an undefined state and any buttons that are currently held
	 * down on the stylus are marked as released. Button release events for
	 * each button that was held down on the stylus are sent before the
	 * proximity out event.
	 *
	 * @since 1.2
	 */
    LIBINPUT_EVENT_TABLET_TOOL_PROXIMITY,
    /**
	 * Signals that a tool has come in contact with the surface of a
	 * device with the @ref LIBINPUT_DEVICE_CAP_TABLET_TOOL capability.
	 *
	 * On devices without distance proximity detection, the @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_TIP is sent immediately after @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_PROXIMITY for the tip down event, and
	 * immediately before for the tip up event.
	 *
	 * The decision when a tip touches the surface is device-dependent
	 * and may be derived from pressure data or other means. If the tip
	 * state is changed by axes changing state, the
	 * @ref LIBINPUT_EVENT_TABLET_TOOL_TIP event includes the changed
	 * axes and no additional axis event is sent for this state change.
	 * In other words, a caller must look at both @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_AXIS and @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_TIP events to know the current state
	 * of the axes.
	 *
	 * If a button state change occurs at the same time as a tip state
	 * change, the order of events is device-dependent.
	 *
	 * @since 1.2
	 */
    LIBINPUT_EVENT_TABLET_TOOL_TIP,
    /**
	 * Signals that a tool has changed a logical button state on a
	 * device with the @ref LIBINPUT_DEVICE_CAP_TABLET_TOOL capability.
	 *
	 * Button state changes occur on their own and do not include axis
	 * state changes. If button and axis state changes occur within the
	 * same logical hardware event, the order of the @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_BUTTON and @ref
	 * LIBINPUT_EVENT_TABLET_TOOL_AXIS event is device-specific.
	 *
	 * This event is not to be confused with the button events emitted
	 * by the tablet pad. See @ref LIBINPUT_EVENT_TABLET_PAD_BUTTON.
	 *
	 * @see LIBINPUT_EVENT_TABLET_PAD_BUTTON
	 *
	 * @since 1.2
	 */
    LIBINPUT_EVENT_TABLET_TOOL_BUTTON,

    /**
	 * A button pressed on a device with the @ref
	 * LIBINPUT_DEVICE_CAP_TABLET_PAD capability.
	 *
	 * A button differs from @ref LIBINPUT_EVENT_TABLET_PAD_KEY in that
	 * buttons are sequentially indexed from 0 and do not carry any
	 * other information.  Keys have a specific functionality assigned
	 * to them. The key code thus carries a semantic meaning, a button
	 * number does not.
	 *
	 * This event is not to be confused with the button events emitted
	 * by tools on a tablet (@ref LIBINPUT_EVENT_TABLET_TOOL_BUTTON).
	 *
	 * @since 1.3
	 */
    LIBINPUT_EVENT_TABLET_PAD_BUTTON = 700,
    /**
	 * A status change on a tablet ring with the @ref
	 * LIBINPUT_DEVICE_CAP_TABLET_PAD capability.
	 *
	 * @since 1.3
	 */
    LIBINPUT_EVENT_TABLET_PAD_RING,

    /**
	 * A status change on a strip on a device with the @ref
	 * LIBINPUT_DEVICE_CAP_TABLET_PAD capability.
	 *
	 * @since 1.3
	 */
    LIBINPUT_EVENT_TABLET_PAD_STRIP,

    /**
	 * A key pressed on a device with the @ref
	 * LIBINPUT_DEVICE_CAP_TABLET_PAD capability.
	 *
	 * A key differs from @ref LIBINPUT_EVENT_TABLET_PAD_BUTTON in that
	 * keys have a specific functionality assigned to them (buttons are
	 * sequentially ordered). The key code thus carries a semantic
	 * meaning, a button number does not.
	 *
	 * @since 1.15
	 */
    LIBINPUT_EVENT_TABLET_PAD_KEY,

    LIBINPUT_EVENT_GESTURE_SWIPE_BEGIN = 800,
    LIBINPUT_EVENT_GESTURE_SWIPE_UPDATE,
    LIBINPUT_EVENT_GESTURE_SWIPE_END,
    LIBINPUT_EVENT_GESTURE_PINCH_BEGIN,
    LIBINPUT_EVENT_GESTURE_PINCH_UPDATE,
    LIBINPUT_EVENT_GESTURE_PINCH_END,

    /**
	 * @since 1.7
	 */
    LIBINPUT_EVENT_SWITCH_TOGGLE = 900,
};

public unsafe struct libinput_device_group {
    int   refcount;
    void* user_data;
    char* identifier;/* unique identifier or NULL for singletons */

    public list link;
};

public unsafe struct list {
    public list* prev;
    public list* next;
}

public unsafe struct libinput_source {
    IntPtr      dispatch;//libinput_source_dispatch_t
    void*       user_data;
    int         fd;
    public list link;
};

public unsafe struct timer {
    public list             list;
    public libinput_source* source;
    int                     fd;
    ulong                   next_expiry;
}

public unsafe struct libinput {
    int         epoll_fd;
    public list source_destroy_list;

    public list seat_list;

    public timer timer;

    public libinput_event** events;
    IntPtr                  events_count;
    IntPtr                  events_len;
    IntPtr                  events_in;
    IntPtr                  events_out;

    public list tool_list;

    public libinput_interface *@interface;
    public libinput_interface_backend* interface_backend;

    IntPtr         log_handler; //libinput_log_handler
    public libinput_log_priority log_priority;
    void*                        user_data;
    int                          refcount;

    public list device_group_list;

    ulong last_event_time;
    ulong dispatch_time;

    bool                   quirks_initialized;
    public quirks_context* quirks;

// #if HAVE_LIBWACOM
    public libwacom libwacom;
// #endif
};

/**
 * @ingroup base
 *
 * Log priority for internal logging messages.
 */
public enum libinput_log_priority {
    LIBINPUT_LOG_PRIORITY_DEBUG = 10,
    LIBINPUT_LOG_PRIORITY_INFO  = 20,
    LIBINPUT_LOG_PRIORITY_ERROR = 30,
};

public struct libinput_interface_backend {
    // int (*resume)(struct libinput *libinput);
    // void (*suspend)(struct libinput *libinput);
    // void (*destroy)(struct libinput *libinput);
    // int (*device_change_seat)(struct libinput_device *device, const char * seat_name);

    public IntPtr resume;
    public IntPtr suspend;
    public IntPtr destroy;
    public IntPtr device_change_seat;
};

public unsafe struct libinput_interface {
    /**
	 * Open the device at the given path with the flags provided and
	 * return the fd.
	 *
	 * @param path The device path to open
	 * @param flags Flags as defined by open(2)
	 * @param user_data The user_data provided in
	 * libinput_udev_create_context()
	 *
	 * @return The file descriptor, or a negative errno on failure.
	 */
    public IntPtr open_restricted;
    // int (*open_restricted)(const char * path, int flags, void *user_data);
    /**
	 * Close the file descriptor.
	 *
	 * @param fd The file descriptor to close
	 * @param user_data The user_data provided in
	 * libinput_udev_create_context()
	 */
    public IntPtr close_restricted;
    // void (*close_restricted)(int fd, void *user_data);
};

public unsafe struct libwacom {
    IntPtr db; //WacomDeviceDatabase*
    IntPtr               refcount;
}

public unsafe struct libinput_seat {
    public libinput*           libinput;
    public list                link;
    public list                devices_list;
    void*                      user_data;
    int                        refcount;
    IntPtr destroy; //libinput_seat_destroy_func

    char* physical_name;
    char* logical_name;

    uint slot_map;

    public fixed uint button_count[InputEventCodes.KEY_CNT];
};

public unsafe struct libinput_device {
    public libinput_seat*         seat;
    public libinput_device_group* group;
    public list                   link;
    public list                   event_listeners;
    void*                         user_data;
    int                           refcount;
    public libinput_device_config config;
};

// struct libinput_device_config_tap {
// 	int (*count)(struct libinput_device *device);
// 	enum libinput_config_status (*set_enabled)(struct libinput_device *device,
// 						   enum libinput_config_tap_state enable);
// 	enum libinput_config_tap_state (*get_enabled)(struct libinput_device *device);
// 	enum libinput_config_tap_state (*get_default)(struct libinput_device *device);
//
// 	enum libinput_config_status (*set_map)(struct libinput_device *device,
// 						   enum libinput_config_tap_button_map map);
// 	enum libinput_config_tap_button_map (*get_map)(struct libinput_device *device);
// 	enum libinput_config_tap_button_map (*get_default_map)(struct libinput_device *device);
//
// 	enum libinput_config_status (*set_drag_enabled)(struct libinput_device *device,
// 							enum libinput_config_drag_state);
// 	enum libinput_config_drag_state (*get_drag_enabled)(struct libinput_device *device);
// 	enum libinput_config_drag_state (*get_default_drag_enabled)(struct libinput_device *device);
//
// 	enum libinput_config_status (*set_draglock_enabled)(struct libinput_device *device,
// 							    enum libinput_config_drag_lock_state);
// 	enum libinput_config_drag_lock_state (*get_draglock_enabled)(struct libinput_device *device);
// 	enum libinput_config_drag_lock_state (*get_default_draglock_enabled)(struct libinput_device *device);
// };
//
// struct libinput_device_config_calibration {
// 	int (*has_matrix)(struct libinput_device *device);
// 	enum libinput_config_status (*set_matrix)(struct libinput_device *device,
// 						  const float matrix[6]);
// 	int (*get_matrix)(struct libinput_device *device,
// 			  float matrix[6]);
// 	int (*get_default_matrix)(struct libinput_device *device,
// 							  float matrix[6]);
// };
//
// struct libinput_device_config_send_events {
// 	uint32_t (*get_modes)(struct libinput_device *device);
// 	enum libinput_config_status (*set_mode)(struct libinput_device *device,
// 						   enum libinput_config_send_events_mode mode);
// 	enum libinput_config_send_events_mode (*get_mode)(struct libinput_device *device);
// 	enum libinput_config_send_events_mode (*get_default_mode)(struct libinput_device *device);
// };
//
// struct libinput_device_config_accel {
// 	int (*available)(struct libinput_device *device);
// 	enum libinput_config_status (*set_speed)(struct libinput_device *device,
// 						 double speed);
// 	double (*get_speed)(struct libinput_device *device);
// 	double (*get_default_speed)(struct libinput_device *device);
//
// 	uint32_t (*get_profiles)(struct libinput_device *device);
// 	enum libinput_config_status (*set_profile)(struct libinput_device *device,
// 						   enum libinput_config_accel_profile);
// 	enum libinput_config_accel_profile (*get_profile)(struct libinput_device *device);
// 	enum libinput_config_accel_profile (*get_default_profile)(struct libinput_device *device);
// };
//
// struct libinput_device_config_natural_scroll {
// 	int (*has)(struct libinput_device *device);
// 	enum libinput_config_status (*set_enabled)(struct libinput_device *device,
// 						   int enabled);
// 	int (*get_enabled)(struct libinput_device *device);
// 	int (*get_default_enabled)(struct libinput_device *device);
// };
//
// struct libinput_device_config_left_handed {
// 	int (*has)(struct libinput_device *device);
// 	enum libinput_config_status (*set)(struct libinput_device *device, int left_handed);
// 	int (*get)(struct libinput_device *device);
// 	int (*get_default)(struct libinput_device *device);
// };
//
// struct libinput_device_config_scroll_method {
// 	uint32_t (*get_methods)(struct libinput_device *device);
// 	enum libinput_config_status (*set_method)(struct libinput_device *device,
// 						  enum libinput_config_scroll_method method);
// 	enum libinput_config_scroll_method (*get_method)(struct libinput_device *device);
// 	enum libinput_config_scroll_method (*get_default_method)(struct libinput_device *device);
// 	enum libinput_config_status (*set_button)(struct libinput_device *device,
// 						  uint32_t button);
// 	uint32_t (*get_button)(struct libinput_device *device);
// 	uint32_t (*get_default_button)(struct libinput_device *device);
// 	enum libinput_config_status (*set_button_lock)(struct libinput_device *device,
// 						       enum libinput_config_scroll_button_lock_state);
// 	enum libinput_config_scroll_button_lock_state (*get_button_lock)(struct libinput_device *device);
// 	enum libinput_config_scroll_button_lock_state (*get_default_button_lock)(struct libinput_device *device);
// };
//
// struct libinput_device_config_click_method {
// 	uint32_t (*get_methods)(struct libinput_device *device);
// 	enum libinput_config_status (*set_method)(struct libinput_device *device,
// 						  enum libinput_config_click_method method);
// 	enum libinput_config_click_method (*get_method)(struct libinput_device *device);
// 	enum libinput_config_click_method (*get_default_method)(struct libinput_device *device);
// };
//
// struct libinput_device_config_middle_emulation {
// 	int (*available)(struct libinput_device *device);
// 	enum libinput_config_status (*set)(
// 			 struct libinput_device *device,
// 			 enum libinput_config_middle_emulation_state);
// 	enum libinput_config_middle_emulation_state (*get)(
// 			 struct libinput_device *device);
// 	enum libinput_config_middle_emulation_state (*get_default)(
// 			 struct libinput_device *device);
// };
//
// struct libinput_device_config_dwt {
// 	int (*is_available)(struct libinput_device *device);
// 	enum libinput_config_status (*set_enabled)(
// 			 struct libinput_device *device,
// 			 enum libinput_config_dwt_state enable);
// 	enum libinput_config_dwt_state (*get_enabled)(
// 			 struct libinput_device *device);
// 	enum libinput_config_dwt_state (*get_default_enabled)(
// 			 struct libinput_device *device);
// };
//
// struct libinput_device_config_rotation {
// 	int (*is_available)(struct libinput_device *device);
// 	enum libinput_config_status (*set_angle)(
// 			 struct libinput_device *device,
// 			 unsigned int degrees_cw);
// 	unsigned int (*get_angle)(struct libinput_device *device);
// 	unsigned int (*get_default_angle)(struct libinput_device *device);
// };

public struct libinput_device_config {
    public IntPtr tap;//public libinput_device_config_tap*         
    public IntPtr calibration;//public libinput_device_config_calibration*     
    public IntPtr sendevents;//public libinput_device_config_send_events*     
    public IntPtr accel;//public libinput_device_config_accel*           
    public IntPtr natural_scroll;//public libinput_device_config_natural_scroll*  
    public IntPtr left_handed;//public libinput_device_config_left_handed*     
    public IntPtr scroll_method;//public libinput_device_config_scroll_method*   
    public IntPtr click_method;//public libinput_device_config_click_method*    
    public IntPtr middle_emulation;//public libinput_device_config_middle_emulation*
    public IntPtr dwt;//public libinput_device_config_dwt*             
    public IntPtr rotation;//public libinput_device_config_rotation*        
};

public unsafe struct libinput_event {
    public libinput_event_type type;
    public libinput_device*    device;
};

public struct Interface {
    public IntPtr OpenRestricted;
    public IntPtr CloseRestricted;
}

public struct LibInputContext {}

public struct libinput_event_device_notify {
    public libinput_event @base;
};

public unsafe struct libinput_event_keyboard {
    public libinput_event     @base;
    ulong                     time;
    uint                      key;
    uint                      seat_key_count;
    public libinput_key_state state;
};

/**
 * @ingroup device
 *
 * Logical state of a key. Note that the logical state may not represent
 * the physical state of the key.
 */
public enum libinput_key_state {
    LIBINPUT_KEY_STATE_RELEASED = 0,
    LIBINPUT_KEY_STATE_PRESSED  = 1
};