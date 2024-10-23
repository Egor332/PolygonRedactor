using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes
{
    public class Bresenham
    {
        public System.Drawing.Graphics g;
        public System.Drawing.SolidBrush brush;

        public Bresenham() 
        { 
            // g;
            brush = new SolidBrush(Color.Black);
        }

        public void Draw(Point start, Point end)
        {
            int x0, x1, y0, y1;
            x0 = start.X;
            y0 = start.Y;
            x1 = end.X;
            y1 = end.Y;
            if (Math.Abs(x1 - x0) > Math.Abs(y1 - y0))
            {
                if (x1 > x0)
                {
                    BresenhamLow(x0, x1, y0, y1);
                }
                else
                {
                    BresenhamLow(x1, x0, y1, y0);
                }
            }
            else
            {
                if (y1 > y0)
                {
                    BresenhamHigh(x0, x1, y0, y1);
                }
                else
                {
                    BresenhamHigh(x1, x0, y1, y0);
                }
            }
        }

        private void BresenhamLow(int x0, int x1, int y0, int y1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int D = 2 * dy - dx;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                g.FillRectangle(brush, x, y, 1, 1);
                if (D > 0)
                {
                    y = y + yi;
                    D += 2 * dy - 2 * dx;
                }
                else
                {
                    D += 2 * dy;
                }
            }
        }

        private void BresenhamHigh(int x0, int x1, int y0, int y1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            int D = 2 * dx - dy;
            int x = x0;
            for (int y = y0; y<= y1; y++)
            {
                g.FillRectangle(brush, x, y, 1, 1);
                if (D > 0)
                {
                    x += xi;
                    D += 2 * dx - 2 * dy; 
                }
                else
                {
                    D += 2 * dx;
                }
            }
        }
    }
}
