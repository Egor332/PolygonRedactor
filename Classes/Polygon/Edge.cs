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

        public bool isSelected;

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

        public void MoveEdge(Point p)
        {            
            int dx = p.X - pressPoint.Value.X;
            int dy = p.Y - pressPoint.Value.Y;

            start.position.X += dx;
            start.position.Y += dy;

            end.position.X += dx;
            end.position.Y += dy;
        }

        
    }
}
