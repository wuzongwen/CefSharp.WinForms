using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.WinForms
{
    public class HotKeys
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vk);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        public static void RegHotKey(IntPtr hwnd, int hotKeyId, int keyModifiers, Keys key)
        {
            if (!HotKeys.RegisterHotKey(hwnd, hotKeyId, keyModifiers, key))
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                if (lastWin32Error == 1409)
                {
                    MessageBox.Show("热键被占用 ！");
                    return;
                }
                MessageBox.Show("注册热键失败！错误代码：" + lastWin32Error);
            }
        }
        public static void UnRegHotKey(IntPtr hwnd, int hotKeyId)
        {
            HotKeys.UnregisterHotKey(hwnd, hotKeyId);
        }

        //组合控制键
        public enum HotkeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

    }
}
