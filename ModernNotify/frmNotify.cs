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

        private int[] CloseOverlay = new int[] { 0x60, 0x00, 0x30 };
        private Image CloseImage => Properties.Resources.CloseButton;
        private int CloseState = 0;

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

            // TopMost
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
                        new Rectangle(10, 10, this.Source.IconSize, this.Source.IconSize)
                    );

                    SizeF sz = g.MeasureString(this.Source.Title, this.Font, 302 - this.Source.IconSize);
                    g.DrawString(
                        this.Source.Title,
                        this.Font,
                        new SolidBrush(Extension.RgbColor(0xffffff)),
                        new Point(18 + this.Source.IconSize, 6)
                    );

                    g.DrawString(
                        this.Source.Content,
                        this.Font,
                        new SolidBrush(Extension.RgbColor(0xa5a5a5)),
                        new Rectangle(18 + this.Source.IconSize, 6 + (int)sz.Height - 2, 302 - this.Source.IconSize, this.Height - 28)
                    );
                }

                Graphics g2 = e.Graphics;

                g2.TranslateTransform(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
                g2.ScaleTransform(NotifyScale, NotifyScale);
                g2.TranslateTransform(-buffer.Width / 2, -buffer.Height / 2);

                g2.DrawImage(buffer, 0, 0);
                g2.ResetTransform();

                if (MouseEntered)
                {
                    g2.DrawImage(CloseImage, 360 - 24 + 2, 8 + 2, 12, 12);
                    g2.FillRectangle(
                        new SolidBrush(Color.FromArgb(CloseOverlay[CloseState], this.BackColor)),
                        new Rectangle(360 - 24, 8, 16, 16)
                    );
                }
            }
        }

        private void frmNotify_Resize(object sender, EventArgs e)
        {
            this.PrepareDrawing();
        }

        private void PrepareDrawing()
        {
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));

            SizeF contentSize = g.MeasureString(
                this.Source.Title + Environment.NewLine + this.Source.Content,
                this.Font,
                326 - this.Source.IconSize
            );

            Rectangle rc = Screen.PrimaryScreen.WorkingArea; // Screen size

            int Left = rc.Right - 360;
            int Top = rc.Bottom - this.Height - 16;
            int Height = Math.Max((int)contentSize.Height + 20, this.Source.IconSize + 36);

            this.Size = new Size(360 - EnterX + EnterXDelta, Height);
            this.Location = new Point(Left + EnterX, Top);

            this.Invalidate();
        }

        private void frmNotify_MouseEnter(object sender, EventArgs e)
        {
            MouseEntered = true;
            this.PrepareDrawing();

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
            if (IsInClose(e.X, e.Y))
            {
                CloseState = 2;
                this.PrepareDrawing();
                return;
            }

            if (Processed) return;
            ScaleDir = true;
        }

        private void frmNotify_MouseUp(object sender, MouseEventArgs e)
        {
            if (CloseState == 2)
            {
                if (IsInClose(e.X, e.Y)) // Click
                    this.Close();

                CloseState = 0;
                this.PrepareDrawing();
                return;
            }

            if (Processed) return;
            Processed = true;

            ScaleDir = false;
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

        private bool IsInClose(int X, int Y)
        {
            return X >= 360 - 24 && Y >= 8 && X <= 360 - 8 && Y <= 24;
        }

        private void frmNotify_MouseMove(object sender, MouseEventArgs e)
        {
            var p = CloseState;
            if (IsInClose(e.X, e.Y))
            {
                CloseState = (CloseState == 2) ? 2 : 1;
                if (p != CloseState) this.PrepareDrawing();
            }
            else
            {
                CloseState = (CloseState == 2) ? 2 : 0;
                if (p != CloseState) this.PrepareDrawing();
            }
        }
    }
}
