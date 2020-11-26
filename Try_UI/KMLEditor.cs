using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Try_UI
{
    public partial class KMLEditor : Form
    {
        public KMLEditor()
        {
            InitializeComponent();
        }

        private void KMLEditor_Load(object sender, EventArgs e)
        {

        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "请选择文件";
            ofd.Filter = "kml文件(*.kml*)|*.kml*|所有文件 (*.*)|*.*";
            ofd.InitialDirectory = "C:/Users/琛琛/Desktop/Try_UI";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 获取打开文件的路径
                string path = ofd.FileName;
                // 指定打开文件路径及编码
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                string text = sr.ReadToEnd();
                textBox1.Text = text;
                sr.Close();
                // 将窗体标题改为打开文件路径
                this.Text = path;
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            // 当前内容不为空
            if (textBox1.Text.Trim() != "")
            {
                    SaveFileDialog sfd = new SaveFileDialog();
                    // 创建筛选器，指定保存文件的类型
                    sfd.Filter = ("文本文档(*.kml)|*.kml|所有文件 (*.*)|*.*");
                    sfd.InitialDirectory = "C:/Users/琛琛/Desktop/Try_UI";
                    // 判断用户选择的是保存按钮还是取消按钮
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        // 获取用户选择路径
                        string path = sfd.FileName;
                        // 保存文件到指定路径
                        StreamWriter sw = new StreamWriter(path, false);
                        sw.WriteLine(textBox1.Text.Trim());
                        // 清空缓存
                        sw.Flush();
                        sw.Close();
                    }
            }
            else
            {
                MessageBox.Show("当前文本框内容为空！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
