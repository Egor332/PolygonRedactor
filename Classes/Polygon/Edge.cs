using PolygonRedactor.Enums;
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
        public EdgeStates state = EdgeStates.None;

        public Point? pressPoint = null;

        public bool isSelected = false;

        private double error = 4.0;

        public Edge(Vertex start, Vertex end)
        {
            this.start = start;
            this.end = end;
        }

        public void ChangeState(EdgeStates state)
        {
            this.state = state;
            start.rightConstraint = state;
            end.rightConstraint = state;
            switch (state)
            {
                case EdgeStates.Vertical:
                    SetVertical();
                    break;
                case EdgeStates.Horizontal:
                    SetHorizontal();
                    break;
                case EdgeStates.Fixed:
                    break;
                case EdgeStates.Broken:
                    break;
            }
        }

        private void SetVertical()
        {
            int dx = start.position.X - end.position.X;
            start.MovePointDelta(0 - (dx/2), 0);
            end.MovePointDelta((dx / 2) + (dx % 2), 0);
        }

        private void SetHorizontal()
        {
            int dy = start.position.Y - end.position.Y;
            start.MovePointDelta(0, 0 - (dy / 2));
            end.MovePointDelta(0, (dy / 2) + (dy % 2));
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
            //int dx = p.X - pressPoint.Value.X;
            //int dy = p.Y - pressPoint.Value.Y;

            //if (ValidatePositionOnAxis(start.position.X + dx, end.position.X + dx, width - 20)) 
            //{
            //    start.position.X += dx;
            //    end.position.X += dx;
            //}



            //if (ValidatePositionOnAxis(start.position.Y + dy, end.position.Y + dy, height - 35))
            //{
            //    start.position.Y += dy;
            //    end.position.Y += dy;
            //}

            start.MovePoint(p, pressPoint.Value);
            end.MovePoint(p, pressPoint.Value);
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
