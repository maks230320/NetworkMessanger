using System;
using System.Collections.Generic;
using System.Net;

namespace NetworkMessanger
{
    public class Nicknames
    {
        Dictionary<IPAddress, String> nicknames;


        public Nicknames() {
            nicknames = new Dictionary<IPAddress, String>();
        }

        public string this[IPAddress address]
        {
            get {
                return GetNickname(address);
            }
            set { 
                Add(address, value);
            }
        }

        public bool Contains(IPAddress address)
        {
            return nicknames.ContainsKey(address);
        }

        void Add(IPAddress key, String value)
        {
            if (!Contains(key))
                nicknames.Add(key, value);
            else
                nicknames[key] = value;
        }

        string GetNickname(IPAddress address)
        {
            String nickname;

            if (Contains(address))
            {
                nickname = nicknames[address];
            }
            else
            {
                nickname = address.ToString();
            }

            return nickname;
        }
    }
}
