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

        public void MovePoint(int x, int y)
        {
            int dx = x - this.position.X;
            int dy = y - this.position.Y;

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
            if ((leftConstraint == EdgeStates.Bezier) && (rightConstraint == EdgeStates.Bezier) && ((bezierState == BezierStates.C1) || (bezierState == BezierStates.G1)))
            {
                leftEdge.bezierControlPoints[1].MovePointDeltaEnforce(dx, dy);
                rightEdge.bezierControlPoints[0].MovePointDeltaEnforce(dx, dy);
            }
            if (leftEdge != null)
            {
                if ((leftConstraint == EdgeStates.Horizontal) && (dy != 0))
                {
                    leftEdge.start.DoLeftCycle(0, position.Y - leftEdge.start.position.Y);
                }
                if ((leftConstraint == EdgeStates.Vertical) && (dx != 0))
                {
                    leftEdge.start.DoLeftCycle(position.X - leftEdge.start.position.X, 0);
                }
                if (leftConstraint == EdgeStates.Fixed)
                {
                    (int xChange, int yChange) = DoFixedEdge(leftEdge.start, leftEdge.length.Value);
                    leftEdge.start.DoLeftCycle(xChange, yChange);
                }                
            }
            if (rightEdge != null)
            {
                if ((rightConstraint == EdgeStates.Horizontal) && (dy != 0))
                {
                    rightEdge.end.DoRightCycle(0, position.Y - rightEdge.end.position.Y);
                }
                if ((rightConstraint == EdgeStates.Vertical) && (dx != 0))
                {
                    rightEdge.end.DoRightCycle(position.X - rightEdge.end.position.X, 0);
                }
                if ((rightConstraint == EdgeStates.Fixed))
                {
                    (int xChange, int yChange) = DoFixedEdge(rightEdge.end, rightEdge.length.Value);
                    rightEdge.end.DoRightCycle(xChange, yChange);
                }
                
            }


            
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
                leftEdge.bezierControlPoints[1].MovePointDeltaEnforce(dx, dy);
            }
            if ((leftConstraint == EdgeStates.Bezier) && (rightConstraint == EdgeStates.Bezier) && ((bezierState == BezierStates.C1) || (bezierState == BezierStates.G1)))
            {
                leftEdge.bezierControlPoints[1].MovePointDeltaEnforce(dx, dy);
                rightEdge.bezierControlPoints[0].MovePointDeltaEnforce(dx, dy);
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
            if ((leftConstraint == EdgeStates.Bezier) && (rightConstraint == EdgeStates.Bezier) && ((bezierState == BezierStates.C1) || (bezierState == BezierStates.G1)))
            {
                leftEdge.bezierControlPoints[1].MovePointDeltaEnforce(dx, dy);
                rightEdge.bezierControlPoints[0].MovePointDeltaEnforce(dx, dy);
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
            MoveConnectedControlPointsLeft(dx, dy);
            MoveConnectedControlPointsRight(dx, dy);
        }

        private void MoveConnectedControlPointsLeft(int dx, int dy)
        {
            if (leftConstraint == EdgeStates.Bezier)
            {
                //int x = leftEdge.bezierControlPoints[1].position.X;
                //int y = leftEdge.bezierControlPoints[1].position.Y;
                leftEdge.bezierControlPoints[1].MovePointDeltaEnforce(dx, dy);
            }
        }
        private void MoveConnectedControlPointsRight(int dx, int dy)
        {
            if (rightConstraint == EdgeStates.Bezier)
            {
                //int x = rightEdge.bezierControlPoints[0].position.X;
                //int y = rightEdge.bezierControlPoints[0].position.Y;
                rightEdge.bezierControlPoints[0].MovePointDeltaEnforce(dx, dy);
            }
        }

        public void SetBezierState(BezierStates state)
        {
            bezierState = state;
            if ((leftConstraint == EdgeStates.Bezier) && (rightConstraint == EdgeStates.Bezier) && ((bezierState == BezierStates.C1) || (bezierState == BezierStates.G1)))
            {
                leftEdge.bezierControlPoints[1].MovePointEnforce(position.X + 100, position.Y + 100);
                rightEdge.bezierControlPoints[0].MovePointEnforce(position.X - 100, position.Y - 100);
            }
        }
    }
}
