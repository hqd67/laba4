using OnlineWhiteboard.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OnlineWhiteboard
{
    public class Form1 : Form
    {
        PictureBox pictureBox;
        Bitmap canvas;
        Graphics graphics;

        bool drawing;
        Point start;
        Color currentColor = Color.Black;
        DrawTool currentTool = DrawTool.Pen;

        public Form1()
        {
            Text = "Online Whiteboard";
            Width = 900;
            Height = 600;

            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            Controls.Add(pictureBox);

            canvas = new Bitmap(900, 600);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White);
            pictureBox.Image = canvas;

            pictureBox.MouseDown += MouseDown;
            pictureBox.MouseUp += MouseUp;
            pictureBox.MouseMove += MouseMove;
        }

        void MouseDown(object s, MouseEventArgs e)
        {
            drawing = true;
            start = e.Location;
        }

        void MouseUp(object s, MouseEventArgs e)
        {
            drawing = false;
            using Pen pen = new Pen(currentColor, 3);
            graphics.DrawLine(pen, start, e.Location);
            pictureBox.Invalidate();
        }

        void MouseMove(object s, MouseEventArgs e)
        {
            if (!drawing) return;
            using Pen pen = new Pen(currentColor, 3);
            graphics.DrawEllipse(pen, e.X, e.Y, 2, 2);
            pictureBox.Invalidate();
        }
    }
}
