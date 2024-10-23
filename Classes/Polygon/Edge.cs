using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public BezierControlPoint?[] bezierControlPoints = new BezierControlPoint?[2];
        // public (int dx, int dy)?[] bezierControlPointsSifts = new (int, int)?[2];

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
                case EdgeStates.Bezier:
                    CreateBezier();
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

        private void CreateBezier()
        {
            // bezierControlPoints[0] = new BezierControlPoint(start.position);
            bezierControlPoints[0] = new BezierControlPoint(start.position.X + 50, start.position.Y + 50);
            bezierControlPoints[1] = new BezierControlPoint(end.position.X - 50, end.position.Y - 50);
            // bezierControlPoints[3] = new BezierControlPoint(end.position);
        }

        public void DrawBezier(Graphics g, SolidBrush brush)
        {
            Pen pen = new Pen(brush);
            int steps = 1000;
            DrawControlPoints(g);
            PointF prevPoint = start.position;
            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;  // Calculate t in the range [0, 1]

                // Calculate the point on the Bézier curve using the cubic Bézier formula
                PointF currentPoint = GetBezierPoint( t);

                // Draw a line from the previous point to the current point
                g.DrawLine(pen, prevPoint, currentPoint);

                // Update previous point
                prevPoint = currentPoint;
            }
        }

        private PointF GetBezierPoint(float t)
        {
            float x = (float)(Math.Pow(1 - t, 3) * start.position.X +
                              3 * Math.Pow(1 - t, 2) * t * bezierControlPoints[0].position.X +
                              3 * (1 - t) * Math.Pow(t, 2) * bezierControlPoints[1].position.X +
                              Math.Pow(t, 3) * end.position.X);

            float y = (float)(Math.Pow(1 - t, 3) * start.position.Y +
                              3 * Math.Pow(1 - t, 2) * t * bezierControlPoints[0].position.Y +
                              3 * (1 - t) * Math.Pow(t, 2) * bezierControlPoints[1].position.Y +
                              Math.Pow(t, 3) * end.position.Y);

            return new PointF(x, y);
        }

        public void DrawControlPoints(Graphics g)
        {
            Pen dashedPen = new Pen(Color.DeepSkyBlue, 1);
            dashedPen.DashStyle = DashStyle.Dash;
            int radius = 3;
            Brush brush = Brushes.DeepSkyBlue;
            if (bezierControlPoints[0].isSelected == true) { brush = Brushes.Red; }
            g.FillEllipse(brush, bezierControlPoints[0].position.X - radius,
                    bezierControlPoints[0].position.Y - radius, 2 * radius, 2 * radius);

            if (bezierControlPoints[1].isSelected == true) { brush = Brushes.Red; }
            else { brush = Brushes.DeepSkyBlue; }
            g.FillEllipse(brush, bezierControlPoints[1].position.X - radius,
                    bezierControlPoints[1].position.Y - radius, 2 * radius, 2 * radius);

            g.DrawLine(dashedPen, start.position, bezierControlPoints[0].position);
            g.DrawLine(dashedPen, bezierControlPoints[1].position, bezierControlPoints[0].position);
            g.DrawLine(dashedPen, end.position, bezierControlPoints[1].position);


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
