using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.Runtime.InteropServices;

namespace ModernNotify
{
    public partial class frmNotify : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;

        private NotifyData Source { get; }
        private bool Processed { get; set; } = false;

        private int EnterXDelta { get; set; }
        private int EnterX { get; set; }
        private bool MouseEntered { get; set; }

        private float NotifyScale { get; set; } = 1.0f;
        private bool ScaleDir = false; // false=>to original, true=>to small

        public frmNotify(NotifyData notify)
        {
            InitializeComponent();

            if (notify.Icon == null)
                notify.Icon = Properties.Resources.NotifyIcon;

            if (notify.OnSound)
            {
                Stream stream = Properties.Resources.NotifySound;
                SoundPlayer snd = new SoundPlayer(stream);
                snd.Play();
            }

            this.Source = notify;

            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

            EnterX = 360;
            this.FormBorderStyle = FormBorderStyle.None;
            this.PrepareDrawing();
        }

        private void frmNotify_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(
                new Pen(Extension.RgbColor(0x484848), 1.0f),
                0, 0, this.ClientSize.Width, this.ClientSize.Height - 1
            );

            if (this.ClientSize.Width - 8 > 0)
            {
                Bitmap buffer = new Bitmap(this.ClientSize.Width - 8, this.ClientSize.Height - 8);
                using (Graphics g = Graphics.FromImage(buffer))
                {
                    g.DrawImage(
                        this.Source.Icon,
                        new Rectangle(10, 10, 16, 16)
                    );

                    g.DrawString(
                        this.Source.Title,
                        this.Font,
                        new SolidBrush(Extension.RgbColor(0xffffff)),
                        new Point(36, 6)
                    );
                    g.DrawString(
                        this.Source.Content,
                        this.Font,
                        new SolidBrush(Extension.RgbColor(0xa5a5a5)),
                        new Rectangle(36, 26, 288, this.Height - 28)
                    );
                }

                Graphics g2 = e.Graphics;

                g2.ResetTransform();
                g2.TranslateTransform(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
                g2.ScaleTransform(NotifyScale, NotifyScale);
                g2.TranslateTransform(-buffer.Width / 2, -buffer.Height / 2);

                g2.DrawImage(buffer, 0, 0);
            }
        }

        private void frmNotify_Resize(object sender, EventArgs e)
        {
            this.PrepareDrawing();
        }

        private void PrepareDrawing()
        {
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            SizeF contentSize = g.MeasureString(this.Source.Content, this.Font, 288);

            Rectangle rc = Screen.PrimaryScreen.WorkingArea; // Screen size

            int Left = rc.Right - 360;
            int Top = rc.Bottom - this.Height - 16;
            int Height = (int)contentSize.Height + 40;

            this.Size = new Size(360 - EnterX + EnterXDelta, Height);
            this.Location = new Point(Left + EnterX, Top);

            this.Invalidate();
            this.Update();
        }

        private void frmNotify_MouseEnter(object sender, EventArgs e)
        {
            MouseEntered = true;
            if (tmrEnter.Enabled == false)
                this.tmrClose.Enabled = !MouseEntered;
        }

        private void frmNotify_MouseLeave(object sender, EventArgs e)
        {
            MouseEntered = false;
            if (tmrEnter.Enabled == false)
                this.tmrClose.Enabled = !MouseEntered;
        }

        private void frmNotify_MouseDown(object sender, MouseEventArgs e)
        {
            if (Processed) return;
            ScaleDir = true;
        }

        private void frmNotify_MouseUp(object sender, MouseEventArgs e)
        {
            ScaleDir = false;
        }

        private void frmNotify_Click(object sender, EventArgs e)
        {
            if (Processed) return;
            Processed = true;
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tmrEnter_Tick(object sender, EventArgs e)
        {
            var v = EnterX / 6;
            if (v < 1) v = 1;
            EnterX -= v;
            EnterXDelta = v;

            if (EnterX <= 0)
            {
                this.tmrEnter.Enabled = false;
                this.tmrClose.Enabled = !MouseEntered;
                EnterXDelta = 0;
                EnterX = 0;
            }
            this.PrepareDrawing();
        }

        private void tmrScale_Tick(object sender, EventArgs e)
        {
            float v;
            if (ScaleDir)
                v = (NotifyScale - 0.979f) / 2.18f;
            else
                v = (NotifyScale - 1.0f) / 2.2f;

            if (Math.Abs(NotifyScale - v - 1.0f) < 0.0002f)
            {
                if (Processed)
                {
                    tmrScale.Enabled = false;
                    this.Source.Activated?.Invoke();
                    this.Close();
                }
                return;
            }

            NotifyScale -= v;
            this.PrepareDrawing();
        }
    }
}
