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
