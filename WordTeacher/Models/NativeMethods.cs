using System;
using System.Runtime.InteropServices;

namespace WordTeacher.Models
{
    public static class NativeMethods
    {
        public const int WmMoving = 0x0216;
        public const int WmExitsizemove = 0x0232;

        [StructLayout(LayoutKind.Sequential)]
        //Struct for Marshalling the lParam
        public struct MoveRectangle
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    }
}
