using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace clean
{
    public partial class clean : Form
    {
        private string cmd = "";
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private ProgressBar progressBar;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        public clean(string[] args)
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            foreach (string temp in args)
            {
                cmd += temp+" ";
            }
        }

        private void InitializeBackgroundWorker()//初始化异步操作
        {
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
        }

        private void clean_Load(object sender, EventArgs e)
        {
            LoadForm();
            LoadText();
            LoadButtonAttribute();
        }
        public void LoadButtonAttribute()//装载右上角按钮和确定取消按钮
        {
            LoadImage(Properties.Resources._11, this.Width - 45 - 4, 0, 45, 22, "关闭");
            label3 = SetLabelButtonOneOK(this.Width / 4 * 1 , this.Height - 45, 100, 24, "开始运行");
            label4 = SetLabelButtonOneOK(this.Width / 4 * 3 - 100 , this.Height - 45, 100, 24, "取消");
        }
        private Label SetLabelButtonOneOK(int x, int y, int width, int height, string name)//设置自定义label按钮
        {
            Label lb = new Label();
            lb.Left = x;
            lb.Top = y;
            lb.Width = width;
            lb.Height = height;
            lb.Image = Properties.Resources._12;
            lb.BackColor = Color.Transparent;
            lb.Text = name;
            lb.TextAlign = ContentAlignment.MiddleCenter;
            lb.ForeColor = Color.White;
            lb.MouseEnter += Lb_MouseEnter;
            lb.MouseLeave += Lb_MouseLeave;
            lb.MouseDown += Lb_MouseDown;
            lb.MouseUp += Lb_MouseUp;
            if (name.Equals("取消"))
            {
                lb.MouseClick += Pb_MouseClick4;
            }else
            {
                lb.MouseClick += Lb_MouseClick;
            }
            
            this.Controls.Add(lb);
            return lb;
        }
        private void Lb_MouseClick(object sender, MouseEventArgs e)
        {
            if (bgWorker.IsBusy)
                return;
            this.progressBar.Maximum = 100;
            bgWorker.RunWorkerAsync(cmd);
        }

        private void Lb_MouseUp(object sender, MouseEventArgs e)
        {
            ((Label)sender).Image = Properties.Resources._12;
        }
        private void Lb_MouseDown(object sender, MouseEventArgs e)
        {
            ((Label)sender).Image = Properties.Resources._112;
        }
        private void Lb_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources._12;
        }
        private void Lb_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).Image = Properties.Resources._012;
        }
        private void Pb_MouseClick4(object sender, MouseEventArgs e)//鼠标点击关闭（这个点击的是对话框里的关闭）
        {
            this.Close();
        }
        private void Pb_MouseLeave2(object sender, EventArgs e)//鼠标离开
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.FromArgb(0, 0, 0, 0);
        }
        private void Pb_MouseEnter4(object sender, EventArgs e)//鼠标进入了关闭按钮
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.FromArgb(180, 230, 10, 10);
        }

        public void LoadImage(Image image, int left, int top, int width, int height, string name)//装载右上角按钮
        {
            PictureBox pb = new PictureBox();
            pb.Image = image;
            pb.Left = left;
            pb.Top = top;
            pb.Width = width;
            pb.Height = height;
            pb.BackColor = Color.Transparent;
            pb.SizeMode = PictureBoxSizeMode.CenterImage;
            ToolTip tt = new ToolTip();
            tt.SetToolTip(pb, name);
            pb.MouseEnter += Pb_MouseEnter4;
            pb.MouseClick += Pb_MouseClick4;
            pb.MouseLeave += Pb_MouseLeave2;
            this.Controls.Add(pb);
        }

        private void LoadForm()//设置窗口
        {
            this.Icon = Properties.Resources.clean;
            this.BackgroundImage = Properties.Resources.background;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.TopMost = true;
        }
        private void LoadText()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            SetLabel("Clean 1.0", 12, 10, "微软雅黑", 10, Color.Transparent, true);
            this.Width = 500;
            this.Height = 220;
            Rectangle rect = Screen.GetWorkingArea(this);
            this.Left = rect.Width / 2 - this.Width / 2;
            this.Top = rect.Height / 2 - this.Height / 2;
            progressBar = new ProgressBar();
            progressBar.Width = this.Width;
            progressBar.Height = 30;
            progressBar.Top = 80;
            progressBar.Left = 0;
            this.Controls.Add(progressBar);
            label1 = SetLabel("", 0, 50, "微软雅黑", 10, Color.Transparent, false);
            label2 = SetLabel("", 12, 120, "微软雅黑", 10, Color.Transparent, true);
        }
        private Label SetLabel(string text, int x, int y, string font, int fontsize, Color BackColor, bool Size)//增加显示文字
        {
            Label title = new Label();
            title.Text = text;
            title.Left = x;
            title.Top = y;
            title.AutoSize = Size;
            title.Width = this.Width;
            title.Height = 20;
            title.BackColor = BackColor;
            title.Font = new Font(font, fontsize);
            title.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(title);
            return title;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private void clean_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF010 + 0x0002, 0);
        }


        public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            double lineCount = new StringReader(e.Argument.ToString()).ReadToEnd().Split('\n').Length;
            using (StringReader sr = new StringReader(e.Argument.ToString()))
            {
                string line;
                double lineIndex = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    lineIndex++;
                    bgWorker.ReportProgress(Convert.ToInt32((lineIndex / lineCount) * 100), line);
                    Thread.Sleep(10);
                    InvokeExcute(line);
                }
            }
        }

        public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            this.label2.Text = "正在处理："+((string)e.UserState);//接收ReportProgress方法传递过来的userState
            this.progressBar.Value = e.ProgressPercentage;
            this.label1.Text = "处理进度:" + Convert.ToString(e.ProgressPercentage) + "%";
        }

        public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                this.label1.Text = "已完成!";
                this.label2.Text = "";
                this.label3.Visible = false;
                this.label4.Text = "关闭";
                this.label4.Left = this.Width/2 - 50;
            }
        }

        public static string InvokeExcute(string command)
        {
            command = command.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                p.Start();//启动程序
                          //向cmd窗口写入命令
                p.StandardInput.WriteLine(command);
                p.StandardInput.AutoFlush = true;
                //获取cmd窗口的输出信息
                StreamReader reader = p.StandardOutput;//截取输出流
                StreamReader error = p.StandardError;//截取错误信息
                string str = reader.ReadToEnd() + error.ReadToEnd();
                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return str;
            }
        }

    }
}
