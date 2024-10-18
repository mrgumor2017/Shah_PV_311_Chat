using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAppChat
{
    internal class MessageInfo
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public MessageInfo(string Text, DateTime Date) 
        {
            this.Text = Text;
            this.Date = Date;
        }
        public override string ToString()
        {
            return $"{Text} . Date :{Date}";
        }
    }
}
