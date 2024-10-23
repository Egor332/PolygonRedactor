using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.Polygon
{
    public class Vertex
    {
        public Point position;
        public int radius = 3;
        public bool isSelected = false;
        public bool used = false;

        public EdgeStates leftConstraint = EdgeStates.None;
        public Edge? leftEdge = null;
        public EdgeStates rightConstraint = EdgeStates.None;
        public Edge? rightEdge = null;

        public BezierStates bezierState = BezierStates.G1;

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
            position.X += dx;
            position.Y += dy;
            if (leftEdge != null)
            {
                if ((leftConstraint == EdgeStates.Horizontal) && (dy != 0))
                {
                    leftEdge.start.MovePointDelta(0, position.Y - leftEdge.start.position.Y);
                }
                if ((leftConstraint == EdgeStates.Vertical) && (dx != 0))
                {
                    leftEdge.start.MovePointDelta(position.X - leftEdge.start.position.X, 0);
                }
                if (leftConstraint == EdgeStates.Fixed)
                {
                    (int xChange, int yChange) = DoFixedEdge(leftEdge.start, leftEdge.length.Value);
                    leftEdge.start.DoLeftCycle(xChange, yChange);
                }
                if (leftConstraint == EdgeStates.Bezier)
                {
                    leftEdge.bezierControlPoints[1].MovePointDelta(dx, dy);
                }
            }
            if (rightEdge != null)
            {
                if ((rightConstraint == EdgeStates.Horizontal) && (dy != 0))
                {
                    rightEdge.end.MovePointDelta(0, position.Y - rightEdge.end.position.Y);
                }
                if ((rightConstraint == EdgeStates.Vertical) && (dx != 0))
                {
                    rightEdge.end.MovePointDelta(position.X - rightEdge.end.position.X, 0);
                }
                if ((rightConstraint == EdgeStates.Fixed))
                {
                    (int xChange, int yChange) = DoFixedEdge(rightEdge.end, rightEdge.length.Value);
                    rightEdge.end.DoRightCycle(xChange, yChange);
                }
                if (rightConstraint == EdgeStates.Bezier)
                {
                    rightEdge.bezierControlPoints[0].MovePointDelta(dx, dy);
                }
            }
            //if ((leftConstraint == EdgeStates.Horizontal) && (dy != 0))
            //{
            //    leftEdge.start.MovePointDelta(0, dy);
            //}
            //if ((leftConstraint == EdgeStates.Vertical) && (dx != 0))
            //{
            //    leftEdge.start.MovePointDelta(dx, 0);
            //}
            //if (leftConstraint == EdgeStates.Fixed)
            //{
            //    DoFixedEdge(leftEdge.start, leftEdge.length.Value);
            //}
            //if ((rightConstraint == EdgeStates.Horizontal) && (dy != 0))
            //{
            //    rightEdge.end.MovePointDelta(0, dy);
            //}
            //if ((rightConstraint == EdgeStates.Vertical) && (dx != 0))
            //{
            //    rightEdge.end.MovePointDelta(dx, 0);
            //}
            //if ((rightConstraint == EdgeStates.Fixed))
            //{
            //    DoFixedEdge(rightEdge.end, rightEdge.length.Value); 
            //}

            
        }

        private void DoLeftCycle(int dx, int dy)
        {
            if (used) return;
            position.X += dx;
            position.Y += dy;
            if ((leftConstraint == EdgeStates.Horizontal) && (dy != 0))
            {
                leftEdge.start.MovePointDelta(0, position.Y - leftEdge.start.position.Y);
            }
            if ((leftConstraint == EdgeStates.Vertical) && (dx != 0))
            {
                leftEdge.start.MovePointDelta(position.X - leftEdge.start.position.X, 0);
            }
            if (leftConstraint == EdgeStates.Fixed)
            {
                (int xChange, int yChange) = DoFixedEdge(leftEdge.start, leftEdge.length.Value);
                leftEdge.start.DoLeftCycle(xChange, yChange);
            }
            if (leftConstraint == EdgeStates.Bezier) 
            {
                leftEdge.bezierControlPoints[1].MovePointDelta(dx, dy);
            }
        }

        private void DoRightCycle(int dx, int dy)
        {
            if (used) return;
            position.X += dx;
            position.Y += dy;
            if ((rightConstraint == EdgeStates.Horizontal) && (dy != 0))
            {
                rightEdge.end.MovePointDelta(0, position.Y - rightEdge.end.position.Y);
            }
            if ((rightConstraint == EdgeStates.Vertical) && (dx != 0))
            {
                rightEdge.end.MovePointDelta(position.X - rightEdge.end.position.X, 0);
            }
            if ((rightConstraint == EdgeStates.Fixed))
            {
                (int xChange, int yChange) = DoFixedEdge(rightEdge.end, rightEdge.length.Value);
                rightEdge.end.DoRightCycle(xChange, yChange);
            }
            if (rightConstraint == EdgeStates.Bezier)
            {
                rightEdge.bezierControlPoints[0].MovePointDelta(dx, dy);
            }
        }

        private (int dx, int dy) DoFixedEdge(Vertex v, int length)
        {
            int currentLength = Edge.Distance(position, v.position);
            int dx = v.position.X - position.X;
            int dy = v.position.Y - position.Y;
            double cos = (double)dx / (double)currentLength;
            double sin = (double)dy / (double)currentLength;
            int xChange = Convert.ToInt32(Math.Round(cos * (length - currentLength)));
            int yChange = Convert.ToInt32(Math.Round(sin * (length - currentLength)));
            return (xChange, yChange);
        }

        public void MovePointEnforce(Point p, Point start)
        {
            int dx = p.X - start.X;
            int dy = p.Y - start.Y;
            position.X += dx;
            position.Y += dy;
            if (leftConstraint == EdgeStates.Bezier)
            {
                leftEdge.bezierControlPoints[1].MovePointDelta(dx, dy);
            }
            if (rightConstraint == EdgeStates.Bezier)
            {
                rightEdge.bezierControlPoints[0].MovePointDelta(dx, dy);
            }
        }

        public void SetBezierState(BezierStates state)
        {
            bezierState = state;
        }
    }
}
