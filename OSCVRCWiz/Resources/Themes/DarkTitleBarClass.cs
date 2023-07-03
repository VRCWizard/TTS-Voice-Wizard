using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace OSCVRCWiz.Resources.Themes
{
    internal class DarkTitleBarClass
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        //  private const int DWMWA_MICA_EFFECT = 1029;
        // static int trueValue = 0x01;

        internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                // DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int));
                // DwmSetWindowAttribute(handle, DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;

            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }
    }
}

