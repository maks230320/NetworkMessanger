using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace NetworkMessanger
{
    public class NetSender
    {
        public event EventHandler<TextEventArgs> NeedTextIP;
        public event EventHandler<IntEventArgs> NeedPort;

        public NetSender() { 
        }

        public bool Send(string message, IPAddress ip = null)
        {
            try
            {
                string textIp = GetTextIP();
                int port = GetPort();
                if (string.IsNullOrEmpty(textIp) || port < 0)
                {
                    return false;
                }
                Socket socket = new Socket(
                                AddressFamily.InterNetwork,
                                SocketType.Dgram,
                                ProtocolType.IP);

                if (ip == null)
                    ip = IPAddress.Parse(textIp);

                socket.SendTo(
                    Encoding.Unicode.GetBytes(message),
                    new IPEndPoint(ip, port));
                socket.Shutdown(SocketShutdown.Send);
                socket.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string GetTextIP()
        {
            if (NeedTextIP != null)
            {
                TextEventArgs e = new TextEventArgs();
                NeedTextIP.Invoke(this, e);
                return e.Text;
            }
            else
            {
                return String.Empty;
            }
            
        }
        private int GetPort()
        {
            if (NeedPort != null)
            {
                IntEventArgs e = new IntEventArgs();
                NeedPort.Invoke(this, e);
                return e.Number;
            }
            else
            {
                return -1;
            }
        }
    }

}
