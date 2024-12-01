using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetworkMessanger
{
    public class NetReciever
    {
        Thread thread;
        Socket socket;
        int port;

        public event EventHandler<TextEventArgs> NeedTargetIP;
        public event EventHandler<IntEventArgs> NeedPort;
        public event EventHandler<MessageEventArgs> MessageRecived;

        public NetReciever() { }

        public SocketError Start()
        {
            string ip = GetTargetTextIP();
            this.port = GetPort();
            if(string.IsNullOrEmpty(ip) || port < 0)
            {
                return SocketError.DestinationAddressRequired;
            }

            IPAddress targetAddres = IPAddress.Parse(ip);

            if (socket != null && thread != null)
                return SocketError.InProgress;

            socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.IP);
            try {
                socket.Bind(
                    new IPEndPoint(
                        targetAddres,
                        port)
                    );
            }
            catch(SocketException ex)
            {
                socket.Close();
                socket = null;
                return ex.SocketErrorCode;
            }
            thread = new Thread(ReciveFunction);
            thread.Start(socket);
            return SocketError.Success;
        }

        public void Stop()
        {
            if (socket != null)
            {
                thread.Abort();
                thread = null;
                socket.Shutdown(SocketShutdown.Receive);
                socket.Close();
                socket = null;
            }
        }

        void ReciveFunction(object obj)
        {
            Socket rs = (Socket)obj;
            byte[] buffer = new byte[1024];
            do
            {
                EndPoint ep = new IPEndPoint(0x7F000000, port);
                int l = rs.ReceiveFrom(buffer, ref ep);
                IPAddress address = ((IPEndPoint)ep).Address;
                string message = Encoding.Unicode.GetString(buffer, 0, l);
                if(MessageRecived != null)
                {
                    MessageRecived.Invoke(this, new MessageEventArgs(message, address));
                }
            } while (true);
        }

        private string GetTargetTextIP()
        {
            if (NeedTargetIP != null)
            {
                TextEventArgs e = new TextEventArgs();
                NeedTargetIP.Invoke(this, e);
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
