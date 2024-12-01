using System.Net;

namespace NetworkMessanger
{
    public class MessageEventArgs: TextEventArgs
    {
        public MessageEventArgs(string message, IPAddress address):base(message) { 
            Address = address;
        }
        public IPAddress Address { get; set; }
    }
}
