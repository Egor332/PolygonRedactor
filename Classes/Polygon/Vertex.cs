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
        public bool used = false;

        public EdgeStates leftConstraint = EdgeStates.None;
        public Edge? leftEdge = null;
        public EdgeStates rightConstraint = EdgeStates.None;
        public Edge? rightEdge = null;

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
            if (used)
            {
                return;
            }
            used = true;
            if ((leftConstraint == EdgeStates.Horizontal) && (dy != 0))
            {
                leftEdge.start.MovePointDelta(0, dy);
            }
            if ((leftConstraint == EdgeStates.Vertical) && (dx != 0))
            {
                leftEdge.start.MovePointDelta(dx, 0);
            }
            if ((rightConstraint == EdgeStates.Horizontal) && (dy != 0))
            {
                rightEdge.end.MovePointDelta(0, dy);
            }
            if ((rightConstraint == EdgeStates.Vertical) && (dx != 0))
            {
                rightEdge.end.MovePointDelta(dx, 0);
            }

            position.X += dx;
            position.Y += dy;
        }

        public void MovePointEnforce(Point p, Point start)
        {
            int dx = p.X - start.X;
            int dy = p.Y - start.Y;
            position.X += dx;
            position.Y += dy;
        }
    }
}
