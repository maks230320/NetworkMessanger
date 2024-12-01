using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMessanger
{
    public class TextEventArgs: EventArgs
    {
        public TextEventArgs() { }
        public TextEventArgs(string text):this()
        {
            Text = text;
        }

        public String Text { get; set; }
    }
}
