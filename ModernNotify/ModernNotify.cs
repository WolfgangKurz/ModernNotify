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

        public void Notify(NotifyData Notify)
        {
            var form = new frmNotify(Notify);
            form.Show();
        }

        /*
        * For Debug
        public static void Main() {
            ModernNotify.Instance.Notify(new NotifyData()
            {
                Title = "TEST",
                Content = "TEST TEST TEST TEST",
                Activated = () => Application.Exit()
            });

            Application.Run();
        }
        */
    }
}
