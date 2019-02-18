using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using GalaxyHelper;

namespace FindPic
{
    public partial class Form1 : Form
    {
        Bitmap 大图 = new Bitmap(@"无标题.bmp");
        Bitmap 完全对比 = new Bitmap(@"完全对比.bmp");
        Bitmap 相似度 = new Bitmap(@"相似度.bmp");
        Bitmap 透明 = new Bitmap(@"透明.bmp");
        Bitmap img_test = Properties.Resources.img_quest;
        IntPtr intptr_GBF;
        public Form1()
        {
            //避免线程冲突
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //提高程序运行优先等级
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            
            InitializeComponent();
            pictureBox1.Image = 大图;
            pictureBox2.Image = 完全对比;
            pictureBox3.Image = 透明;
            pictureBox4.Image = 相似度;
        }

        //完全对比
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Bitmap btm = (Bitmap)大图.Clone();
            Stopwatch sw = new Stopwatch();
            sw.Start(); //计时开始
            List<Point> list = BmpColor.FindPic(0, 0, 大图.Width, 大图.Height, 大图, 完全对比, 0);
            sw.Stop();   //计时结束
            label4.Text = sw.ElapsedMilliseconds + "毫秒";
            label2.Text = list.Count.ToString();
            if (list.Count > 0)
            {
                listView1.Items.Clear();
                Graphics g = Graphics.FromImage(btm);
                for (int i = 0; i < list.Count; i++)
                {
                    listView1.Items.Insert(listView1.Items.Count,
                        new ListViewItem(new string[] { 
                            i.ToString(),
                            list[i].X.ToString(), 
                            list[i].Y.ToString() 
                        }));
                    g.DrawRectangle(new Pen(Color.Red, 2), list[i].X, list[i].Y, 完全对比.Width, 完全对比.Height);
                }
            }
            pictureBox1.Image = btm;
        }

        //透明
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Bitmap btm = (Bitmap)大图.Clone();
            Stopwatch sw = new Stopwatch();
            sw.Start(); //计时开始
            List<Point> list = BmpColor.FindPic(0, 0, 大图.Width, 大图.Height, 大图, 透明, 0);
            sw.Stop();   //计时结束
            label4.Text = sw.ElapsedMilliseconds + "毫秒";
            label2.Text = list.Count.ToString();
            if (list.Count > 0)
            {
                listView1.Items.Clear();
                Graphics g = Graphics.FromImage(btm);
                for (int i = 0; i < list.Count; i++)
                {
                    listView1.Items.Insert(listView1.Items.Count,
                        new ListViewItem(new string[] { 
                            i.ToString(),
                            list[i].X.ToString(), 
                            list[i].Y.ToString() 
                        }));
                    g.DrawRectangle(new Pen(Color.Red, 2), list[i].X, list[i].Y, 透明.Width, 透明.Height);
                }
            }
            pictureBox1.Image = btm;
        }

        //相似度
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Bitmap btm = (Bitmap)大图.Clone();
            Stopwatch sw = new Stopwatch();
            sw.Start(); //计时开始
            List<Point> list = BmpColor.FindPic(0, 0, 大图.Width, 大图.Height, 大图, 相似度, 40);
            sw.Stop();   //计时结束
            label4.Text = sw.ElapsedMilliseconds + "毫秒";
            label2.Text = list.Count.ToString();
            if (list.Count > 0)
            {
                listView1.Items.Clear();
                Graphics g = Graphics.FromImage(btm);
                for (int i = 0; i < list.Count; i++)
                {
                    listView1.Items.Insert(listView1.Items.Count,
                        new ListViewItem(new string[] { 
                            i.ToString(),
                            list[i].X.ToString(), 
                            list[i].Y.ToString() 
                        }));
                    g.DrawRectangle(new Pen(Color.Red, 2), list[i].X, list[i].Y, 相似度.Width, 相似度.Height);
                }
            }
            pictureBox1.Image = btm;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start(); //计时开始
            Bitmap btm = BmpColor.CopyScreen(100, 100, Cursor.Position.X - 100 / 2, Cursor.Position.Y - 100 / 2);
            List<Point> list = BmpColor.FindColor(0, 0, btm.Width, btm.Height, btm, Color.FromArgb(80, 180, 216), 0);
            sw.Stop();   //计时结束
            label4.Text = sw.ElapsedMilliseconds + "毫秒";
            label2.Text = list.Count.ToString();
            listView1.Items.Clear();
            if (list.Count > 0)
            {
                //创建 GraphicsPath
                GraphicsPath graphicsPath = new GraphicsPath();
                Graphics g = Graphics.FromImage(btm);
                for (int i = 0; i < list.Count; i++)
                {
                    listView1.Items.Insert(listView1.Items.Count,
                        new ListViewItem(new string[] { 
                            i.ToString(),
                            list[i].X.ToString(), 
                            list[i].Y.ToString() 
                        }));
                    //将不透明点加到graphics path
                    graphicsPath.AddRectangle(new Rectangle(list[i].X, list[i].Y, 1, 1));
                    //g.DrawRectangle(new Pen(Color.Red, 1), list[i].X, list[i].Y, 2,2);
                }
                g.DrawPath(new Pen(Color.Red, 1), graphicsPath);
            }
            pictureBox1.Image = btm;
        }

        //屏幕找色
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            intptr_GBF = GalaxyHelper.Capture.FindWindow(null, "Granblue Fantasy - Google Chrome");
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            //Bitmap btm = (Bitmap)大图.Clone();
            Bitmap btm = GalaxyHelper.Capture.PrintWindow(intptr_GBF);
            btm=GalaxyHelper.Capture.ConvertTo24bpp(btm);
            Stopwatch sw = new Stopwatch();
            sw.Start(); //计时开始

            //List<Point> list = BmpColor.FindPic(0, 0, btm.Width, btm.Height, btm, img_test, 40);
            List<Point> list = BmpColor.FindPic(0, 0, btm.Width, btm.Height, btm, img_test, trackBar1.Value);
            sw.Stop();   //计时结束
            label4.Text = sw.ElapsedMilliseconds + "毫秒";
            label2.Text = list.Count.ToString();

            if (list.Count > 0)
            {
                listView1.Items.Clear();
                Graphics g = Graphics.FromImage(btm);
                for (int i = 0; i < list.Count; i++)
                {
                    listView1.Items.Insert(listView1.Items.Count,
                        new ListViewItem(new string[] {
                            i.ToString(),
                            list[i].X.ToString(),
                            list[i].Y.ToString()
                        }));
                    g.DrawRectangle(new Pen(Color.Red, 3), list[i].X, list[i].Y, img_test.Width, img_test.Height);
                }
            }

            pictureBox1.Image = btm;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label9.Text = trackBar1.Value.ToString();
        }
    }

    ///////////////////////////////////////////////////////
    public class BmpColor
    {
        /// <summary>
        /// 在大图里找小图
        /// </summary>
        /// <param name="S_bmp">大图</param>
        /// <param name="P_bmp">小图</param>
        /// <param name="similar">容错值 取值0--255，数值越高效率越低，不建议超过50</param>
        /// <returns></returns>
        public static List<Point> FindPic(int left, int top, int width, int height, Bitmap S_bmp, Bitmap P_bmp, int similar)
        {
            if (S_bmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            if (P_bmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            int S_Width = S_bmp.Width;
            int S_Height = S_bmp.Height;
            int P_Width = P_bmp.Width;
            int P_Height = P_bmp.Height;
            //取出4个角的颜色
            int px1 = P_bmp.GetPixel(0, 0).ToArgb(); //左上角
            int px2 = P_bmp.GetPixel(P_Width - 1, 0).ToArgb(); //右上角
            int px3 = P_bmp.GetPixel(0, P_Height - 1).ToArgb(); //左下角
            int px4 = P_bmp.GetPixel(P_Width - 1, P_Height - 1).ToArgb(); //右下角
            Color BackColor = P_bmp.GetPixel(0, 0); //背景色
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_Width, S_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData P_Data = P_bmp.LockBits(new Rectangle(0, 0, P_Width, P_Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            List<Point> List;
            if (px1 == px2 && px1 == px3 && px1 == px4) //如果4个角的颜色相同
            {
                //透明找图
                List = _FindPic(left, top, width, height, S_Data, P_Data, GetPixelData(P_Data, BackColor), similar);
            }
            else if (similar > 0)
            {
                //相似找图
                List = _FindPic(left, top, width, height, S_Data, P_Data, similar);
            }
            else
            {
                //全匹配找图效率最高
                List = _FindPic(left, top, width, height, S_Data, P_Data);
            }
            S_bmp.UnlockBits(S_Data);
            P_bmp.UnlockBits(P_Data);
            return List;
        }

        /// <summary>
        /// 在大图里找小图
        /// </summary>
        /// <param name="S_bmp">大图</param>
        /// <param name="P_bmp">小图</param>
        /// <param name="similar">容错值 取值0--255，数值越高效率越低，不建议超过50</param>
        /// <returns></returns>
        

        /// <summary>
        /// 全匹配找图
        /// </summary>
        /// <param name="S_Data">大图数据</param>
        /// <param name="P_Data">小图数据</param>
        /// <returns></returns>
        ///     
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData S_Data, BitmapData P_Data)
        {
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = width - P_Data.Width + 1;
            int _BreakH = height - P_Data.Height + 1;
            for (int h = top; h < _BreakH; h++)
            {
                for (int w = left; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Data.Height; y++)
                    {
                        for (int x = 0; x < P_Data.Width; x++)
                        {
                            S_ptr = (byte*)((int)S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            P_ptr = (byte*)((int)P_Iptr + P_stride * y + x * 3);
                            if (S_ptr[0] == P_ptr[0] && S_ptr[1] == P_ptr[1] && S_ptr[2] == P_ptr[2])
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false;
                                break;
                            }
                        }
                        if (!IsOk) { break; }
                    }
                    if (IsOk) { List.Add(new Point(w, h)); }
                    IsOk = false;
                }
            }
            return List;
        }

        /// <summary>
        /// 相似找图
        /// </summary>
        /// <param name="S_Data">大图数据</param>
        /// <param name="P_Data">小图数据</param>
        /// <param name="similar">误差值</param>
        /// <returns></returns>
        /// 
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData S_Data, BitmapData P_Data, int similar)
        {
            List<Point> List = new List<Point>();
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = width - P_Data.Width + 1;
            int _BreakH = height - P_Data.Height + 1;
            for (int h = top; h < _BreakH; h++)
            {
                for (int w = left; w < _BreakW; w++)
                {
                    P_ptr = (byte*)(P_Iptr);
                    for (int y = 0; y < P_Data.Height; y++)
                    {
                        for (int x = 0; x < P_Data.Width; x++)
                        {
                            S_ptr = (byte*)((int)S_Iptr + S_stride * (h + y) + (w + x) * 3);
                            P_ptr = (byte*)((int)P_Iptr + P_stride * y + x * 3);
                            if (ScanColor(S_ptr[0], S_ptr[1], S_ptr[2], P_ptr[0], P_ptr[1], P_ptr[2], similar))  //比较颜色
                            {
                                IsOk = true;
                            }
                            else
                            {
                                IsOk = false; break;
                            }
                        }
                        if (IsOk == false) { break; }
                    }
                    if (IsOk) { List.Add(new Point(w, h)); }
                    IsOk = false;
                }
            }
            return List;
        }

        


        /// <summary>
        /// 透明找图
        /// </summary>
        /// <param name="S_Data">大图数据</param>
        /// <param name="P_Data">小图数据</param>
        /// <param name="PixelData">小图中需要比较的像素数据</param>
        /// <param name="similar">误差值</param>
        /// <returns></returns>
        private static unsafe List<Point> _FindPic(int left, int top, int width, int height, BitmapData S_Data, BitmapData P_Data, int[,] PixelData, int similar)
        {
            List<Point> List = new List<Point>();
            int Len = PixelData.GetLength(0);
            int S_stride = S_Data.Stride;
            int P_stride = P_Data.Stride;
            IntPtr S_Iptr = S_Data.Scan0;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* S_ptr;
            byte* P_ptr;
            bool IsOk = false;
            int _BreakW = width - P_Data.Width + 1;
            int _BreakH = height - P_Data.Height + 1;
            for (int h = top; h < _BreakH; h++)
            {
                for (int w = left; w < _BreakW; w++)
                {
                    for (int i = 0; i < Len; i++)
                    {
                        S_ptr = (byte*)((int)S_Iptr + S_stride * (h + PixelData[i, 1]) + (w + PixelData[i, 0]) * 3);
                        P_ptr = (byte*)((int)P_Iptr + P_stride * PixelData[i, 1] + PixelData[i, 0] * 3);
                        if (ScanColor(S_ptr[0], S_ptr[1], S_ptr[2], P_ptr[0], P_ptr[1], P_ptr[2], similar))  //比较颜色
                        {
                            IsOk = true;
                        }
                        else
                        {
                            IsOk = false; break;
                        }
                    }
                    if (IsOk) { List.Add(new Point(w, h)); }
                    IsOk = false;
                }
            }
            return List;
        }

        #region 范围找色
        /// <summary>
        /// 范围找色
        /// </summary>
        /// <param name="left">起始X</param>
        /// <param name="top">起始Y</param>
        /// <param name="width">查询宽度</param>
        /// <param name="height">查询高度</param>
        /// <param name="S_bmp">图片</param>
        /// <param name="clr">要查询的颜色</param>
        /// <param name="similar">误差值0-255</param>
        /// <returns></returns>
        public static unsafe List<Point> FindColor(int left, int top, int width, int height, Bitmap S_bmp, Color clr, int similar)
        {
            if (S_bmp.PixelFormat != PixelFormat.Format24bppRgb) { throw new Exception("颜色格式只支持24位bmp"); }
            BitmapData S_Data = S_bmp.LockBits(new Rectangle(0, 0, S_bmp.Width, S_bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr _Iptr = S_Data.Scan0;
            byte* _ptr;
            List<Point> List = new List<Point>();
            for (int y = top; y < height; y++)
            {
                for (int x = left; x < width; x++)
                {
                    _ptr = (byte*)((int)_Iptr + S_Data.Stride * (y) + (x) * 3);
                    if (ScanColor(_ptr[0], _ptr[1], _ptr[2], clr.B, clr.G, clr.R, similar))
                    {
                        List.Add(new Point(x, y));
                    }
                }
            }
            S_bmp.UnlockBits(S_Data);
            return List;
        }
        #endregion

        #region 比较两个Color
        /// <summary>
        /// 比较两个 Color 
        /// </summary>
        /// <param name="similar">容错值</param>
        /// <returns></returns>
        public static bool IsColor(Color clr1, Color clr2, int similar = 0)
        {
            if (ScanColor(clr1.B, clr1.G, clr1.R, clr2.B, clr2.G, clr2.R, similar))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 屏幕截图
        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="rect">截图矩形范围</param>
        /// <returns></returns>
        public static unsafe Bitmap CopyScreen(int Width, int Height, int x, int y)
        {
            Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(Width, Height));
                g.Dispose();
            }
            System.GC.Collect();
            return bitmap;
        }
        #endregion

        #region 私有方法
        private static unsafe int[,] GetPixelData(BitmapData P_Data, Color BackColor)
        {
            byte B = BackColor.B, G = BackColor.G, R = BackColor.R;
            int Width = P_Data.Width, Height = P_Data.Height;
            int P_stride = P_Data.Stride;
            IntPtr P_Iptr = P_Data.Scan0;
            byte* P_ptr;
            int[,] PixelData = new int[Width * Height, 2];
            int i = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    P_ptr = (byte*)((int)P_Iptr + P_stride * y + x * 3);
                    if (B == P_ptr[0] & G == P_ptr[1] & R == P_ptr[2])
                    {

                    }
                    else
                    {
                        PixelData[i, 0] = x;
                        PixelData[i, 1] = y;
                        i++;
                    }
                }
            }
            int[,] PixelData2 = new int[i, 2];
            Array.Copy(PixelData, PixelData2, i * 2);
            return PixelData2;
        }

        //找图BGR比较
        private static unsafe bool ScanColor(byte b1, byte g1, byte r1, byte b2, byte g2, byte r2, int similar)
        {
            if ((Math.Abs(b1 - b2)) > similar) { return false; } //B
            if ((Math.Abs(g1 - g2)) > similar) { return false; } //G
            if ((Math.Abs(r1 - r2)) > similar) { return false; } //R
            return true;
        }

        #endregion
    }
}
