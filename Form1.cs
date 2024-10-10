namespace PolygonRedactor
{
    public partial class Form1 : Form
    {
        protected bool shouldDrawLine = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            shouldDrawLine = true;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (shouldDrawLine)
            {
                DrawBresenham(e.Graphics, new Point(100, 100), new Point(500, 500));
            }

        }

        private void DrawBresenham(Graphics g, Point start, Point end)
        {
            int dx = (end.X - start.X);
            int dy = (end.Y - start.Y);
            int pLess = 2 * dy;
            int pGreater = 2*(dy - dx);
            int p = 2 * dy - dx;          

            int x, y, xEnd;


            if (start.X > end.X)
            {
                x = end.X;
                y = end.Y;
                xEnd = start.X;
            }
            else
            {
                x = start.X;
                y = start.Y;
                xEnd = end.X;
            }
            SolidBrush brush = new SolidBrush(Color.Black);
            while (xEnd > x)
            {
                g.FillRectangle(brush, x, y, 1, 1);
                x++;
                if (p > 0) { p += pGreater; } 
                else { p += pLess; y++; }
                
            }
            
        }
    }
}
