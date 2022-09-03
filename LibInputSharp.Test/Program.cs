using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace LibInputSharp.Test; 

public static class Program {
    public unsafe delegate int open_restricted(char* path, int flags, void* user_data);

    public unsafe delegate void close_restricted(int fd, void* user_data);

    private static Dictionary<int, SafeFileHandle> _fileHandles = new();

    public static unsafe void Main() {
        libinput*          li;
        libinput_event*    @event;
        libinput_interface @interface = new();
        
        open_restricted openRestricted = delegate(char* pathPtr, int flags, void* data) {
            string path = Marshal.PtrToStringAuto(new IntPtr(pathPtr)) ?? throw new Exception();

            SafeFileHandle handle             = File.OpenHandle(path);
            IntPtr         dangerousHandle = handle.DangerousGetHandle();

            _fileHandles[(int)dangerousHandle] = handle;

            return (int)dangerousHandle;
        };
        close_restricted closeRestricted = delegate(int fd, void* data) {
            SafeFileHandle handle = _fileHandles[fd];
            
            handle.Close();
        };

        @interface.open_restricted = Marshal.GetFunctionPointerForDelegate(openRestricted);
        @interface.close_restricted = Marshal.GetFunctionPointerForDelegate(closeRestricted);

        udev* udev = Udev.udev_new();
        
        li = LibInput.libinput_udev_create_context(&@interface, null, udev);
        
        LibInput.libinput_udev_assign_seat(li, "seat0");
        LibInput.libinput_dispatch(li);
 
        while ((@event = LibInput.libinput_get_event(li)) != null) {
 
            // handle the event here
            
            Console.WriteLine($"libinput event: {@event->type}");

            LibInput.libinput_event_destroy(@event);
            LibInput.libinput_dispatch(li);
        }
 
        LibInput.libinput_unref(li);
    }
}