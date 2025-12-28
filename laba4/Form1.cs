using OnlineWhiteboard.Models;
using OnlineWhiteboard.Network;
using System;
using System.Drawing;
using System.Text.Json;
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

        NetworkManager network = new NetworkManager();

        public Form1()
        {
            Text = "Online Whiteboard";
            Width = 900;
            Height = 600;

            CreateToolbar();
            CreateCanvas();

            network.OnMessage += OnNetworkMessage;
        }

        void CreateCanvas()
        {
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            Controls.Add(pictureBox);

            canvas = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White);
            pictureBox.Image = canvas;

            pictureBox.MouseDown += MouseDown;
            pictureBox.MouseUp += MouseUp;
            pictureBox.MouseMove += MouseMove;
        }

        void CreateToolbar()
        {
            Panel panel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top
            };
            Controls.Add(panel);

            Button btnPen = NewButton("✏️", (s, e) => currentTool = DrawTool.Pen);
            Button btnLine = NewButton("📏", (s, e) => currentTool = DrawTool.Line);
            Button btnRect = NewButton("⬛", (s, e) => currentTool = DrawTool.Rectangle);
            Button btnEllipse = NewButton("⚪", (s, e) => currentTool = DrawTool.Ellipse);
            Button btnColor = NewButton("Цвет", ColorClick);
            Button btnClear = NewButton("Очистить", ClearClick);
            Button btnHost = NewButton("Host", (s, e) => network.StartServer(5000));
            Button btnJoin = NewButton("Join", (s, e) => network.Connect("127.0.0.1", 5000));

            panel.Controls.AddRange(new Control[]
            {
                btnPen, btnLine, btnRect, btnEllipse,
                btnColor, btnClear, btnHost, btnJoin
            });

            int x = 5;
            foreach (Control c in panel.Controls)
            {
                c.Location = new Point(x, 10);
                x += c.Width + 5;
            }
        }

        Button NewButton(string text, EventHandler click)
        {
            Button b = new Button
            {
                Text = text,
                Width = 70,
                Height = 30
            };
            b.Click += click;
            return b;
        }

        void ColorClick(object s, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
                currentColor = cd.Color;
        }

        void ClearClick(object s, EventArgs e)
        {
            DrawPoint dp = new DrawPoint { Tool = DrawTool.Clear };
            Draw(dp);
            network.Send(dp);
        }

        void MouseDown(object s, MouseEventArgs e)
        {
            drawing = true;
            start = e.Location;
        }

        void MouseUp(object s, MouseEventArgs e)
        {
            drawing = false;

            DrawPoint dp = new DrawPoint
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = e.X,
                Y2 = e.Y,
                Color = currentColor.Name,
                Thickness = 3,
                Tool = currentTool
            };

            Draw(dp);
            network.Send(dp);
        }

        void MouseMove(object s, MouseEventArgs e)
        {
            if (!drawing || currentTool != DrawTool.Pen) return;

            DrawPoint dp = new DrawPoint
            {
                X1 = e.X,
                Y1 = e.Y,
                X2 = e.X,
                Y2 = e.Y,
                Color = currentColor.Name,
                Thickness = 3,
                Tool = DrawTool.Pen
            };

            Draw(dp);
            network.Send(dp);
        }

        void OnNetworkMessage(string json)
        {
            DrawPoint dp = JsonSerializer.Deserialize<DrawPoint>(json);
            Invoke(() => Draw(dp));
        }

        void Draw(DrawPoint d)
        {
            if (d.Tool == DrawTool.Clear)
            {
                graphics.Clear(Color.White);
                pictureBox.Invalidate();
                return;
            }

            using Pen pen = new Pen(Color.FromName(d.Color), d.Thickness);

            switch (d.Tool)
            {
                case DrawTool.Pen:
                    graphics.DrawEllipse(pen, d.X1, d.Y1, 2, 2);
                    break;

                case DrawTool.Line:
                    graphics.DrawLine(pen, d.X1, d.Y1, d.X2, d.Y2);
                    break;

                case DrawTool.Rectangle:
                    graphics.DrawRectangle(pen,
                        Math.Min(d.X1, d.X2),
                        Math.Min(d.Y1, d.Y2),
                        Math.Abs(d.X2 - d.X1),
                        Math.Abs(d.Y2 - d.Y1));
                    break;

                case DrawTool.Ellipse:
                    graphics.DrawEllipse(pen,
                        Math.Min(d.X1, d.X2),
                        Math.Min(d.Y1, d.Y2),
                        Math.Abs(d.X2 - d.X1),
                        Math.Abs(d.Y2 - d.Y1));
                    break;
            }

            pictureBox.Invalidate();
        }
    }
}
