namespace OnlineWhiteboard.Models
{
    public enum DrawTool
    {
        Pen,
        Line,
        Rectangle,
        Ellipse,
        Clear
    }

    public class DrawPoint
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public string Color { get; set; }
        public int Thickness { get; set; }
        public DrawTool Tool { get; set; }
    }
}
