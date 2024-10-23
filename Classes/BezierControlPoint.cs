using PolygonRedactor.Classes.Polygon;
using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public BezierControlPoint(Vertex v, BezierControlPoint c, BezierStates state) 
        {
            int x = 0, y = 0;
            if (state == BezierStates.G1)
            {
                (x, y) = MakeForG1(v.position, c.position);
            }
            else
            {
                (x, y) = MakeForC1Bezier(v.position, c.position);
            }
            position = new Point(x, y);
        }

        public BezierControlPoint(Vertex v, Vertex c, BezierStates state)
        {
            int x = 0, y = 0;
            if (state == BezierStates.G1)
            {
                (x, y) = MakeForG1(v.position, c.position);
            }
            else
            {
                (x, y) = MakeForC1Edge(v.position, c.position);
            }
            position = new Point(x, y);
        }

        private (int, int) MakeForG1(Point p, Point pp)
        {
            int dx = p.X - pp.X;
            int dy = p.Y - pp.Y;
            int length = Edge.Distance(p, pp);
            int xChange = Convert.ToInt32(((double)dx / (double)length) * (100));
            int yChange = Convert.ToInt32(((double)dy / (double)length) * (100));
            return (p.X + xChange, p.Y + yChange);
        }

        private (int, int) MakeForC1Edge(Point p, Point pp)
        {
            int dx = p.X - pp.X;
            int dy = p.Y - pp.Y;
            int length = Edge.Distance(p, pp);
            int xChange = Convert.ToInt32(((double)dx / (double)length) * ((double)length / 3.0));
            int yChange = Convert.ToInt32(((double)dy / (double)length) * ((double)length / 3.0));
            return (p.X + xChange, p.Y + yChange);
        }

        private (int, int) MakeForC1Bezier(Point p, Point pp)
        {
            int dx = p.X - pp.X;
            int dy = p.Y - pp.Y;
            return (p.X + dx, p.Y + dy);
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
