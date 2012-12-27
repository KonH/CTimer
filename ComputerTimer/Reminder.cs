using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComputerTimer
{
    [Serializable]
    public class Reminder
    {
        public string text;
        public int interval;

        public Reminder()
        {
        }

        public Reminder(string _text, int _interval)
        {
            text = _text;
            interval = _interval;
        }
    }
}
