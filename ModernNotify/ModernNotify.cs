using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModernNotify
{
    public class ModernNotify
    {
        public static ModernNotify Instance => new ModernNotify();
        private static frmNotify NotifyInstance { get; set; }

        public bool Notify(NotifyData Notify)
        {
            try
            {
                NotifyInstance = new frmNotify(Notify);
                NotifyInstance.Show();

                return true;
            }
            catch (Exception e)
            {
                Notify.Failed?.Invoke(e);

                return false;
            }
        }

        /*
        // For Debug
        public static void Main() {
            ModernNotify.Instance.Notify(new NotifyData()
            {
                Title = "TEST",
                Content = "TEST TEST TEST TEST",
                Activated = () =>
                {
                    MessageBox.Show("Activated");
                    Application.Exit();
                }
            });

            Application.Run(NotifyInstance);
        }
        // */
    }
}
