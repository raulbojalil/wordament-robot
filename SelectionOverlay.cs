using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordamentCheater
{
    public class SelectionOverlay : Form
    {
        Point startPos;
        Point currentPos;
        bool drawing;

        public SelectionOverlay()
        {
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            BackColor = Color.White;
            Opacity = 0.75;
            Cursor = Cursors.Cross;
            MouseDown += Form_MouseDown;
            MouseMove += Form_MouseMove;
            MouseUp += Form_MouseUp;
            Paint += Form_Paint;
            KeyDown += Form_KeyDown;
            DoubleBuffered = true;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y));
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            currentPos = startPos = e.Location;
            drawing = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            currentPos = e.Location;
            if (drawing) this.Invalidate();
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (drawing) e.Graphics.DrawRectangle(Pens.Red, GetRectangle());
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ShowInTaskbar = false;
            ResumeLayout(false);
        }
    }
}
