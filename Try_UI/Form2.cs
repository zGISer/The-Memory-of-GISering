using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using System.Media;

namespace Try_UI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Music_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            //o.Filter = "音频文件(*.wav)|*.wav*";
            
            o.Filter = "MP3文件(*.mp3)|*.mp3|Audio文件(*.avi)|*.avi|WAV文件(*.wav)|*.wav|VCD文件(*.dat)|*.dat|所有文件 (*.*)|*.*";
            o.Title="打开音乐";
            o.InitialDirectory = "E:/i/大三上/3Dmax/Work";
            try
            {
                if (o.ShowDialog() == DialogResult.OK)
                {
                    // 2003一下版本 方法this.axMediaPlayer1.FileName = ofDialog.FileName;
                    this.axWindowsMediaPlayer1.URL = o.FileName;//2005用法
                }
                /*
                SoundPlayer player = new SoundPlayer(o.FileName);
                //player.Play();//只播放一遍
                player.PlayLooping();//循环播放
                 */
            }
            catch (Exception ex)
            {
                MessageBox.Show("背景音乐打开发生错误：" + ex.ToString());
            }
        }

    }
}
