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
        public bool isWu = false;

        public Bresenham() 
        { 
            // g;
            brush = new SolidBrush(Color.Black);
        }

        public void Draw(Point start, Point end)
        {
            if (isWu)
            {
                DrawWuAntialiasedLine(start, end);
                return;
            }
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

        public void DrawWuAntialiasedLine(Point start, Point end)
        {
            int x0, x1, y0, y1;
            x0 = start.X;
            y0 = start.Y;
            x1 = end.X;
            y1 = end.Y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            
            if (steep)
            {
                int temp = x0; x0 = y0; y0 = temp;
                temp = x1; x1 = y1; y1 = temp;
            }

            if (x0 > x1)
            {
                int temp = x0; x0 = x1; x1 = temp;
                temp = y0; y0 = y1; y1 = temp;
            }

            int dx = x1 - x0;
            int dy = y1 - y0;
            float gradient = dy / (float)dx;

            float xEnd = x0;
            float yEnd = y0 + gradient * (xEnd - x0);
            float xGap = 1 - (xEnd - (int)xEnd);

            int xPixel1 = (int)xEnd;
            int yPixel1 = (int)yEnd;

            if (steep)
            {
                PlotWu(yPixel1, xPixel1, (1 - (yEnd - yPixel1)) * xGap);
                PlotWu(yPixel1 + 1, xPixel1, (yEnd - yPixel1) * xGap);
            }
            else
            {
                PlotWu(xPixel1, yPixel1, (1 - (yEnd - yPixel1)) * xGap);
                PlotWu(xPixel1, yPixel1 + 1, (yEnd - yPixel1) * xGap);
            }

            // Draw the main line
            float y = yEnd + gradient;
            for (int x = xPixel1 + 1; x < x1; x++)
            {
                int yInt = (int)y;
                float yFrac = y - yInt;

                if (steep)
                {
                    PlotWu(yInt, x, 1 - yFrac);
                    PlotWu(yInt + 1, x, yFrac);
                }
                else
                {
                    PlotWu(x, yInt, 1 - yFrac);
                    PlotWu(x, yInt + 1, yFrac);
                }

                y += gradient;
            }
        }

        private void PlotWu(int x, int y, float brightness)
        {
            // Create a color with adjusted alpha for smooth transitions
            Color color = Color.FromArgb((int)(brightness * 255), ((SolidBrush)brush).Color);
            Brush adjustedBrush = new SolidBrush(color);

            // Draw pixel with adjusted brightness
            g.FillRectangle(adjustedBrush, x, y, 1, 1);
            adjustedBrush.Dispose();
        }
    }
}
