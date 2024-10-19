using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.Polygon
{
    internal class Vertex
    {
        public Point position;
        public int radius = 3;
        public bool isSelected = false;

        public EdgeStates leftConstraint = EdgeStates.None;
        public EdgeStates rightConstraint = EdgeStates.None;

        public Vertex(Point position)
        {
            this.position = position;
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

        public void MovePoint(Point p, Point start)
        {
            int dx = p.X - start.X;
            int dy = p.Y - start.Y;

            MovePointDelta(dx, dy);
        }

        public void MovePointDelta(int dx, int dy)
        {
            position.X += dx;
            position.Y += dy;
        }
    }
}
