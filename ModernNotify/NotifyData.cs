using System;
using System.Drawing;

namespace ModernNotify
{
    public class NotifyData
    {
        /// <summary>
        /// Notification Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Notification Content
        /// </summary>
        public string Content { get; set; }


        /// <summary>
        /// Play notification sound
        /// </summary>
        public bool OnSound { get; set; } = true;


        /// <summary>
        /// Notification icon, null if use default icon
        /// </summary>
        public Image Icon { get; set; } = null;

        /// <summary>
        /// Notification icon size with 1:1 ratio
        /// </summary>
        public int IconSize { get; set; } = 16;


        /// <summary>
        /// Clicked callback function
        /// </summary>
        public Action Activated { get; set; }

        /// <summary>
        /// Failed to notify callback function
        /// </summary>
        public Action<Exception> Failed { get; set; }
    }
}
