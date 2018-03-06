using Cowboy.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client20710711
{
    public class ClientManager : IDisposable
    {

        public event EventHandler<MessageEventArgs> DateMessage;
        public event EventHandler<MessageEventArgs> ShowMessage;
        public delegate void ReLoadDele();
        public ReLoadDele ReLoad;
        static TcpSocketClient _client;
        private static ClientManager instance;
        public static ClientManager Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new ClientManager();
                }
                return instance;
            }
        }
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="IpAdress">服务器地址</param>
        /// <param name="port">服务器监听的端口号</param>
        public void ConnectToServer(string IpAdress, int port)
        {
            try
            {
                var config = new TcpSocketClientConfiguration();
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(IpAdress), port);
                _client = new TcpSocketClient(remoteEP, config);
                _client.ServerConnected += client_ServerConnected;
                _client.ServerDisconnected += client_ServerDisconnected;
                _client.ServerDataReceived += client_ServerDataReceived;
                _client.Connect();
            }
            catch
            {
                throw;
            }
        }

        void client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
        {
            string str = string.Format("TCP Client {0} has connected.", e.LocalEndPoint);
            Console.WriteLine(str);
            OnMessageEvent(str);
        }

        void client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
        {
            string str = string.Format("TCP Client {0} has disconnected.", e.LocalEndPoint);
            OnMessageEvent(str);
            Console.WriteLine(str);
        }

        void client_ServerDataReceived(object sender, TcpServerDataReceivedEventArgs e)
        {
            //OnDateMessageEvent(sender, e);

            HandlerDateMessage(e);

            //HandlerDateMessage(e);
            //string str = string.Format("Server : {0} --> ", e.Client.RemoteEndPoint);
            //OnMessageEvent(str);
            //Console.Write(str);
            //if (e.DataLength < 1024 * 1024 * 1)
            //{
            //    Console.WriteLine(text);
            //    OnMessageEvent(text);
            //}
            //else
            //{
            //    Console.WriteLine("{0} Bytes", e.DataLength);
            //}
        }
        void HandlerDateMessage(TcpServerDataReceivedEventArgs e)
        {
            MessageType type = (MessageType)e.Data[e.DataOffset];
            var text = Encoding.UTF8.GetString(e.Data, e.DataOffset + 1, e.DataLength - 1);
            // OnDateMessageEvent(text);
            switch (type)
            {
                case MessageType.msg:
                   // OnMessageEvent("msg");
                    OnDateMessageEvent(this,new MessageEventArgs(type,text));
                    break;
                case MessageType.arrived:
                    OnMessageEvent("arrived");
                    OnDateMessageEvent(this, new MessageEventArgs(type, text));
                    break;
                case MessageType.reStart:
                    OnMessageEvent("reStart");
                    OnDateMessageEvent(this, new MessageEventArgs(type, text));
                    break;
                case MessageType.move:
                    OnMessageEvent("move");
                    OnDateMessageEvent(this, new MessageEventArgs(type, text));
                    break;
                case MessageType.AgvFile:
                    OnMessageEvent("AgvFile");
                    OnDateMessageEvent(this, new MessageEventArgs(type, text));
                  //  OnMessageEvent("AgvFile接收中...");
                  //  string pathAgv = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { '\\' });
                  //  Task.Factory.StartNew(() => ReciveFile(pathAgv, e), TaskCreationOptions.LongRunning);
                    break;
                case MessageType.mapFile:
                    OnMessageEvent("mapFile");
                    OnDateMessageEvent(this, new MessageEventArgs(type, text));
                  //  OnMessageEvent("mapFile接收中...");
                   // string pathMap ="..\\..\\ElcMap.xml";// System.Configuration.ConfigurationManager.AppSettings["MAPPath"].ToString();// 
                  //  Task.Factory.StartNew(() => ReciveFile(pathMap, e), TaskCreationOptions.LongRunning);
                    break;
                default:
                    OnMessageEvent("错误的消息类型");
                    break;

            }
        }
        public void OnMessageEvent(string message)
        {
            try
            {
                if (null != ShowMessage)
                {
                    ShowMessage(this, new MessageEventArgs(message));
                }
            }
            catch
            {

            }
        }
        public const int SUCCESS = 6;
        public const int FAIRED = -1;
        void ReciveFile(string path, TcpServerDataReceivedEventArgs e)
        {
            try
            {
                using (FileStream fswrite = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    fswrite.Write(e.Data, e.DataOffset + 1, e.DataLength - 1);
                }
               //  OnMessageEvent("文件接收成功");
               // ReLoad();
            }
            catch
            {
                OnMessageEvent("文件接收失败");
            }
        }

        //public void OnMessageEvent(string message)
        //{
        //    try
        //    {
        //        if (null != ShowMessage)
        //        {
        //            ShowMessage(this, new MessageEventArgs(message));
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        public void OnDateMessageEvent(object sender, MessageEventArgs e)
        {
            try
            {
                if (null != DateMessage)
                {
                    DateMessage(sender, e);
                }
            }
            catch
            {

            }
        }

        public int Send(MessageType type, string text)
        {
            try
            {
                int res = FAIRED;
                switch (type)
                {
                    case MessageType.msg:
                    case MessageType.reStart:
                    case MessageType.disConnect:
                    case MessageType.arrived:
                    case MessageType.move:
                    case MessageType.none:
                        List<byte> list = new List<byte>();
                        list.Add((byte)type);
                        list.AddRange(Encoding.UTF8.GetBytes(text));
                        _client.Send(list.ToArray());
                        return SUCCESS;
                    case MessageType.AgvFile:
                    case MessageType.mapFile:
                        res = SendFile(text, (byte)type);
                        return SUCCESS;
                    default:
                        OnMessageEvent("错误的消息类型");
                        return FAIRED;
                }
            }
            catch (Exception ex)
            {
                OnMessageEvent("发送失败:" + ex);
                return FAIRED;
            }

        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="flag">文件类型</param>
        int SendFile(string path, byte flag)
        {
            if (!File.Exists(path))
            {
                OnMessageEvent("文件不存在");
                return FAIRED;
            }
            try
            {
                using (FileStream fsRead = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    int r = fsRead.Read(buffer, 0, buffer.Length);
                    List<byte> list = new List<byte>();
                    list.Add(flag);
                    list.AddRange(buffer);
                    byte[] newBuffer = list.ToArray();
                    list.Clear();
                    _client.Send(newBuffer, 0, r + 1);
                    // Task.Factory.StartNew(() => _client.Send(newBuffer, 0, r + 1, SocketFlags.None), TaskCreationOptions.LongRunning);

                    // MessageBox.Show("文件已发送~~");
                    OnMessageEvent("文件发送成功");
                    return SUCCESS;
                }
            }
            catch (Exception ex)
            {
                OnMessageEvent("文件发送失败" + ex);
                return FAIRED;
            }
        }

        public void Dispose()
        {
            _client.Shutdown();
        }


    }
}
