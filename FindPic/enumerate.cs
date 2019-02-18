using System;
using System.Collections.Generic;
using System.Text;

namespace FindPic
{
    partial class key
    {
        //********************************************************************************************
        //键盘和鼠标 等 枚举
        #region

        internal enum HookType //枚举，钩子的类型
        {

            //MsgFilter     = -1,

            //JournalRecord    = 0,

            //JournalPlayback  = 1,

            Keyboard = 2,

            GetMessage = 3,

            CallWndProc = 4,

            //CBT              = 5,

            SysMsgFilter = 6,

            Mouse            = 7,

            Hardware = 8,

            //Debug            = 9,

            //Shell           = 10,

            //ForegroundIdle  = 11,

            CallWndProcRet = 12,

            KeyboardLL = 13,

            //MouseLL           = 14,

        };


        public enum wMsG : int
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WA_INACTIVE = 0,
            WA_ACTIVE = 1,
            WA_CLICKACTIVE = 2,

            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,

            WM_QUERYENDSESSION = 0x0011,
            WM_QUERYOPEN = 0x0013,
            WM_ENDSESSION = 0x0016,
            WM_QUIT = 0x0012,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_SHOWWINDOW = 0x0018,
            WM_WININICHANGE = 0x001A,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_USER   =    0x0400,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,

            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEMOVE = 0x0200,
            // 移动鼠标
            WM_LBUTTONDOWN = 0x0201,
            //按下鼠标左键
            WM_LBUTTONUP = 0x0202,
            //释放鼠标左键
            WM_LBUTTONDBLCLK = 0x0203,
            //双击鼠标左键
            WM_RBUTTONDOWN = 0x0204,
            //按下鼠标右键
            WM_RBUTTONUP = 0x0205,
            //释放鼠标右键
            WM_RBUTTONDBLCLK = 0x0206,
            //双击鼠标右键
            WM_MBUTTONDOWN = 0x0207,
            //按下鼠标中键 
            WM_MBUTTONUP = 0x0208,
            //释放鼠标中键
            WM_MBUTTONDBLCLK = 0x0209,
            //双击鼠标中键
            WM_MOUSEWHEEL = 0x020A,
        }

        /// <summary>
        /// 鼠标动作枚举
        /// </summary>
        public enum mouseeventflag : uint
        {
            move = 0x0001,
            leftdown = 0x0002,
            leftup = 0x0004,
            rightdown = 0x0008,
            rightup = 0x0010,
            middledown = 0x0020,
            middleup = 0x0040,
            xdown = 0x0080,
            xup = 0x0100,
            wheel = 0x0800,
            virtualdesk = 0x4000,
            absolute = 0x8000
        }
        /// <summary>
        /// 键盘动作枚举
        /// </summary>
        public enum VirtualKeys : byte
        {
            //VK_NUMLOCK = 0x90, //数字锁定键
            //VK_SCROLL = 0x91,  //滚动锁定
            //VK_CAPITAL = 0x14, //大小写锁定
            //VK_A = 62,         //键盘A
            VK_LBUTTON = 1,      //鼠标左键 
            VK_RBUTTON = 2,　    //鼠标右键 
            VK_CANCEL = 3,　　　 //Ctrl+Break(通常不需要处理) 
            VK_MBUTTON = 4,　　  //鼠标中键 
            VK_BACK = 8, 　　　  //Backspace 
            VK_TAB = 9,　　　　  //Tab 
            VK_CLEAR = 12,　　　 //Num Lock关闭时的数字键盘5 
            VK_RETURN = 13,　　　//Enter(或者另一个) 
            VK_SHIFT = 16,　　　 //Shift(或者另一个) 
            VK_CONTROL = 17,　　 //Ctrl(或者另一个） 
            VK_MENU = 18,　　　　//Alt(或者另一个) 
            VK_PAUSE = 19,　　　 //Pause 
            VK_CAPITAL = 20,　　 //Caps Lock 
            VK_ESC = 27,　　　//Esc 
            VK_SPACE = 32,　　　 //Spacebar 
            VK_PRIOR = 33,　　　 //Page Up 
            VK_NEXT = 34,　　　　//Page Down 
            VK_END = 35,　　　　 //End 
            VK_HOME = 36,　　　　//Home 
            VK_LEFT = 37,　　　  //左箭头 
            VK_UP = 38,　　　　  //上箭头 
            VK_RIGHT = 39,　　　 //右箭头 
            VK_DOWN = 40,　　　  //下箭头 
            VK_SELECT = 41,　　  //可选 
            VK_PRINT = 42,　　　 //可选 
            VK_EXECUTE = 43,　　 //可选 
            VK_SNAPSHOT = 44,　　//Print Screen 
            VK_INSERT = 45,　　　//Insert 
            VK_DELETE = 46,　　  //Delete 
            VK_HELP = 47,　　    //可选 
            VK_NUM0 = 48,        //0
            VK_NUM1 = 49,        //1
            VK_NUM2 = 50,        //2
            VK_NUM3 = 51,        //3
            VK_NUM4 = 52,        //4
            VK_NUM5 = 53,        //5
            VK_NUM6 = 54,        //6
            VK_NUM7 = 55,        //7
            VK_NUM8 = 56,        //8
            VK_NUM9 = 57,        //9
            VK_A = 65,           //A
            VK_B = 66,           //B
            VK_C = 67,           //C
            VK_D = 68,           //D
            VK_E = 69,           //E
            VK_F = 70,           //F
            VK_G = 71,           //G
            VK_H = 72,           //H
            VK_I = 73,           //I
            VK_J = 74,           //J
            VK_K = 75,           //K
            VK_L = 76,           //L
            VK_M = 77,           //M
            VK_N = 78,           //N
            VK_O = 79,           //O
            VK_P = 80,           //P
            VK_Q = 81,           //Q
            VK_R = 82,           //R
            VK_S = 83,           //S
            VK_T = 84,           //T
            VK_U = 85,           //U
            VK_V = 86,           //V
            VK_W = 87,           //W
            VK_X = 88,           //X
            VK_Y = 89,           //Y
            VK_Z = 90,           //Z
            VK_NUMPAD0 = 96,     //0
            VK_NUMPAD1 = 97,     //1
            VK_NUMPAD2 = 98,     //2
            VK_NUMPAD3 = 99,     //3
            VK_NUMPAD4 = 100,    //4
            VK_NUMPAD5 = 101,    //5
            VK_NUMPAD6 = 102,    //6
            VK_NUMPAD7 = 103,    //7
            VK_NUMPAD8 = 104,    //8
            VK_NUMPAD9 = 105,    //9
            VK_NULTIPLY = 106,　 //数字键盘上的* 
            VK_ADD = 107,　　　　//数字键盘上的+ 
            VK_SEPARATOR = 108,　//可选 
            VK_SUBTRACT = 109,　 //数字键盘上的- 
            VK_DECIMAL = 110,　　//数字键盘上的. 
            VK_DIVIDE = 111,　　 //数字键盘上的/
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_NUMLOCK = 144,　　//Num Lock 
            VK_SCROLL = 145 　   // Scroll Lock 
        }
        public enum nCmdShow : uint
        {
            SW_FORCEMINIMIZE = 0x0,
            SW_HIDE = 0x1,
            SW_MAXIMIZE = 0x2,
            SW_MINIMIZE = 0x3,
            SW_RESTORE = 0x4,
            SW_SHOW = 0x5,
            SW_SHOWDEFAULT = 0x6,
            SW_SHOWMAXIMIZED = 0x7,
            SW_SHOWMINIMIZED = 0x8,
            SW_SHOWMINNOACTIVE = 0x9,
            SW_SHOWNA = 0xA,
            SW_SHOWNOACTIVATE = 0xB,
            SW_SHOWNORMAL = 0xC,
            WM_CLOSE = 0x10,
        }

        public enum ScanCodes : int
        {
            ESC = 0x01,
            VK_1 = 0x02,
            VK_2 = 0x03,
            VK_3 = 0x04,
            VK_4 = 0x05,
            VK_5 = 0x06,
            VK_6 = 0x07,
            VK_7 = 0x08,
            VK_8 = 0x09,
            VK_9 = 0x0A,
            VK_0 = 0x0B,
            VK_横 = 0x0C,
            VK_等于 = 0x0D,
            BKSP = 0x0E,
            TAB = 0x0F,
            Q = 0x10,
            W = 0x11,
            E = 0x12,
            R = 0x13,
            T = 0x14,
            Y = 0x15,
            U = 0x16,
            I = 0x17,
            O = 0x18,
            P = 0x19,
            ENTER = 0x1C,

            LCTRL = 0x1D,
            RCTRL = 0x1D,
            A = 0x1E,
            S = 0x1F,
            D = 0x20,
            F = 0x21,
            G = 0x22,
            H = 0x23,
            J = 0x24,
            K = 0x25,
            L = 0x26,

            分号 = 0x27,
            引号 = 0x28,
            波折号 = 0x29,
            SHIFT = 0x2A,
            //R_ShIFT=0x2B,    
            Z = 0x2C,
            X = 0x2D,
            C = 0x2E,
            V = 0x2F,
            B = 0x30,
            N = 0x31,
            M = 0x32,
            逗号 = 0x33,
            句号 = 0x34,
            问好 = 0x35,
            //=0x35,
            R_SHIFT = 0x36,
            星_PRTSC = 0x37,
            L_ALT = 0x38,
            R_ALT = 0x38,
            SPACE = 0x39,
            CAPS = 0x3A,
            F1 = 0x3B,
            F2 = 0x3C,
            F3 = 0x3D,
            F4 = 0x3E,
            F5 = 0x3F,
            F6 = 0x40,
            F7 = 0x41,
            F8 = 0x42,
            F9 = 0x43,
            F10 = 0x44,
            F11 = 0x57,
            F12 = 0x58,
            NUM = 0x45,
            SCROLL = 0x46,
            HOME = 0x47,
            UP = 0x48,
            PGUP = 0x49,
            小键盘横 = 0x4A,
            LEFT = 0x4B,
            小键盘_5 = 0x4C,
            RIGHT = 0x4D,
            加号 = 0x4E,
            END = 0x4F,
            DOWN = 0x50,
            PGDN = 0x51,
            INS = 0x52,
            DEL = 0x53,



        }



        #endregion
        //********************************************************************************************
    }
}
