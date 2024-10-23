using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes
{
    public class BezierControlPoint
    {
        public Point position;
        public bool isSelected = false;
        private int radius = 4;

        public BezierControlPoint(Point position)
        {
            this.position = position;
        }

        public BezierControlPoint(int x, int y)
        {
            position = new Point(x, y);
        }

        public void MovePoint(Point p)
        {
            int dx = p.X - position.X;
            int dy = p.Y - position.Y;

            MovePointDelta(dx, dy);

        } 

        public void MovePointDelta(int dx, int dy)
        {
            position.X += dx;
            position.Y += dy;
        }

        public bool CheckIsInArea(Point point)
        {
            if (((point.X >= position.X - radius) && (point.X <= position.X + radius))
                && ((point.Y >= position.Y - radius) && (point.Y <= position.Y + radius)))
            {
                return true;
            }
            return false;
        }


    }
}
