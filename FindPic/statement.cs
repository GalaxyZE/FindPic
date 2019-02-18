using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FindPic
{
    partial class key
    {
        //********************************************************************************************
        //常量定义
        #region
        public int key1 = 0, key2 = 0;
        const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        const uint KEYEVENTF_KEYUP = 0x2;
        const int SWP_NOSIZE = 0x1; const int SWP_NOMOVE = 0x2; const int SWP_NOZORDER = 0x4; const int SWP_SHOWWINDOW = 0x4;
       // private readonly int MOUSEEVENTF_LEFTDOWN = 0x2;
        //private readonly int MOUSEEVENTF_LEFTUP = 0x4;
        const uint KBC_KEY_CMD = 0x64;
        const uint KBC_KEY_DATA = 0x60;
        private KBDLLHOOKSTRUCT kbdllhs;
        private WINDOWPLACEMENT lpwndpl;
       // private POINTAPI pt;
        private Rect rt;
        private IntPtr iHookHandle = IntPtr.Zero;
       // private GCHandle _hookProcHandle;
        public const int SND_FILENAME = 0x00020000;
        public const int SND_ASYNC = 0x0001;
        //********************************************************************************************
        //钩子事件委托
        public unsafe delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        //********************************************************************************************
        //********************************************************************************************
        //api用到的数据类型
        private const int WH_KEYBOARD = 13;
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT { public int vkCode; public int scanCode; public int flags; public int time; public int dwExtraInfo; }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Right;
            public int Top;
            public int Button;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public Rect rcNormalPosition;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct WinFromXY
        {
            public int x;
            public int y;
            public int Width;
            public int Height;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LUID
        {

            public uint LowPart;

            public uint HighPart;

        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TOKEN_PRIVILEGES
        {

            public uint PrivilegeCount;

            public LUID Luid;

            public uint Attributes;

        };
        //********************************************************************************************

        /// <summary>
        /// 消息值：按下鼠标左键
        /// </summary>
        public const int Message_Mouse_LeftButon_Down = 513;

        /// <summary>
        /// 消息值：松开鼠标左键
        /// </summary>
        public const int Message_Mouse_LeftButon_Up = 514;

        /// <summary>
        /// 消息值：获取窗口文本
        /// </summary>
        public const int Message_Window_GetText = 13;


        #endregion
        //********************************************************************************************

        //********************************************************************************************
        //api声明
        #region
        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound, int hmod, int fdwSound);//播放windows音乐，重载
       
        [DllImport("user32.dll", EntryPoint = "GetClassName")]
         static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
         static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        //IMPORTANT : LPARAM  must be a pointer (InterPtr) in VS2005, otherwise an exception will be thrown
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);


        //the callback function for the EnumChildWindows


        //********************************************************************************************
        //用于系统关机等权限操作
        [DllImport("user32.dll", EntryPoint = "ExitWindowsEx", CharSet = CharSet.Auto)]
        private static extern int ExitWindowsEx(int uFlags, int dwReserved);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetCurrentProcess();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetLastError();
        [DllImport("advapi32", CharSet = CharSet.Auto)]
        private static extern int OpenProcessToken(int ProcessHandle, uint DesiredAccess, ref int TokenHandle);
        [DllImport("advapi32", CharSet = CharSet.Auto)]
        private static extern int LookupPrivilegeValue(String lpSystemName, String lpName, ref  LUID lpLuid);
        [DllImport("advapi32", CharSet = CharSet.Auto)]
        private static extern int AdjustTokenPrivileges(int TokenHandle, bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, int PreviousState, int ReturnLength);

        //********************************************************************************************

        [DllImport("activ.dll", CharSet = CharSet.Auto)]
         static extern bool ForceForegroundWindow(int hwnd);
        //********************************************************************************************
        //主要用于更改程序标题
        //这个函数用来置顶显示,参数hwnd为窗口句柄 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern bool SetWindowTextA(IntPtr hwn, IntPtr lpString);
        //********************************************************************************************
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern void SetForegroundWindow(int hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern bool BringWindowToTop(IntPtr hwnd);

        //这个函数用来显示窗口,参数hwnd为窗口句柄,nCmdShow是显示类型的枚举 
        [DllImport("user32.dll")]
         static extern bool ShowWindow(int hWnd, nCmdShow nCmdShow);
        [DllImport("user32.dll")]
         static extern bool SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);
        //得到窗体句柄的函数,FindWindow函数用来返回符合指定的类名( ClassName )和窗口名( WindowTitle )的窗口句柄
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern IntPtr FindWindow(
        string lpClassName, //   pointer   to   class   name   
        string lpWindowName   //   pointer   to   window   name   
        );
        /// <summary>
        /// 查找子窗口
        /// </summary>
        /// <param name="hWnd_Father">父窗口的句柄</param>
        /// <param name="hWnd_PreChild">上一个兄弟窗口</param>
        /// <param name="lpszclass">窗口类</param>
        /// <param name="lpszwindows">窗口标题</param>
        /// <returns>窗口的句柄（如果查找失败将返回0）</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern IntPtr FindWindowEx(IntPtr hWnd_Father, IntPtr hWnd_PreChild, string lpszclass, string lpszwindows);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowPlacement(int hwnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(int id, out int pid);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(int hwnd, ref Rect lpwndpl);

        [DllImport("user32.dll")]//该函数返回与指定窗口有特定关系(如Z序或所有者)的窗口句柄
         static extern int GetWindow(IntPtr hWnd, int nCmd);
        [DllImport("user32.dll")]//当前桌面句柄
         static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]//检查进程窗口是否存在
         static extern bool IsWindow(int hWnd);
        [DllImport("user32.dll")]//获取窗口标题长度 
         static extern int GetWindowTextLength(int hWnd);







        //********************************************************************************************
        //读取进程内存的函数
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, int lpBaseAddress,
          out int lpBuffer, uint nSize, int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, int lpBaseAddress,
          char[] lpBuffer, uint nSize, uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, int lpBaseAddress,
          string lpBuffer, uint nSize, uint lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
         static extern bool ReadProcessMemory(
            uint hProcess,
            int lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            uint lpNumberOfBytesRead
        );
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(
           uint hProcess,
           int lpBaseAddress,
           out int [,] lpBuffer,
           int nSize,
           uint lpNumberOfBytesRead
       );
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(
           uint hProcess,
           int lpBaseAddress,
           out int[] lpBuffer,
           int nSize,
           uint lpNumberOfBytesRead
       );
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(
           uint hProcess,
           int lpBaseAddress,
            int[] lpBuffer,
           int nSize,
           uint lpNumberOfBytesRead
       );
        [DllImport("Kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, byte[] lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesWritten);
       
        [DllImport("Kernel32.dll")]
        static extern int VirtualQuery(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
        [DllImport("Kernel32.dll")]
        static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, IntPtr flNewProtect, IntPtr lpflOldProtect);

        [DllImport("kernel32.dll")]
         static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, GetSend gs, UInt32 nSize, IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern int WriteProcessMemory(IntPtr hProcess,IntPtr lpBaseAddress, GetRecv gr, UInt32 nSize, IntPtr lpNumberOfBytesWritten);
        
        [DllImport("Kernel32.dll")]
         static extern System.UInt32 VirtualAllocEx(
        IntPtr hProcess,
        int lpAddress,
        int dwSize,
        int flAllocationType,
        int flProtect
        );
        [DllImport("kernel32.dll")]
         static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            UInt32 dwSize,
            UInt32 dwFreeType
            );
        [DllImport("kernel32.dll")]
         static extern IntPtr CreateRemoteThread(
          int hProcess,
          int lpThreadAttributes,
          int dwStackSize,
          int lpStartAddress,
          IntPtr param,
          int dwCreationFlags,
          ref uint lpThreadId
        );
        [DllImport("kernel32.dll")]
         static extern UInt32 WaitForSingleObject(
          IntPtr hHandle,
          IntPtr dwMilliseconds
          );
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenFileMapping(
                                              IntPtr dwDesiredAccess,
                                              bool bInheritHandle,
                                              String lpName
                                            );
        [DllImport("kernel32.dll")]
        static extern int MapViewOfFile(
                                          IntPtr hFileMappingObject,
                                          int dwDesiredAccess,
                                          int dwFileOffsetHigh,
                                          int dwFileOffsetLow,
                                          int dwNumberOfBytesToMap
                                        );
        [DllImport("kernel32.dll")]
        static extern bool UnmapViewOfFile(
                                            IntPtr lpBaseAddress
                                            );


        //********************************************************************************************
        // public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, out char lpBuffer, int nSize, int lpNumberOfBytesWritten);


        //得到目标进程句柄的函数
        [DllImport("kernel32.dll")]
         static extern uint OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern void CloseHandle
        (
         uint hObject //Handle to object
        );
        //鼠标事件声明
        [DllImport("user32.dll")]
        static extern bool setcursorpos(int x, int y);
        [DllImport("user32.dll")]
        static extern void mouse_event(mouseeventflag flags, int dx, int dy, uint data, UIntPtr extrainfo);
        //键盘事件声明
        [DllImport("user32.dll")]
        static extern byte MapVirtualKey(byte wCode, int wMap);
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        //键盘事件声明winio
        [DllImport("winio.dll")]
         static extern bool InitializeWinIo();
        [DllImport("winio.dll")]
         static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("winio.dll")]
         static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("winio.dll")]
         static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("winio.dll")]
         static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("winio.dll")]
         static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("winio.dll")]
         static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("winio.dll")]
         static extern void ShutdownWinIo();

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        //全局键盘钩子

        //第一个参数:指定钩子的类型，有WH_MOUSE、WH_KEYBOARD等十多种(具体参见MSDN)
        //第二个参数:标识钩子函数的入口地址
        //第三个参数:钩子函数所在模块的句柄；
        //第四个参数:钩子相关函数的ID用以指定想让钩子去钩哪个线程，为0时则拦截整个系统的消息。
        //安装在钩子链表中的钩子子程
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern IntPtr SetWindowsHookEx(int hookid, [MarshalAs(UnmanagedType.FunctionPtr)] HookProc lpfn, IntPtr hinst, int threadid);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowsHookEx(int hookid, [MarshalAs(UnmanagedType.FunctionPtr)] C4HookProc lpfn, IntPtr hinst, int threadid);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetWindowsHookEx(int hookid, IntPtr  lpfn, IntPtr hinst, int threadid);

        //移除由SetWindowsHookEx方法安装在钩子链表中的钩子子程
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
         static extern bool UnhookWindowsHookEx(IntPtr hhook);
        //对一个事件处理的hook可能有多个，它们成链状，使用CallNextHookEx一级一级地调用。简单解释过来就是“调用下一个HOOK”
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
         static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wparam, CWPSTRUCT cwp);
        //发送系统消息
        [DllImport("user32.dll")]
         static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //发送系统消息
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr hWnd, int msg, int lParam, Char[] wParam);
        //取得自身进程的模块地址，句柄
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        //函数功能描述:将一块内存的数据从一个位置复制到另一个位置
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
         static extern void CopyMemory(ref KBDLLHOOKSTRUCT Source, IntPtr Destination, int Length);
        //函数功能描述:将一块内存的数据从一个位置复制到另一个位置
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(KBDLLHOOKSTRUCT Source, IntPtr Destination, int Length);
        //取得当前线程编号的API
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll")]
        static extern bool GetMessage(IntPtr lpMsg,
                                          IntPtr hWnd,
                                          int wMsgFilterMin,
                                          int wMsgFilterMax
                                      );
        [DllImport("Kernel32.dll")]
        static extern IntPtr GetCurrentProcessId();
        [DllImport("Kernel32.dll")]
        static extern IntPtr LoadLibrary( String lpFileName);
        [DllImport("Kernel32.dll")]
        static extern IntPtr  GetProcAddress(IntPtr hModule,String  lpProcNam);
        [DllImport("Kernel32.dll")]
        static extern IntPtr CreateFileMapping(IntPtr hFile,
                                              int lpAttributes,
                                              int flProtect,
                                              int dwMaximumSizeHigh,
                                              int dwMaximumSizeLow,
                                              String lpName);


        //********************************************************************************************
        //获取屏幕1024*768图像
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, UInt32 dwRop);
        //创建桌面句柄
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
         static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        //创建与系统匹配的图像资源
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        //删除用过的资源
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
         static extern int DeleteDC(IntPtr hdc);
        //释放用过的句柄等资源
        [DllImport("user32.dll")]
         static extern bool ReleaseDC(
         IntPtr hwnd, IntPtr hdc
         );
        //释放用过的画笔，等图像资源
        [DllImport("gdi32.dll")]
         static extern bool DeleteObject(
          IntPtr hdc
         );
        //用于像素放大,最后一参数cc0020
        [DllImport("gdi32.dll")]
         static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, IntPtr rop);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);


        [DllImport("user32.dll")]
        public static extern bool PrintWindow(
         IntPtr hwnd,                // Window to copy,Handle to the window that will be copied.
         IntPtr hdcBlt,              // HDC to print into,Handle to the device context.
         UInt32 nFlags               // Optional flags,Specifies the drawing options. It can be one of the following values.
         );
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(
         IntPtr hwnd
         );
        //********************************************************************************************
       #endregion
        //********************************************************************************************

    }
}
