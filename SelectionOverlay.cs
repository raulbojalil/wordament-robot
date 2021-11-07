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
        Point _startPos;
        Point _currentPos;
        bool _drawing;
        string _message;

        public SelectionOverlay(string message)
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
            _message = message;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }

        public Rectangle GetSelectionArea()
        {
            return new Rectangle(
                Math.Min(_startPos.X, _currentPos.X),
                Math.Min(_startPos.Y, _currentPos.Y),
                Math.Abs(_startPos.X - _currentPos.X),
                Math.Abs(_startPos.Y - _currentPos.Y));
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            _currentPos = _startPos = e.Location;
            _drawing = true;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            _currentPos = e.Location;
            if (_drawing) this.Invalidate();
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (var brush = new SolidBrush(Color.Black))
            {
                var font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                var messageSize = e.Graphics.MeasureString(_message, font);

                e.Graphics.DrawString(_message,
                    font,
                    brush,
                    new PointF(
                        (Screen.PrimaryScreen.Bounds.Width / 2) - (messageSize.Width / 2), 
                        Screen.PrimaryScreen.Bounds.Height / 2)
                );
            }            
            if (_drawing) 
                e.Graphics.DrawRectangle(Pens.Red, GetSelectionArea());
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ShowInTaskbar = false;
            ResumeLayout(false);
        }
    }
}
