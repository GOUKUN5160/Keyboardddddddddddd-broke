using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace キーボーddddddddddッドが壊れた
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new Form1();
            var keyHook = new KeyHook();
            keyHook.Hook();
            Application.Run();
        }
    }

    class KeyHook
    {
        delegate int delegateHookCallback(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, delegateHookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        IntPtr hookPtr = IntPtr.Zero;
        KeyInput keyInput = new KeyInput();
        bool preve = false;


        public void Hook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookPtr = SetWindowsHookEx(
                    13,
                    HookCallback,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
        }

        int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            switch ((int)wParam)
            {
                case 256:
                    short keyCode = (short)Marshal.ReadInt32(lParam);
                    if (keyCode == 68 & !preve)
                    {
                        preve = true;
                        for (int i = 0; i < 12; i++)
                        {
                            Thread.Sleep(1);
                            keyInput.Input(keyCode);
                        }
                        preve = false;
                        return 1;
                    } else
                    {
                        break;
                    }
            }
            return 0;
        }

        public void HookEnd()
        {
            UnhookWindowsHookEx(hookPtr);
            hookPtr = IntPtr.Zero;
        }
    }

    class KeyInput
    {
        [DllImport("user32.dll")]
        static extern void SendInput(int nInputs, ref INPUT pInputs, int cbsize);

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public int dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT no;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        };

        public void Input(short code)
        {
            INPUT input = new INPUT
            {
                type = 1,
                ki = new KEYBDINPUT()
                {
                    wVk = code,
                    wScan = 0,
                    dwFlags = 0,
                    time = 0,
                    dwExtraInfo = 0
                },
            };

            SendInput(1, ref input, Marshal.SizeOf(input));
        }
    }

}
