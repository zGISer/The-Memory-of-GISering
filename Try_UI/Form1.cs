using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EARTHLib;
using System.Runtime.InteropServices;

using System.IO;
using System.Diagnostics;

using System.Media;
namespace Try_UI
{
    public partial class The_Memory_of_GISering : Form
    {
        /// <summary>
        /// 用来关闭GoogleEarth的消息定义
        /// </summary>
        static readonly Int32 WM_QUIT = 0x0012;

        private IntPtr GEHWnd = (IntPtr)5;
        private IntPtr GEHrender = (IntPtr)5;
        private IntPtr GEParentHrender = (IntPtr)5;

        private int jstr1, jstr2, jstr3;
        private int wstr1, wstr2, wstr3;
        private double ilon, dlon, ilat, dlat;
        private const double GEPI = 3600.0;

        private bool isGEStarted = false;
        /// <summary>
        /// 定义GE应用程序类
        /// </summary>
        private ApplicationGEClass GeApp;
        private string ssFile;
        
        //NativeMethods类
        public class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, UInt32 uflags);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr PostMessage(int hWnd, int msg, int wParam, int lParam);

            #region 预定义

            public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
            public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
            public static readonly IntPtr HWND_TOP = new IntPtr(0);
            public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            public static readonly UInt32 SWP_NOSIZE = 1;
            public static readonly UInt32 SWP_NOMOVE = 2;
            public static readonly UInt32 SWP_NOZORDER = 4;
            public static readonly UInt32 SWP_NOREDRAW = 8;
            public static readonly UInt32 SWP_NOACTIVATE = 16;
            public static readonly UInt32 SWP_FRAMECHANGED = 32;
            public static readonly UInt32 SWP_SHOWWINDOW = 64;
            public static readonly UInt32 SWP_HIDEWINDOW = 128;
            public static readonly UInt32 SWP_NOCOPYBITS = 256;
            public static readonly UInt32 SWP_NOOWNERZORDER = 512;
            public static readonly UInt32 SWP_NOSENDCHANGING = 1024;

            #endregion

            public delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

            [DllImport("user32", CharSet = CharSet.Auto)]
            public extern static IntPtr GetParent(IntPtr hWnd);

            [DllImport("user32", CharSet = CharSet.Auto)]
            public extern static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

            [DllImport("user32", CharSet = CharSet.Auto)]
            public extern static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

            public static int GW_CHILD = 5;
            public static int GW_HWNDNEXT = 2;



        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //if (!this.DesignMode)
            //{
            try
            {
                //GoogleEarth实例化
                GeApp = new ApplicationGEClass();
                //取得GE主窗口句柄
                GEHWnd = (IntPtr)GeApp.GetMainHwnd();
                //隐藏GoogleEarth主窗口
                NativeMethods.SetWindowPos(GEHWnd, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0,
                    NativeMethods.SWP_NOSIZE + NativeMethods.SWP_HIDEWINDOW);
                //将渲染窗口嵌入到主窗体
                //取得GE的影像窗口(渲染窗口)句柄
                GEHrender = (IntPtr)GeApp.GetRenderHwnd();
                GEParentHrender = (IntPtr)NativeMethods.GetParent(GEHrender);

                NativeMethods.MoveWindow(GEHrender, 0, 0, this.Width, this.Height, true);

                NativeMethods.SetParent(GEHrender, this.MainPanel.Handle);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //}
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //关闭窗口
            NativeMethods.PostMessage((int)GEHWnd, WM_QUIT, 0, 0);
        }
        public The_Memory_of_GISering()
        {
            InitializeComponent();
            customizeDesing();
        }
        private void The_Memory_of_GISering_Load(object sender, EventArgs e)
        {

        }

        //控制菜单按键的函数
        private void customizeDesing()
        {
            Openchildpanel.Visible = false;
            Playchildpanel.Visible = false;
            panelLogo.Visible = true;
            SideMenupanel.Visible = true;
        }
        //控制菜单显示的函数
        private void hideSubMenu()
        {
            if (Openchildpanel.Visible == true)
                Openchildpanel.Visible = false;
            if (Playchildpanel.Visible == true)
                Playchildpanel.Visible = false;
        }
        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }
        //父级按键,主要控制button的可视情况
        private void Play_Click(object sender, EventArgs e)
        {
            showSubMenu(Playchildpanel);
        }
        private void KML_Click(object sender, EventArgs e)
        {
            showSubMenu(Openchildpanel);
        }
        //具体Button的功能按键
        private void Editkml_Click(object sender, EventArgs e)
        {
            openChildForm(new KMLEditor());
            hideSubMenu();
        }
        //打开KML
        private void Openkml_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择文件";
            ofd.Filter = "kml文件(*.kml*)|*.kml*";
            //这是系统提供的桌面路径，还可以是其他的路径：比如文档、音乐等文件夹
            //ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ofd.InitialDirectory = "C:/Users/琛琛/Desktop/Try_UI";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string file = ofd.FileName;
                GeApp.OpenKmlFile(file, 1);
                hideSubMenu();
            }
        }
        private void Savekml_Click(object sender, EventArgs e)
        {
            MessageBox.Show("决策失误");
            hideSubMenu();
        }
        private void BYkml_Click(object sender, EventArgs e)
        {
            MessageBox.Show("本功能尚未开发，敬请期待!");
            hideSubMenu();
        }
        //GoogleEarth截屏
        private void Cut_Click(object sender, EventArgs e)
        {
            ssFile = Path.Combine(Application.StartupPath, System.DateTime.Now.ToString("GES_yyyyMMddHHmmss") + ".jpg");
            try
            {
                //quality的取值范围在(0,100)之间，质量越高，quality越大
                GeApp.SaveScreenShot(ssFile, 100);
                //载入刚才的图像,会和panal起冲突？可能
                //pictureBox1.Image = Bitmap.FromFile(ssFile);
                MessageBox.Show("截图文件名称为" + System.DateTime.Now.ToString("GES_yyyyMMddHHmmss") + ".jpg" +
                    "\n" + "文件保存至" + Application.StartupPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存截屏图像时发生错误：" + ex.ToString());
            }
        }
        //打开背景音乐
        private void Music_Click(object sender, EventArgs e)
        {
            openChildForm(new Form2());
            hideSubMenu();
        }
        //HELP
        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by GISering_CC");
        }
        //结束程序
        private void Exit_Click(object sender, EventArgs e)
        {
            try
            {
                //杀掉GoogleEarth进程
                Process[] geProcess = Process.GetProcessesByName("GoogleEarth");
                foreach (var p in geProcess)
                {
                    p.Kill();
                }
                //清除内容
                GeApp = null;
                GEHrender = (IntPtr)0;
                GEParentHrender = (IntPtr)0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
            Application.Exit();
        }
        
        //打开子窗口
        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }
        public struct SCamParams
        {
            public double dLat;
            public double dLon;
            public double dAlt;
            public double dRng;
            public double dAng;
            public double dAzm;
            public double dSpd;
        }
        SCamParams scp = new SCamParams();
        public double TranslateLatLon(string slatlon)
        {
            double ddeg = 0.0;
            double dmin = 0.0;
            double dsec = 0.0;
            int start = 0, pos = 0;
            #region VAL
            pos = slatlon.IndexOf('°', start);
            if (pos > 0)
            {
                #region ddeg
                while (start < pos)
                {
                    if (slatlon[start] == '0') ++start;
                    else break;
                }
                if (start < pos - 1)
                {
                    ddeg = double.Parse(slatlon.Substring(start, pos - start));
                }
                #endregion ddeg

                start = pos + 1;
                pos = slatlon.IndexOf('\'', start);
                if (pos > 0)
                {
                    #region dmin
                    while (start < pos)
                    {
                        if (slatlon[start] == '0') ++start;
                        else break;
                    }
                    if (start < pos - 1)
                    {
                        dmin = double.Parse(slatlon.Substring(start, pos - start));
                    }
                    #endregion dmin

                    start = pos + 1;
                    pos = slatlon.IndexOf('\'', start);
                    if (pos > 0)
                    {
                        #region dsec
                        while (start < pos)
                        {
                            if (slatlon[start] == '0') ++start;
                            else break;
                        }
                        if (start < pos - 1)
                        {
                            dsec = double.Parse(slatlon.Substring(start, pos - start));
                        }
                        #endregion dsec
                    } //if(pos,dsec)
                } //if(pos,dmin)
                #region SGN
                pos = slatlon.LastIndexOf('\'') + 1;
                if (slatlon[pos] == 'S' || slatlon[pos] == 'W')
                {
                    ddeg = -ddeg;
                    dmin = -dmin;
                    dsec = -dsec;
                } //if(pos,len-1)
                #endregion SGN
            } //if(pos ddeg)
            else
            {
                ddeg = double.Parse(slatlon);
            }
            #endregion VAL

            return (ddeg + dmin / 60.0 + dsec / 3600.0);
        }
        private void GetParamsValue()
        {
            scp.dLat = TranslateLatLon(txtBoxLat.Text);
            scp.dLon = TranslateLatLon(txtBoxLon.Text);
            scp.dAlt = double.Parse(txtBoxAlt.Text);
            scp.dRng = double.Parse(txtBoxRng.Text);
            scp.dAng = double.Parse(txtBoxAng.Text);
            scp.dAzm = double.Parse(txtBoxAzm.Text);
            scp.dSpd = double.Parse(txtBoxSpd.Text);


        }
        private void SetParamsDefault()
        {
            //默认值为体育馆附近
            txtBoxLat.Text = "28°11'16.1''N";
            txtBoxLon.Text = "112°56'36.9''E";
            txtBoxAlt.Text = "01";
            txtBoxRng.Text = "0777";
            txtBoxAng.Text = "01";
            txtBoxAzm.Text = "01";
            txtBoxSpd.Text = "03";

            scp.dLat = 28.11161;
            scp.dLon = 112.56369;
            scp.dAlt = 01;
            scp.dRng = 0777;
            scp.dAng = 01;
            scp.dAzm = 01;
            scp.dSpd = 03;
        }
        private bool CheckInputParams()
        {
            return (scp.dLat >= -90 && scp.dLat <= 90) &&
                (scp.dLon >= -180 && scp.dLon <= 180) &&
                (scp.dAlt >= 0) && (scp.dRng > 0) &&
                (scp.dAng >= 0) && (scp.dSpd > 0);
        }

        private void Camera_Click(object sender, EventArgs e)
        {
            SetParamsDefault();
            GetParamsValue();
            if (CheckInputParams())
            {
                GeApp.SetCameraParams(scp.dLat, scp.dLon, scp.dAlt,
                    AltitudeModeGE.AbsoluteAltitudeGE,
                    scp.dRng, scp.dAng, scp.dAzm, scp.dSpd);
            }
            else
            {
                MessageBox.Show("Invalid Parameters!");
            }
            hideSubMenu();
        }
        private void Extent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("尚未开启");
            hideSubMenu();
        }

        private CameraInfoGEClass myCamera;
        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //当GoogleEarth接管Panel之后，e可能就不能接收panel的坐标了。
            textBox1.Text = e.Location.ToString();
            //GeApp.GetCamera(1);
            //txtBoxLon.Text = GeApp.GetCamera(2).ToString();
            PointOnTerrainGE pt = GeApp.GetPointOnTerrainFromScreenCoords(0.0, 0.0);
            //myCamera=new CameraInfoGEClass();
            //txtBoxLat.Text = myCamera.FocusPointLatitude.ToString();
            if (pt != null)
            {
                double m_lat = pt.Latitude;
                double m_lng = pt.Longitude;
                txtBoxLat.Text = m_lat.ToString();
                txtBoxLon.Text = m_lng.ToString();
            }

        }

        private void txtBoxLat_TextChanged(object sender, EventArgs e)
        {
            try
            {
                scp.dLat = TranslateLatLon(txtBoxLat.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
            
        }
        private void txtBoxLon_TextChanged(object sender, EventArgs e)
        {
            try
            {
                scp.dLon = TranslateLatLon(txtBoxLon.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
            
        }
        private void txtBoxAlt_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                scp.dAlt = double.Parse(txtBoxAlt.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
        }
        private void txtBoxRng_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                scp.dRng = double.Parse(txtBoxRng.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
        }
        private void txtBoxAng_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                scp.dAng = double.Parse(txtBoxAng.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
        }
        private void txtBoxAzm_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                scp.dAzm = double.Parse(txtBoxAzm.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
        }
        private void txtBoxSpd_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                scp.dSpd = double.Parse(txtBoxSpd.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Shutdown GE");
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            if (CheckInputParams())
            {
                GeApp.SetCameraParams(scp.dLat, scp.dLon, scp.dAlt,
                    AltitudeModeGE.AbsoluteAltitudeGE,
                    scp.dRng, scp.dAng, scp.dAzm, scp.dSpd);
            }
            else
            {
                MessageBox.Show("Invalid Parameters!");
            }
        }





        



        

        







    }
}
