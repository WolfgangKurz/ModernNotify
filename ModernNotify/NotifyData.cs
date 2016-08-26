using System;
using System.Drawing;

namespace ModernNotify
{
    public class NotifyData
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public bool OnSound { get; set; } = true;
        public Image Icon { get; set; } = null;

        public Action Activated { get; set; }
        public Action<Exception> Failed { get; set; }
    }
}
