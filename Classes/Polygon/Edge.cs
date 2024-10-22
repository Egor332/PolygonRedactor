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
        public int? length = null;

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
            this.state = EdgeStates.None;
            start.rightConstraint = EdgeStates.None;
            end.leftConstraint = EdgeStates.None;
            switch (state)
            {
                case EdgeStates.Vertical:
                    SetVertical();
                    break;
                case EdgeStates.Horizontal:
                    SetHorizontal();
                    break;
                case EdgeStates.Fixed:
                    if (!SetFixed()) return;
                    break;
                case EdgeStates.Broken:
                    break;
            }

            this.state = state;
            start.rightConstraint = state;
            start.rightEdge = this;
            end.leftConstraint = state;
            end.leftEdge = this;
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

        private bool SetFixed()
        {
            string lenToForm = (Distance(start.position, end.position)).ToString();
            SetLengthForm setLengthForm = new SetLengthForm(lenToForm);
            string? buf = setLengthForm.Show();
            if (buf != null) 
            {                
                ChangeLength(Int32.Parse(buf));
                length = Distance(start.position, end.position);
                return true;
            }
            return false;
        }

        private void ChangeLength(int toSet)
        {
            int currentLength = Distance(start.position, end.position);
            int dx = end.position.X - start.position.X;
            int dy = end.position.Y - start.position.Y;
            double cos = (double)dx / (double)currentLength;
            double sin = (double)dy / (double)currentLength;
            int xChange = Convert.ToInt32(Math.Round(cos * (toSet - currentLength)));
            int yChange = Convert.ToInt32(Math.Round(sin * (toSet - currentLength)));
            start.MovePointDelta(0 - xChange/2, 0 - yChange/2);
            end.MovePointDelta((xChange / 2) + (xChange % 2), (yChange / 2) + (yChange % 2));
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

        public static int Distance(Point p1, Point p2)
        {
            return Convert.ToInt32(Math.Floor(Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2))));
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

            // moving edge will keep it constrains
            //EdgeStates stateBuffer = state;
            //start.rightConstraint = EdgeStates.None;
            //end.leftConstraint = EdgeStates.None;
            //state = EdgeStates.None;

            start.MovePoint(p, pressPoint.Value);            
            end.MovePoint(p, pressPoint.Value);

            //state = stateBuffer;
            //start.rightConstraint = stateBuffer;
            //end.leftConstraint = stateBuffer;

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
