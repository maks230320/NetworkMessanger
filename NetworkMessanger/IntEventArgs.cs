using System;

namespace NetworkMessanger
{
    public class IntEventArgs:EventArgs
    {
        public IntEventArgs() { }
        public IntEventArgs(int number) : this()
        {
            Number = number;
        }

        public int Number { get; set; }
    }
}
