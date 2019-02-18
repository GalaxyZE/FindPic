using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;

namespace FindPic
{
    public partial class key
    {
        #region 遍历窗口
        private delegate int EnumWindowProc(IntPtr hWnd, IntPtr parameter, string m_classname);
        private IntPtr m_hWnd; // HWND if found
        public IntPtr FoundHandle
        {
            get { return m_hWnd; }
        }


        public int FindChildClassHwnd(IntPtr hwndParent, IntPtr lParam, string m_classname)
        {
            EnumWindowProc childProc = new EnumWindowProc(FindChildClassHwnd);
            IntPtr hwnd = FindWindowEx(hwndParent, IntPtr.Zero, m_classname, string.Empty);
            if (hwnd != IntPtr.Zero)
            {
                this.m_hWnd = hwnd; // found: save it
                return 0; // stop enumerating
            }
            EnumChildWindows(hwndParent, childProc, IntPtr.Zero); // recurse  redo FindChildClassHwnd
            return (int)hwnd;// keep looking
        }
        #endregion

        #region 图像识别调用方法
        // int number = 0;
        /// <summary>
        /// 截取屏幕图像
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public Bitmap fullphoto(int Width, int Height, int x, int y)
        {
            // Bitmap bitmap=null;
            //try
            //{

            IntPtr hScreenDc = CreateDC("DISPLAY", null, null, 0); // 创建桌面句柄
            IntPtr hMemDc = CreateCompatibleDC(hScreenDc); // 创建与桌面句柄相关连的内存DC
            IntPtr hBitmap = CreateCompatibleBitmap(hScreenDc, Width, Height);
            IntPtr hOldBitmap = SelectObject(hMemDc, hBitmap);
            BitBlt(hMemDc, 0, 0, Width, Height, hScreenDc, x, y, (UInt32)0xcc0020);
            IntPtr map = SelectObject(hMemDc, hOldBitmap);
            try
            {
                return Bitmap.FromHbitmap(map);
            }
            catch
            {
                return null;
            }
            finally
            {
                ReleaseDC(hBitmap, hScreenDc);
                DeleteDC(hScreenDc);//删除用过的对象
                DeleteDC(hMemDc);//删除用过的对象
                DeleteDC(hOldBitmap);
                DeleteObject(hBitmap);
            }

            //}
            //catch (Exception wx)
            //{
            //    return null;
            //}
            // number= number +1;
            // bitmap.Save("screen" + number + ".bmp");

        }

        public Bitmap fullphoto(IntPtr Hwnd, int Width, int Height, int x, int y)
        {
            IntPtr hscrdc = GetWindowDC(Hwnd);
            IntPtr hbitmap = CreateCompatibleBitmap(hscrdc, Width, Height);
            IntPtr hmemdc = CreateCompatibleDC(hscrdc);
            SelectObject(hmemdc, hbitmap);
            PrintWindow(Hwnd, hmemdc, 0);
            Bitmap bmp = Bitmap.FromHbitmap(hbitmap);
            DeleteDC(hscrdc);//删除用过的对象
            DeleteDC(hmemdc);//删除用过的对象
            DeleteObject(hbitmap);
            return bmp;
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="PathImage"></param>
        /// <returns></returns>
        public Bitmap photo(String PathImage)
        {
            Bitmap bp = new Bitmap(PathImage);
            return bp;
        }
        public Bitmap EnlargePhoto(int Width, int Height, int x, int y, int multiple)
        {
            //Bitmap ph=null;
            Bitmap bitmap;

            IntPtr hScreenDc = CreateDC("DISPLAY", null, null, 0); // 创建桌面句柄
            IntPtr hMemDc = CreateCompatibleDC(hScreenDc); // 创建与桌面句柄相关连的内存DC
            IntPtr hBitmap = CreateCompatibleBitmap(hScreenDc, Width, Height);
            IntPtr hOldBitmap = SelectObject(hMemDc, hBitmap);
            BitBlt(hMemDc, 0, 0, Width, Height, hScreenDc, x, y, (UInt32)0xcc0020);
            if (StretchBlt(hMemDc, 0, 0, Width * multiple, Height * multiple, hMemDc, 0, 0, Width, Height, (IntPtr)0xCC0020))
            {

                // ph.Save("www.bmp");
            }
            IntPtr map = SelectObject(hMemDc, hOldBitmap);
            bitmap = Bitmap.FromHbitmap(map);
            ReleaseDC(hBitmap, hScreenDc);
            DeleteDC(hScreenDc);//删除用过的对象
            DeleteDC(hMemDc);//删除用过的对象
            DeleteDC(hOldBitmap);
            DeleteObject(hBitmap);
            //bitmap.Save("sss.bmp");

            return bitmap;
        }
        /// <summary>
        /// 比较指定坐标颜色，是否为期待的
        /// </summary>
        /// <param name="PicArray">图片</param>
        /// <param name="color">颜色</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <returns></returns>
        public bool XYbit(Bitmap PicArray, int r, int g, int b, int x, int y)
        {

            Color cl = new Color();
            try
            {
                cl = PicArray.GetPixel(x, y);
                if ((int)cl.R == r && (int)cl.G == g && (int)cl.B == b)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public Color XYbit(Bitmap PicArray, int x, int y)
        {

            Color cl = new Color();
            try
            {
                cl = PicArray.GetPixel(x, y);
            }
            catch
            { return cl; }
            return cl;


        }





        //********************************************************************************************
        // 垃圾算法的图像识别
        //********************************************************************************************
        /// <summary>
        /// 返回相似图片的屏幕坐标
        /// </summary>
        /// <param name="PicArray">母图片</param>
        /// <param name="bp">子图片</param>
        /// <param name="precise">是否精确匹配</param>
        /// <param name="level">匹配等级1-5级</param>
        /// <returns></returns>
        public int[] XYbitmap(Bitmap PicArray, Bitmap bp, Boolean precise, int level)
        {
            int x = 0, y = 0;//yes = 0,num = 0,
            int[] XY = new int[2];
            int PW = PicArray.Width;
            int PH = PicArray.Height;
            int H = bp.Height;
            int W = bp.Width;
            //Color[] cl = new Color[H * W];
            //Color[] cl2 = new Color[H * W];
            Color cl = new Color();
            Color cl2 = new Color();
            for (y = 0; y < PH - H; y++)
            {
                for (x = 0; x < PW - W; x++)
                {
                    cl = PicArray.GetPixel(x, y);
                    cl2 = bp.GetPixel(0, 0);
                    if (cl == cl2 && level >= 1)
                    {
                        cl = PicArray.GetPixel(x + W - 1, y);
                        cl2 = bp.GetPixel(W - 1, 0);
                        if (cl == cl2 && level >= 2)
                        {
                            cl = PicArray.GetPixel(x, y + H - 1);
                            cl2 = bp.GetPixel(0, H - 1);
                            if (cl == cl2 && level >= 3)
                            {
                                cl = PicArray.GetPixel(x + W - 1, y + H - 1);
                                cl2 = bp.GetPixel(W - 1, H - 1);
                                if (cl == cl2 && level >= 4)
                                {
                                    cl = PicArray.GetPixel(x + (W - 1) / 2, y + (H - 1) / 2);
                                    cl2 = bp.GetPixel((W - 1) / 2, (H - 1) / 2);
                                    if (cl == cl2 && level >= 5)
                                    {
                                        XY[0] = x;
                                        XY[1] = y;
                                        Rectangle cloneRect;
                                        cloneRect = new Rectangle(x, y, W, H);
                                        PicArray = PicArray.Clone(cloneRect, PicArray.PixelFormat);//复制小块图

                                        PicArray.Save(@"image/1/" + x.ToString() + ".bmp");
                                        return XY;
                                    }
                                    XY[0] = x;
                                    XY[1] = y;
                                    return XY;
                                }
                                XY[0] = x;
                                XY[1] = y;
                                return XY;
                            }
                            XY[0] = x;
                            XY[1] = y;
                            return XY;
                        }


                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 分割图片
        /// </summary>
        /// <param name="bmpobj">图片数据</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="Width">宽</param>
        /// <param name="Height">高</param>
        /// <returns></returns>
        public Bitmap GetSplitPics(Bitmap bmpobj, int Width, int Height, int x, int y)
        {
            Rectangle cloneRect;
            cloneRect = new Rectangle(x, y, Width, Height);
            try
            {
                bmpobj = bmpobj.Clone(cloneRect, bmpobj.PixelFormat);//复制小块图
            }
            catch
            {
                return null;
            }
            //bmpobj.Save(@"image/1/" + 1+ ".bmp");
            //bmpobj.Save("screen.bmp");
            return bmpobj;
        }

        //********************************************************************************************
        //********************************************************************************************



        #endregion

        #region 对系统操作
        const uint TOKEN_ADJUST_PRIVILEGES = 0x20;
        const uint TOKEN_QUERY = 0x8;
        const uint SE_PRIVILEGE_ENABLED = 0x2;
        public enum EWX : int
        {
            EWX_FORCE = 4,//强迫终止没有响应进程
            EWX_LOGOFF = 0,//终止进程，然后注销
            EWX_REBOOT = 2,//重新引导系统
            EWX_SHUTDOWN = 1//关闭系统
        }
        LUID tmpLuid = new LUID();
        TOKEN_PRIVILEGES tkp = new TOKEN_PRIVILEGES();
        // TOKEN_PRIVILEGES tkpNewButIgnored = new TOKEN_PRIVILEGES();
        int lBufferNeeded = 0;
        /// <summary>
        /// 关闭计算机
        /// </summary>
        public void winclose()
        {
            int hdlTokenHandle = 0;
            int hdlProcessHandle = GetCurrentProcess();
            OpenProcessToken(hdlProcessHandle, (TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY), ref hdlTokenHandle);
            LookupPrivilegeValue("", "SeShutdownPrivilege", ref tmpLuid);
            tkp.PrivilegeCount = 1;
            tkp.Luid = tmpLuid;
            tkp.Attributes = SE_PRIVILEGE_ENABLED;
            AdjustTokenPrivileges(hdlTokenHandle, false, ref  tkp, Marshal.SizeOf(tkp), 0, lBufferNeeded);
            if (GetLastError() == 0)
            {
                ExitWindowsEx((int)EWX.EWX_SHUTDOWN | (int)EWX.EWX_FORCE, 0);
            }
        }


        public void PlaySound(String Path)
        {
            PlaySound(Path, 5000, SND_ASYNC | SND_FILENAME);//播放音乐 
        }

        #endregion

        #region 钩子应用调用方法
        //********************************************************************************************
        // 钩子应用
        //********************************************************************************************
        IntPtr _nextHookPtr; //记录Hook编号
        /// <summary>
        /// 执行钩子
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>

        private IntPtr MyHookProc(int code, IntPtr wparam, IntPtr lparam)
        {
            CopyMemory(ref kbdllhs, lparam, 20);      //结果就在这里了^_^ 
            int iHookCode = kbdllhs.vkCode;

            if (code < 0) return CallNextHookEx(_nextHookPtr, code, wparam, lparam);
            //System.IO.StreamWriter rs = new System.IO.StreamWriter("key.txt", true);
            //rs.WriteLine(iHookCode);
            //rs.Flush();
            //rs.Close();
            if (iHookCode == key1 && kbdllhs.flags == 0) //如果用户输入的是 b 
            {
                //winio方式，穿透力较好
                //sendwinio();
                //MykeyDown(key2);
                //System.Threading.Thread.Sleep(10);
                //MykeyUp(key2);
                Sendkey(key2, GetState(key2));//user32方式，一部分游戏不接受
                return (IntPtr)1;
                // return CallNextHookEx(_nextHookPtr, code, wparam, lparam); //返回，让后面的程序处理该消息
            }
            else
            { return CallNextHookEx(_nextHookPtr, code, wparam, lparam); }
        }

        /// <summary>
        /// 安装钩子
        /// </summary>
        public void SetHookKey(int kk1, int kk2)
        {
            key1 = kk1;
            key2 = kk2;
            if (_nextHookPtr != IntPtr.Zero) //已经勾过了
                return;
            HookProc myhookProc = new HookProc(MyHookProc); //声明一个自己的Hook实现函数的委托对象
            _nextHookPtr = SetWindowsHookEx((int)HookType.KeyboardLL, myhookProc, GetModuleHandle("sendkey.dll"), 0); //加到Hook链中
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void UnHook()
        {
            if (_nextHookPtr != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_nextHookPtr); //从Hook链中取消
                _nextHookPtr = IntPtr.Zero;
            }
        }
        #endregion

        #region 窗口操作
        /// <summary>
        /// 改变窗口大小
        /// </summary>
        /// <param name="EC">窗口名称</param>
        /// <param name="Width">宽度</param>
        /// <param name="Height">高度</param>
        /// <returns></returns>
        public bool setwinform(String EC, int Width, int Height)
        {
            int pid = 0;
            pid = (int)FindWindow(null, EC);
            if (SetWindowPos(pid, -1, 0, 0, Width, Height, SWP_NOMOVE | SWP_SHOWWINDOW))
            {
                return true;
            }
            return false;
        }

        public bool setwinform(int id, int Height, int Width)
        {

            if (SetWindowPos(id, -1, 0, 0, Width, Height, SWP_NOMOVE | SWP_SHOWWINDOW))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查找窗口句柄
        /// </summary>
        /// <returns></returns>
        public int findwin(string EC)
        {
            int pid = 0;//,id=0;
            pid = (int)FindWindow(null, EC);
            //id = (int)FindWindowEx((IntPtr)pid, IntPtr.Zero, "Edit", "");
            return pid;
        }

        /// <summary>
        /// 查找窗口句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr findwin(String classname, string EC)
        {
            IntPtr pid;//, id;
            pid = (IntPtr)FindWindow(classname, EC);
            //id = (int)FindWindowEx((IntPtr)pid, IntPtr.Zero, "Edit", "");

            return pid;
        }

        /// <summary>
        /// 获取窗口信息x,y坐标和宽，高
        /// </summary>
        /// <param name="pid">进程pid</param>
        /// <returns></returns>
        public WinFromXY findform(String EC)
        {
            int pid = 0;
            WinFromXY wf = new WinFromXY();
            pid = (int)FindWindow(null, EC);
            //uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //if (GetWindowPlacement((int)hProcess,ref lpwndpl))
            //{

            //    GetWindowRect((int)hProcess,ref rt);
            //   // rt = lpwndpl.rcNormalPosition;
            //    wf.x = rt.Left;
            //    wf.y = rt.Right;
            //    CloseHandle(hProcess);
            //    return wf;
            //}
            //CloseHandle(hProcess);
            if (GetWindowPlacement((int)pid, ref lpwndpl))
            {
                rt = lpwndpl.rcNormalPosition;
                wf.x = rt.Left;
                wf.y = rt.Right;
                wf.Height = rt.Button - rt.Right;
                wf.Width = rt.Top - rt.Left;
                // wf.Height = rt.Top;
                // wf.Width = rt.Button;
                return wf;
            }
            return wf;
        }

        /// <summary>
        /// 获取窗口信息x,y坐标和宽，高
        /// </summary>
        /// <param name="pid">进程pid</param>
        /// <returns></returns>
        public WinFromXY findform(int ECid)
        {
            WinFromXY wf = new WinFromXY();
            //uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //if (GetWindowPlacement((int)hProcess,ref lpwndpl))
            //{

            //    GetWindowRect((int)hProcess,ref rt);
            //   // rt = lpwndpl.rcNormalPosition;
            //    wf.x = rt.Left;
            //    wf.y = rt.Right;
            //    CloseHandle(hProcess);
            //    return wf;
            //}
            //CloseHandle(hProcess);
            if (GetWindowPlacement((int)ECid, ref lpwndpl))
            {
                rt = lpwndpl.rcNormalPosition;
                wf.x = rt.Left;
                wf.y = rt.Right;
                wf.Height = rt.Button - rt.Right;
                wf.Width = rt.Top - rt.Left;
                // wf.Height = rt.Top;
                // wf.Width = rt.Button;
                return wf;
            }
            return wf;
        }

        /// <summary>
        /// 使指定窗口在屏幕最上方
        /// </summary>
        /// <param name="EC">窗口名称</param>
        public void showform(String EC)
        {
            int pid = 0;
            //  bool bb = false;
            //  uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //这个函数用来置顶显示,参数hwnd为窗口句柄 nCmdShow.SW_SHOWMINNOACTIVE 
            // bb = ForceForegroundWindow((int)pid);
            pid = (int)FindWindow(null, EC);
            //SetForegroundWindow((int)pid);
            ShowWindow((int)pid, nCmdShow.SW_SHOWMINNOACTIVE);
            // IntPtr hScreenDc = CreateDC("DISPLAY", null, null, 0);
            //if (SetWindowPos((int)pid, -1, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW))
            //{

            //}

            // CloseHandle(hProcess);
        }

        public void showform(IntPtr id)
        {
            //  uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //这个函数用来置顶显示,参数hwnd为窗口句柄 nCmdShow.SW_SHOWMINNOACTIVE 
            // bb = ForceForegroundWindow((int)pid);
            SetForegroundWindow((int)id);
            // ShowWindow((int)id, nCmdShow.SW_SHOWMINNOACTIVE);
            // IntPtr hScreenDc = CreateDC("DISPLAY", null, null, 0);
            SetWindowPos((int)id, -10, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            //{
            //}
            // CloseHandle(hProcess);
        }

        /// <summary>
        /// 更改指定窗体标题栏
        /// </summary>
        /// <param name="EC">窗口名</param>
        /// <param name="Text">更改名</param>
        /// <returns></returns>
        public bool winText(String EC, String Text)
        {
            IntPtr pid = FindWindow(null, EC);
            IntPtr t = Marshal.StringToHGlobalAnsi(Text);
            SetWindowTextA(pid, t);
            return true;
        }

        /// <summary>
        /// 更改指定窗体标题栏
        /// </summary>
        /// <param name="EC">窗口名</param>
        /// <param name="Text">更改名</param>
        /// <returns></returns>
        public bool winText(IntPtr pid, String Text)
        {
            IntPtr t = Marshal.StringToHGlobalAnsi(Text);
            return SetWindowTextA(pid, t);
        }

        public int GW_CHILD = 5;
        public int GW_HWNDNEXT = 2;
        /// <summary>
        /// 根据关键字获取窗口句柄
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public int findwindos(String keyword)
        {
            int hWndPrevious = GetWindow(GetDesktopWindow(), GW_CHILD);
            while (IsWindow(hWndPrevious))
            {
                int i = GetWindowTextLength(hWndPrevious);
                StringBuilder szHello = new StringBuilder(i);
                GetWindowText(hWndPrevious, szHello, i);//获取窗口标题
                //这里我的窗口中只有下面几个字是不变的
                //if (hWndPrevious == 1641362)
                if (szHello.ToString().Contains(keyword))
                {
                    return hWndPrevious;
                    //匹配，这时hWndPrevious就是所要找的窗口的句柄
                }
                hWndPrevious = GetWindow((IntPtr)hWndPrevious, GW_HWNDNEXT);
            }
            return 0;
        }
        #endregion

        #region 内存操作
        /// <summary>
        /// 获取进程pid
        /// </summary>
        /// <param name="name">进程名</param>
        /// <returns></returns>
        public int pid(String name)
        {
            try
            {
                ObjectQuery oQuery = new ObjectQuery("select * from Win32_Process where Name='" + name + "'");
                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
                ManagementObjectCollection oReturnCollection = oSearcher.Get();

                string pid = "";
                // string cmdLine;
                StringBuilder sb = new StringBuilder();
                foreach (ManagementObject oReturn in oReturnCollection)
                {
                    pid = oReturn.GetPropertyValue("ProcessId").ToString();
                    //cmdLine = (string)oReturn.GetPropertyValue("CommandLine");

                    //string pattern = "-ap \"(.*)\"";
                    //Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                    // Match match = regex.Match(cmdLine);
                    //string appPoolName = match.Groups[1].ToString();
                    //sb.AppendFormat("W3WP.exe PID: {0}   AppPoolId:{1}\r\n", pid, appPoolName);
                }
                return Convert.ToInt32(pid);
            }
            catch
            { return 0; }

        }
        public int pid(int id)
        {
            int pid = 0;
            GetWindowThreadProcessId(id, out pid);
            return pid;
        }

        //public String getread(String QEC,String EC, IntPtr dizhi, uint size)
        //{
        //    Byte bt = new Byte();
        //    IntPtr id=FindWindow(QEC, EC);
        //    uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid(id));
        //    IntPtr fanhui = new IntPtr();
        //    String gg = null;
        //    if (hProcess == 0)
        //    {
        //       // gg = ReadProcessMemory(hProcess, dizhi, fanhui, size, 0);
        //       // CloseHandle(hProcess);


        //    }
        //    return gg;
        //}

        /// <summary>
        /// 读取内存值
        /// </summary>
        /// <param name="pid">进程pid</param>
        /// <param name="EC">""随便写一个</param>
        /// <param name="dizhi">内存地址</param>
        /// <param name="size">写4</param>
        /// <returns></returns>
        public int getread(int pid, int dizhi)
        {
            byte[] vBuffer = new byte[4];
            // IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0); // 得到缓冲区的地址 
            int vBytesAddress = new int();
            // uint vNumberOfBytesRead = 0;
            //Byte bt = new Byte();
            //IntPtr id = FindWindow(QEC, EC);
            uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //pid(0);
            //IntPtr fanhui = new IntPtr();
            // String gg = null;
            //if (hProcess == 0)
            //{
            if (ReadProcessMemory(hProcess, dizhi, out vBytesAddress, (uint)vBuffer.Length, 0))
            {
                CloseHandle(hProcess);
            }
            else
            {
                CloseHandle(hProcess);
            }

            // }
            // int vInt = Marshal.ReadInt32(vBytesAddress);
            return vBytesAddress;
        }

        /// <summary>
        /// 读取内存值
        /// </summary>
        /// <param name="pid">进程pid</param>
        /// <param name="EC">""随便写一个</param>
        /// <param name="dizhi">内存地址</param>
        /// <param name="size">写4</param>
        /// <returns></returns>
        public int getread(int pid, int dizhi, int bytes)
        {
            byte[] vBuffer = new byte[bytes];
            // IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0); // 得到缓冲区的地址 
            int vBytesAddress = new int();
            //uint vNumberOfBytesRead = 0;
            // Byte bt = new Byte();
            //IntPtr id = FindWindow(QEC, EC);
            uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            //pid(0);
            // IntPtr fanhui = new IntPtr();
            //String gg = null;
            //if (hProcess == 0)
            //{
            if (ReadProcessMemory(hProcess, dizhi, out vBytesAddress, (uint)vBuffer.Length, 0))
            {
                CloseHandle(hProcess);
            }
            else
            {
                CloseHandle(hProcess);
            }
            // }
            // int vInt = Marshal.ReadInt32(vBytesAddress);
            return vBytesAddress;
        }

        /// <summary>
        /// 读取内存值
        /// </summary>
        /// <param name="pid">进程pid</param>
        /// <param name="dizhi">内存地址</param>
        /// <param name="size">写255</param>
        /// <returns></returns>
        public String getread(int pid, int dizhi, uint size)
        {
            // char vBytesAddress = new char();
            // uint vNumberOfBytesRead = 0;
            // Byte bt = new Byte();
            uint hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
            string[] r;
            string temp = "";
            byte[] b = new byte[255];
            //char[] b = new char[255];
            try
            {
                ReadProcessMemory(hProcess, dizhi, b, 255, 0);
                //读出的byte[]要按Unicode编码为字符串
                temp = System.Text.Encoding.Unicode.GetString(b);
                //截取第一段字符串.Encoding.GetEncoding("gb2312")
                System.IO.Stream ss = new System.IO.MemoryStream(b);
                //ss.Read(b, 0, 255);
                System.IO.StreamReader rs = new System.IO.StreamReader(ss, System.Text.Encoding.GetEncoding("gb2312"));
                temp = rs.ReadToEnd();
                r = temp.Split('\0');
                // System.Text.Encoding.GetEncoding("gb2312");
                CloseHandle(hProcess);
                return r[0];
            }
            catch
            {
                return "error";
            }
        }
        #endregion

        #region 执行CALL
        private int MEM_COMMIT = 0x1000;
        private int PAGE_EXECUTE_READWRITE = 0x40;
        private int MEM_RELEASE = 0x8000;
        /// <summary>
        /// 执行反汇编CALL
        /// </summary>
        /// <param name="proc">CALL机器码</param>
        /// <param name="pid">进程pid</param>
        /// <returns></returns>
        public bool RunCall(byte[] proc, int pid)
        {
            try
            {
                IntPtr PinballHandle = (IntPtr)OpenProcess(PROCESS_ALL_ACCESS, false, pid);

                //第２步，在进程中申请空间
                int ThreadAdd = (int)VirtualAllocEx(PinballHandle, 0, proc.Length, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                //第３步，在申请的空间中,写如要执行的代码
                WriteProcessMemory(PinballHandle, (IntPtr)ThreadAdd, proc, (UInt32)proc.Length, (IntPtr)0); //写入函数地址
                //第4步，调用远程线程
                uint threadId = 0;//为了最后一个破参数，随便定义个变量
                IntPtr hThread = (IntPtr)CreateRemoteThread((int)PinballHandle, 0, 0, ThreadAdd, (IntPtr)0, 0, ref threadId); //创建远程线程
                //第5步，等待线程结束
                WaitForSingleObject(hThread, (IntPtr)0xFFFFFFFF);//等待线程结束
                //第6步，释放申请的地址
                VirtualFreeEx(PinballHandle, (IntPtr)ThreadAdd, 0, (uint)MEM_RELEASE); //释放申请的地址
                CloseHandle((uint)hThread);
                CloseHandle((uint)PinballHandle); //关闭打开的句柄
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 机器码转换byte[]
        /// </summary>
        /// <param name="HEX">CALL机器码</param>
        /// <returns> 返回byte[]</returns>
        public static byte[] getBytes(string HEX)
        {
            byte[] bytes = new byte[HEX.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(Int32.Parse(HEX.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            return bytes;
        }
        #endregion

        #region 拦截封包
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public IntPtr AllocationProtect;
            public IntPtr RegionSize;
            public IntPtr State;
            public IntPtr Protect;
            public IntPtr Type;
        }

        unsafe int MyRecv(int TSocket, object Buf, int len, int flags)
        {
            int ret = 0;

            IntPtr dwOldProtect = IntPtr.Zero;
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();

            VirtualQuery(pfnMsgBox, out mbi, sizeof(long));
            VirtualProtect(pfnMsgBox, 8, PAGE_READWRITE, dwOldProtect);
            int hdwd = findwin(_windowName);
            IntPtr PID = (IntPtr)pid(hdwd);
            PID = (IntPtr)OpenProcess(PROCESS_ALL_ACCESS, false, (int)PID);
            // 写入原来的执行代码
            WriteProcessMemory(PID,
                pfnMsgBox,
                add_old,
                sizeof(int) * 2, IntPtr.Zero);
            VirtualProtect(pfnMsgBox, 8, mbi.Protect, IntPtr.Zero);

            return ret;

        }

        unsafe int MySend(int TSocket, object Buf, int len, int flags)
        {
            int ret = 0;

            IntPtr dwOldProtect = IntPtr.Zero;
            MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();

            VirtualQuery(pfnMsgBox, out mbi, sizeof(long));
            VirtualProtect(pfnMsgBox, 8, PAGE_READWRITE, dwOldProtect);

            // 写入原来的执行代码
            WriteProcessMemory((IntPtr)GetCurrentProcess(),
                pfnMsgBox,
                add_old,
                sizeof(int) * 2, IntPtr.Zero);
            VirtualProtect(pfnMsgBox, 8, mbi.Protect, IntPtr.Zero);

            return ret;
        }
        delegate int GetSend(int TSocket, object Buf, int len, int flags);
        delegate int GetRecv(int TSocket, object Buf, int len, int flags);

        static byte[] add_old = new byte[8] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xE0, 0x00 };
        static byte[] addr_new = new byte[8] { 0xB8, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xE0, 0x00 }; //第2，3，4，5是需要手工调整的（重要的步骤）
        static IntPtr pfnMsgBox = (IntPtr)0; //API函数地址
        static IntPtr PAGE_READWRITE = (IntPtr)0x04;
        int CurrentProcess = 0;
        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr lparam;
            public IntPtr wparam;
            public int message;
            public IntPtr hwnd;
        }

        delegate IntPtr C4HookProc(int code, IntPtr wparam, ref CWPSTRUCT cwp);
        private unsafe IntPtr GetHookProc(int code, IntPtr wparam, ref CWPSTRUCT cwp)
        {


            if (CurrentProcess == 0)
            {
                CurrentProcess = GetCurrentProcess();
                int hdwd = findwin(_windowName);
                IntPtr PID = (IntPtr)pid(hdwd);
                PID = (IntPtr)OpenProcess(PROCESS_ALL_ACCESS, false, (int)PID);
                hdwd = (int)PID;
                // if (hdwd == CurrentProcess || PID ==(IntPtr) CurrentProcess)
                //{
                MEMORY_BASIC_INFORMATION mbi = new MEMORY_BASIC_INFORMATION();
                IntPtr dwOldProtect = IntPtr.Zero;
                pfnMsgBox = GetProcAddress(LoadLibrary("ws2_32.dll"), "send");
                VirtualQuery(pfnMsgBox, out mbi, 255);
                VirtualProtect(pfnMsgBox, 8, PAGE_READWRITE, dwOldProtect);



                ReadProcessMemory(PID, pfnMsgBox, add_old, sizeof(uint) * 2, IntPtr.Zero);
                GetSend mb = new GetSend(MySend);
                IntPtr new_add = Marshal.GetFunctionPointerForDelegate(mb);
                byte[] b = BitConverter.GetBytes((int)new_add);
                addr_new[1] = b[0];
                addr_new[2] = b[1];
                addr_new[3] = b[2];
                addr_new[4] = b[3];
                WriteProcessMemory(PID, pfnMsgBox, addr_new, sizeof(uint) * 2, IntPtr.Zero);
                // VirtualProtect(pfnMsgBox, 8, mbi.Protect, IntPtr.Zero);
                VirtualProtect(pfnMsgBox, 8, PAGE_READWRITE, dwOldProtect);

            }
            //当调用这个函数的时候就跳到我的函数上面了
            //  }
            return CallNextHookEx(_nextHookPtr, code, wparam, cwp);

            // return (IntPtr)0;



        }
        String _windowName = "";
        public void SetHook(String windowName)
        {
            //_windowName = windowName;
            //if (_nextHookPtr != IntPtr.Zero) //已经勾过了
            //    return;
            //myhookProc = new C4HookProc(GetHookProc); //声明一个自己的Hook实现函数的委托对象
            //int hdwd = findwin(_windowName);
            //int PID = 0;
            //PID = GetWindowThreadProcessId(hdwd, out PID);
            //_nextHookPtr = SetWindowsHookEx((int)HookType.CallWndProc, myhookProc, GetModuleHandle("sendkey.dll"), (int)PID); //加到Hook链中  
        }

        //            bool CreateRemoteDll(String DllFullPath,   IntPtr dwRemoteProcessId)
        //{


        //    HANDLE hToken;
        //    if ( OpenProcessToken(GetCurrentProcess(),TOKEN_ADJUST_PRIVILEGES,&hToken) )
        // {
        //        TOKEN_PRIVILEGES tkp;

        //        LookupPrivilegeValue( NULL,SE_DEBUG_NAME,&tkp.Privileges[0].Luid );//修改进程权限
        //        tkp.PrivilegeCount=1;
        //        tkp.Privileges[0].Attributes=SE_PRIVILEGE_ENABLED;
        //        AdjustTokenPrivileges( hToken,FALSE,&tkp,sizeof tkp,NULL,NULL );//通知系统修改进程权限

        //    }


        //    HANDLE hRemoteProcess;

        //    //打开远程线程
        //    if( (hRemoteProcess = OpenProcess( PROCESS_CREATE_THREAD |    //允许远程创建线程
        //                            PROCESS_VM_OPERATION |                //允许远程VM操作
        //                            PROCESS_VM_WRITE,                    //允许远程VM写
        //                            FALSE, dwRemoteProcessId ) )== NULL )
        //    {
        //        AfxMessageBox("OpenProcess Error!");
        //        return FALSE;
        //    }

        //    char *pszLibFileRemote;
        //    //在远程进程的内存地址空间分配DLL文件名缓冲区
        //    pszLibFileRemote = (char *) VirtualAllocEx( hRemoteProcess, NULL, lstrlen(DllFullPath)+1, 
        //                            MEM_COMMIT, PAGE_READWRITE);
        //    if(pszLibFileRemote == NULL)
        //    {
        //        AfxMessageBox("VirtualAllocEx error! ");
        //        return FALSE;
        //    }

        //    //将DLL的路径名复制到远程进程的内存空间
        //    if( WriteProcessMemory(hRemoteProcess,
        //                pszLibFileRemote, (void *) DllFullPath, lstrlen(DllFullPath)+1, NULL) == 0)
        //   {
        //        AfxMessageBox("WriteProcessMemory Error");
        //        return FALSE;
        //    }

        //    //计算LoadLibraryA的入口地址
        //    PTHREAD_START_ROUTINE pfnStartAddr = (PTHREAD_START_ROUTINE)
        //            GetProcAddress(GetModuleHandle(TEXT("Kernel32")), "LoadLibraryA");

        //    if(pfnStartAddr == NULL)
        //   {
        //        AfxMessageBox("GetProcAddress Error");
        //        return FALSE;
        //    }

        //    HANDLE hRemoteThread;
        //    if( (hRemoteThread = CreateRemoteThread( hRemoteProcess, NULL, 0, 
        //                pfnStartAddr, pszLibFileRemote, 0, NULL) ) == NULL)
        //    {
        //        AfxMessageBox("CreateRemoteThread Error");
        //        return false;
        //    }

        //    return true;
        //} 
        #endregion

        #region 键盘和鼠标模拟操作
        /// <summary>
        /// POST
        /// </summary>
        /// <param name="id">句柄</param>
        /// <param name="wMsG">键盘按下事件</param>
        /// <param name="three">键盘虚拟吗</param>
        /// <param name="four">键盘扫描码</param>
        /// <returns></returns>
        public bool PostMessageKEY(IntPtr id, int wMsG, int three, int four)
        {
            PostMessage(id, wMsG, three, four);
            return true;
        }

        /// <summary>
        /// 发送系统消息模拟键盘鼠标
        /// </summary>
        /// <param name="id">句柄</param>
        /// <param name="wMsG">第2参数标识（1）比如：键盘按下WM_KEYDOWN =0x0100 ,WM_KEYUP =0x0101 ,</param>
        /// <param name="three">第3参数</param>
        /// <returns></returns>
        public bool sendMessageKEY(IntPtr id, int wMsG, int three, int four)
        {
            SendMessage(id, wMsG, three, four);
            return true;
        }

        public bool sendMessageKEY(IntPtr id, int three, Char[] four)
        {
            //IntPtr bntHwd = FindWindowEx(id,0,null,
            //IntPtr hwdText= FindWindowEx(hwdParent, bntHwd, null, null);
            SendMessage(id, (int)wMsG.WM_SETTEXT, three, four);
            return true;
        }

        /// <summary>
        /// 获取键盘状态
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool GetState(int Key)
        {
            return (GetKeyState((int)Key) == 1);
        }

        /// <summary>
        /// 获取键盘状态
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool GetState(VirtualKeys Key)
        {
            return (GetKeyState((int)Key) == 1);
        }

        public bool GetState2(int Key)
        {
            return (GetKeyState((int)Key) == 1);
        }

        /// <summary>
        /// 发送键盘事件
        /// </summary>
        /// <returns></returns>
        public void Sendkey(VirtualKeys Key, bool State)
        {
            if (State != GetState(Key))
            {
                byte a = MapVirtualKey((byte)Key, 0);
                keybd_event((byte)Key, MapVirtualKey((byte)Key, 0), 0, 0);
                System.Threading.Thread.Sleep(10);
                keybd_event((byte)Key, MapVirtualKey((byte)Key, 0), KEYEVENTF_KEYUP, 0);
            }

        }

        /// <summary>
        /// 发送键盘事件
        /// </summary>
        /// <returns></returns>
        public void Sendkey(int Key, bool State)
        {
            if (State == GetState2(Key))
            {
                byte a = MapVirtualKey((byte)Key, 0);
                keybd_event((byte)Key, a, 0, 0);
                System.Threading.Thread.Sleep(10);
                keybd_event((byte)Key, a, KEYEVENTF_KEYUP, 0);
            }
        }

        /// <summary>
        /// 初始化winio
        /// </summary>
        public void sendwinio()
        {
            if (InitializeWinIo())
            {
                KBCWait4IBE();
            }
        }

        private void KBCWait4IBE() //等待键盘缓冲区为空
        {
            //int[] dwVal = new int[] { 0 };
            int dwVal = 0;
            do
            {
                //这句表示从&H64端口读取一个字节并把读出的数据放到变量dwVal中
                //GetPortVal函数的用法是GetPortVal 端口号,存放读出数据的变量,读入的长度
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }

        /// <summary>
        /// 模拟键盘标按下
        /// </summary>
        /// <param name="vKeyCoad">键盘编码</param>
        public void MykeyDown(int vKeyCoad)
        {
            int btScancode = 0;

            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            // btScancode = vKeyCoad;

            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);// '发送键盘写入命令
            //SetPortVal函数用于向端口写入数据，它的用法是SetPortVal 端口号,欲写入的数据，写入数据的长度
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0xe2, 1);// '写入按键信息,按下键
            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);// '发送键盘写入命令
            //SetPortVal函数用于向端口写入数据，它的用法是SetPortVal 端口号,欲写入的数据，写入数据的长度
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);// '写入按键信息,按下键

        }

        /// <summary>
        /// 模拟键盘弹出
        /// </summary>
        /// <param name="vKeyCoad">键盘编码</param>
        public void MykeyUp(int vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            //btScancode = vKeyCoad;

            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1); //'发送键盘写入命令
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0xe0, 1);// '写入按键信息，释放键
            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1); //'发送键盘写入命令
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);// '写入按键信息，释放键
        }

        /// <summary>
        /// 模拟鼠标按下
        /// </summary>
        /// <param name="vKeyCoad">鼠标编码</param>
        public void MyMouseDown(int vKeyCoad)
        {
            int btScancode = 0;

            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            //btScancode = vKeyCoad;

            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1);// '发送键盘写入命令
            //SetPortVal函数用于向端口写入数据，它的用法是SetPortVal 端口号,欲写入的数据，写入数据的长度
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// '写入按键信息,按下键

        }

        /// <summary>
        /// 模拟鼠标弹出
        /// </summary>
        /// <param name="vKeyCoad">鼠标编码</param>
        public void MyMouseUp(int vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((byte)vKeyCoad, 0);
            // btScancode = vKeyCoad;

            KBCWait4IBE(); // '发送数据前应该先等待键盘缓冲区为空
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD3, 1); //'发送键盘写入命令
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);// '写入按键信息，释放键
        }
        #endregion
    }
}
