using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.Polygon
{
    internal class Edge
    {
        public Vertex start;
        public Vertex end;

        public Point? pressPoint = null;

        public bool isSelected = false;

        private double error = 8.0;

        public Edge(Vertex start, Vertex end)
        {
            this.start = start;
            this.end = end;
        }

        public bool IsOnEdge(Point point)
        {
            if ((Distance(start.position, point) + Distance(end.position, point) < Distance(start.position, end.position) + error) &&
                ((Distance(start.position, point) + Distance(end.position, point) > Distance(start.position, end.position) - error))) 
            {
                Vertex a = new Vertex(start.position);
                a.radius = 10;
                if (a.CheckIsInArea(point)) return false;
                a.position = end.position;
                if (a.CheckIsInArea(point)) return false;
                return true;
            }
            return false;
        }

        public double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
        }

        public void MoveEdge(Point p, int width, int height)
        {            
            int dx = p.X - pressPoint.Value.X;
            int dy = p.Y - pressPoint.Value.Y;

            if (ValidatePositionOnAxis(start.position.X + dx, end.position.X + dx, width - 20)) 
            {
                start.position.X += dx;
                end.position.X += dx;
            }



            if (ValidatePositionOnAxis(start.position.Y + dy, end.position.Y + dy, height - 35))
            {
                start.position.Y += dy;
                end.position.Y += dy;
            }
        }

        private bool ValidatePositionOnAxis(int x1, int x2, int size)
        {
            if ((x1 < 0) || (x2 < 0) || (x1 > size) || (x2 > size))
            {
                return false;
            }
            return true;
        }
        
    }
}
