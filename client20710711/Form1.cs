using AGV_V1._0.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client20710711
{
     partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static ClientManager cm;
        private System.Threading.Timer sendRecvTimer;
        private int HandleDateTime = 100;
        private void button1_Click(object sender, EventArgs e)
        {
            //if (cm != null)
            //{
            try
            {
                cm = new ClientManager();
                cm.ShowMessage += ClientMessage;
                cm.DateMessage += HandleDate;
                cm.ReLoad += LoadAgvFile;
                cm.ConnectToServer("127.0.0.1", 5555);
                this.sendRecvTimer = new System.Threading.Timer(OnRecvTimerCallback, null, 0, HandleDateTime);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接异常:" + ex.ToString());
            }
            //}
            //else
            //{
            //    AddToLog("以连接，请勿重复连接");
            //}
        }
        void LoadAgvFile()
        {

        }
        List<ListViewItem> listItem = new List<ListViewItem>();

        void HandleDate(object sender,MessageEventArgs e )
        {
            if (e.Type == MessageType.AgvFile)
            {
                ReceiveFile(e.Message);
            }
            else
            {
             HandlerData(e.Message);

            }
           // QueueJson.Instance.AddMyQueueList(e.Message);
            //if (QueueJson.Instance.GetMyQueueCount() > 3&&HandleDateTime>1)
            //{
            //    this.sendRecvTimer.Change(0, HandleDateTime / 2);
            //}
        }

        private void ReceiveFile(string data)
        {
            if (data == "" || data == null)
            {
                AddToLog("没有数据");
                return;
            }
            try
            {
                string path = "..\\..\\AGV.xml";


                using (FileStream fswrite = new FileStream(path, FileMode.Create, FileAccess.Write))
                {

                    StreamWriter sw = new StreamWriter(fswrite);
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                }
                AddToLog("文件接收成功");
                // ReLoad();
            }
            catch
            {
                AddToLog("文件接收失败");
            }

        }
        void OnRecvTimerCallback(object state)
        {           
                try
                {
                    //if (!QueueJson.Instance.IsMyQueueHasData())
                    //{
                    //    return;
                    //}
                    //string json = QueueJson.Instance.GetMyQueueList();
                   // AddToLog(json);

                    //字符串json.indexof("{")很耗时

                     //   Stopwatch watch = Stopwatch.StartNew();
                    //  AddToLog("" + WatchTime(watch));
                  
                }


                catch
                {
                }
           
        }
        long time = 0;
        long oldTime = 0;
        void HandlerData(string json)
        {
            oldTime = time;
            time = DateTime.Now.Millisecond;
            Debug.WriteLine("接到时间："+(time-oldTime)+" | " + json);

            GuiData[] jsonObj = (GuiData[])JsonConvert.DeserializeObject(json, typeof(GuiData[]));
            listItem = new List<ListViewItem>();
            rand = new Random(5);
            for (int i = 0; i < jsonObj.Length; i++)
            {
                //添加Item：此处以数组为例
                string[] myItem1 = new string[4] { jsonObj[i].Num + "", jsonObj[i].BeginX + "", jsonObj[i].BeginY + "", jsonObj[i].State.ToString() };

                ListViewItem item1 = new ListViewItem(myItem1);
                int R = rand.Next(100, 255);
                int G = rand.Next(100, 255);
                int B = rand.Next(0, 255);
                Color vehicleColor = Color.FromArgb(R, G, 0);
                item1.BackColor = vehicleColor;
                listItem.Add(item1);
            }
            UpdateUI();

        }
        double WatchTime(Stopwatch stopwatch)
        {
            stopwatch.Stop(); //  停止监视
            TimeSpan timeSpan1 = stopwatch.Elapsed; //  获取总时间
            return Math.Round(timeSpan1.TotalSeconds, 4);
        }
        private void UpdateUI()
        {

            if (listView1.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action actionDelegate = () => { update(); };

                //    IAsyncResult asyncResult =actionDelegate.BeginInvoke()

                // 或者 
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                this.listView1.Invoke(actionDelegate, null);
            }
            else
            {
                update();
            }
            //if (pic.InvokeRequired)
            //{
            //    // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
            //    Action actionDelegate = () => { update(); };
            //    // 或者
            //    // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
            //    this.pic.Invoke(actionDelegate, null);
            //}
            //else
            //{
            //    update();
            //}
        }

        private Bitmap surface = null;
        private Graphics g = null;
        void update()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            // 设置标头：
            listView1.Columns.Add("编号", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("X", 25, HorizontalAlignment.Center);
            listView1.Columns.Add("Y", 25, HorizontalAlignment.Center);
            listView1.Columns.Add("状态", 100, HorizontalAlignment.Center);
            //// 注意Add（）里的格式，（“标题”，宽度，对齐方式）。         
            if (listItem != null)
            {
                for (int i = 0; i < listItem.Count; i++)
                {                   
                   listView1.Items.Add(listItem[i]);
                }
                
                DrawRect();

            }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        private void AddToLog(string str)
        {

            if (listViewLog.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action actionDelegate = () => { ShowLog(str); };

                //    IAsyncResult asyncResult =actionDelegate.BeginInvoke()

                // 或者 
                // Action<string> actionDelegate = delegate(string txt) { this.label2.Text = txt; };
                this.listViewLog.Invoke(actionDelegate, null);
            }
            else
            {
                ShowLog(str);
            }
        }
        void ShowLog(string str)
        {
            if (listViewLog.Items.Count > 100)
            {
                listViewLog.Clear();
            }
            listViewLog.View = View.Details;
            listViewLog.GridLines = false;
            listViewLog.Columns.Add("信息", 150, HorizontalAlignment.Left);
            listViewLog.Items.Add(new ListViewItem(str));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Thread sendTh = new Thread(new ParameterizedThreadStart(Send));
            //byte []type=new byte[2]{AGV_FILE,CHANGED};
            //sendTh.IsBackground = true;
            //sendTh.Start(type);
            if (cm == null)
            {
                AddToLog("请先连接");
                return;
            }
            cm.Send(MessageType.AgvFile, "../../AGV.xml");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sendCloseMsg();

        }
        void sendCloseMsg()
        {
            if (null != cm)
            {
                cm.Dispose();
            }

        }
        protected void ClientMessage(object sender, MessageEventArgs e)
        {
            System.Console.WriteLine(e.Message);
            AddToLog(e.Message);
        }
        protected void ClientError(object sender, MyErrorEventArgs e)
        {
            System.Console.WriteLine(e.Message);
            AddToLog(e.Message + ":" + e.Exception.Message);
        }
        private const int borad = 1;
        private const int rectW = 6;
        private const int rectH = 6;
        private const int width = 90;
        private const int height = 61;
        public static Random rand;
        void DrawRect()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    g.FillRectangle(new SolidBrush(Color.LightBlue), GetRect(i, j));
                   // g.FillEllipse(new SolidBrush(Color.LightBlue), GetRect(i, j));

                }
            }
            for (int i = 0; i < listItem.Count; i++)
            {
                String textX = listView1.Items[i].SubItems[1].Text;
                int X = Convert.ToInt32(textX);
                String textY = listView1.Items[i].SubItems[2].Text;
                int Y = Convert.ToInt32(textY);
                //g.FillRectangle(new SolidBrush(listView1.Items[i].BackColor), GetRect(Y, X));
                g.FillRectangle(new SolidBrush(listView1.Items[i].BackColor), GetEllipseRect(Y, X));
            }
            pic.Image = surface;

        }
        Rectangle GetRect(int i, int j)
        {
            int x = i * (rectW + borad);
            int y = j * (rectH + borad);
            return new Rectangle(x, y, rectW, rectH);
        }
        Rectangle GetEllipseRect(int i, int j)
        {
            int x = i * (rectW + borad);
            int y = j * (rectH + borad);
            return new Rectangle(x+1, y+1, rectW-2, rectH-2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置pictureBox的尺寸和位置
            surface = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(surface);
            pic.Image = surface;
            pic.BackColor = Color.Gray;
            update();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Thread sendTh = new Thread(new ParameterizedThreadStart(Send));
            //byte[] type = new byte[1] { MSG};
            //sendTh.IsBackground = true;
            //sendTh.Start(type);
            if (cm == null)
            {
                AddToLog("请先连接!");
                return;
            }
            cm.Send(MessageType.reStart, "");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cm == null)
            {
                AddToLog("请先连接");
                return;
            }
            cm.Send(MessageType.mapFile, "../../ElcMap.xml");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (cm == null)
            {
                AddToLog("请先连接!");
                return;
            }
            cm.Send(MessageType.move, "");
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }


    }

}
