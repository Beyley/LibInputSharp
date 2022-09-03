using System.Runtime.InteropServices;

namespace LibInputSharp;

public struct udev {
    
}

public static class Udev {
    [DllImport("libudev")]
    public static extern unsafe udev* udev_new();
}
