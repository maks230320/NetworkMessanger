using System;
using System.Net;

namespace NetworkMessanger
{
    public class MessageProcessor
    {
        public event EventHandler<TextEventArgs> NeedTargetIP;
        public event EventHandler<TextEventArgs> NeedNick;
        public event EventHandler<MessageEventArgs> NeedSendMessage;

        Nicknames nicknames;

        public MessageProcessor() {
            nicknames = new Nicknames();            
        }

        public string Process(string message, IPAddress address) {
            nicknames[IPAddress.Parse(GetTargetTextIP())] = GetNick();

            if (IsGetCommand(message)) {
                RaiseNeedSendMessage(Commands.SetNickname(GetNick()), address);
            } else if (IsSetCommand(message)) {
                nicknames[address] = Commands.GetNickname(message);
            } else {
                string nickname = nicknames[address];
                if (!nicknames.Contains(address))
                    RaiseNeedSendMessage(Commands.GETNICK, address);
                return FormatMessage(message, nickname);
            }
            return String.Empty;
        }

        bool IsGetCommand(string message) {
            return message == Commands.GETNICK;
        }

        bool IsSetCommand(string message) {
            return message.Contains(Commands.SETNICK);
        }

        string FormatMessage(string message, string nickname)
        {
            return String.Format(
                "\n{0}: {1}\r\n",
                nickname,
                message);
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
        private string GetNick()
        {
            if (NeedNick != null)
            {
                TextEventArgs e = new TextEventArgs();
                NeedNick.Invoke(this, e);
                return e.Text;
            }
            else
            {
                return String.Empty;
            }
        }
        private void RaiseNeedSendMessage(string message, IPAddress address) {
            if (NeedSendMessage != null) {
                NeedSendMessage.Invoke(this, new MessageEventArgs(message, address));
            }
        }
    }
}
