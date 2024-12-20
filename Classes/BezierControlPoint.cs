﻿using PolygonRedactor.Classes.Polygon;
using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        public int length = 0;
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

        public void MovePoint(Point p, Vertex v)
        {
            int dx = p.X - position.X;
            int dy = p.Y - position.Y;

            MovePointDelta(dx, dy, v);

        } 

        public void MovePointDelta(int dx, int dy, Vertex v)
        {
            position.X += dx;
            position.Y += dy;
            if (v.bezierState == BezierStates.G0) return;

            // Only constraints left
            if (v.leftEdge.bezierControlPoints[1] == this) DoMoveRight(dx, dy, v);
            else DoMoveLeft(dx, dy, v);

        }

        private void DoMoveLeft(int dx, int dy, Vertex v)
        {
            if (v.leftEdge.state == EdgeStates.Bezier)
            {
                if (v.bezierState == BezierStates.G1) 
                {
                    KeepG1Bezier(v, v.leftEdge.bezierControlPoints[1]);
                }
                else
                {
                    int changeX = position.X - v.position.X;
                    int changeY = position.Y - v.position.Y;
                    v.leftEdge.bezierControlPoints[1].MovePointEnforce(v.position.X - changeX, v.position.Y - changeY);
                }
            }
            else
            {
                if (v.bezierState == BezierStates.G1)
                {
                    if (v.leftConstraint == EdgeStates.Vertical) 
                    {                        
                        this.position.X -= dx;
                        if ((this.position.Y < v.position.Y) == (v.leftEdge.start.position.Y < v.position.Y))
                        {
                            int swipe = v.position.Y - v.leftEdge.start.position.Y;
                            v.leftEdge.start.MovePointDelta(0, 2 * swipe);
                        }

                    }
                    else if (v.leftConstraint == EdgeStates.Horizontal) 
                    { 
                        this.position.Y -= dy;
                        if ((this.position.X < v.position.X) == (v.leftEdge.start.position.X < v.position.X))
                        {
                            int swipe = v.position.X - v.leftEdge.start.position.X;
                            v.leftEdge.start.MovePointDelta(2 * swipe, 0);
                        }
                    }
                    else KeepG1Vertex(v, v.leftEdge.start, v.leftEdge.length.Value);
                }
                else // C1
                {
                    if (v.leftConstraint == EdgeStates.Vertical) { this.position.X -= dx;
                        v.leftEdge.start.MovePointDelta(0, -3*dy); }
                    else if (v.leftConstraint == EdgeStates.Horizontal) { this.position.Y -= dy;
                        v.leftEdge.start.MovePointDelta(-dx * 3, 0); }
                    else KeepC1Vertex(v, v.leftEdge.start, v.leftEdge);
                }
            }
        }

        private void DoMoveRight(int dx, int dy, Vertex v)
        {
            if (v.rightEdge.state == EdgeStates.Bezier)
            {
                if (v.bezierState == BezierStates.G1)
                {
                    KeepG1Bezier(v, v.rightEdge.bezierControlPoints[0]);
                }
                else
                {
                    int changeX = position.X - v.position.X;
                    int changeY = position.Y - v.position.Y;
                    v.rightEdge.bezierControlPoints[0].MovePointEnforce(v.position.X - changeX, v.position.Y - changeY);
                }
            }
            else
            {
                if (v.bezierState == BezierStates.G1)
                {
                    if (v.rightConstraint == EdgeStates.Vertical)
                    {
                        this.position.X -= dx;
                        if ((this.position.Y < v.position.Y) == (v.rightEdge.end.position.Y < v.position.Y))
                        {
                            int swipe = v.position.Y - v.rightEdge.end.position.Y;
                            v.rightEdge.end.MovePointDelta(0, 2 * swipe);
                        }
                    }
                    else if (v.rightConstraint == EdgeStates.Horizontal)
                    {
                        this.position.Y -= dy;
                        if ((this.position.X < v.position.X) == (v.rightEdge.end.position.X < v.position.X))
                        {
                            int swipe = v.position.X - v.rightEdge.end.position.X;
                            v.rightEdge.end.MovePointDelta(2 * swipe, 0);
                        }
                    }

                    else KeepG1Vertex(v, v.rightEdge.end, v.rightEdge.length.Value);
                }
                else // C1
                {
                    if (v.rightConstraint == EdgeStates.Vertical) { this.position.X -= dx;
                        v.rightEdge.end.MovePointDelta(0, -dy*3); }
                    else if (v.rightConstraint == EdgeStates.Horizontal) { this.position.Y -= dy;
                        v.rightEdge.end.MovePointDelta(-3*dx, 0); }
                    else KeepC1Vertex(v, v.rightEdge.end, v.rightEdge);
                }
            }
        }

        private void KeepC1Vertex(Vertex v, Vertex c, Edge e)
        {
            int dx = this.position.X - v.position.X;
            int dy = this.position.Y - v.position.Y;
            int lengthV = Edge.Distance(v.position, this.position);
            if (e.state == EdgeStates.Fixed)
            {
                position.X = v.position.X + Convert.ToInt32((((double)dx / (double)lengthV) * ((double)e.length / 3)));
                position.Y = v.position.Y + Convert.ToInt32((((double)dy / (double)lengthV) * ((double)e.length / 3)));
                int xChange = Convert.ToInt32((((double)dx / (double)lengthV) * (e.length)));
                int yChange = Convert.ToInt32((((double)dy / (double)lengthV) * (e.length)));
                c.MovePoint(v.position.X - xChange, v.position.Y - yChange);
            }
            else
            {
                int xChange = Convert.ToInt32((((double)dx / (double)lengthV) * (lengthV*3)));
                int yChange = Convert.ToInt32((((double)dy / (double)lengthV) * (lengthV*3)));
                c.MovePoint(v.position.X - xChange, v.position.Y - yChange);
            }
        }

        private void KeepG1Vertex(Vertex v, Vertex c, int len)
        {
            int dx = this.position.X - v.position.X;
            int dy = this.position.Y - v.position.Y;
            int lengthV = Edge.Distance(v.position, this.position);
            if (lengthV == 0) { return; }
            int xChange = Convert.ToInt32((((double)dx / (double)lengthV) * (len)));
            int yChange = Convert.ToInt32((((double)dy / (double)lengthV) * (len)));
            c.MovePoint(v.position.X - xChange, v.position.Y - yChange);
        }

        private void KeepG1Bezier(Vertex v, BezierControlPoint p)
        {           
            int dx = this.position.X - v.position.X;
            int dy = this.position.Y - v.position.Y;
            int lengthV = Edge.Distance(v.position, this.position);
            if (lengthV == 0) return;
            int xChange = Convert.ToInt32(Math.Floor(((double)dx / (double)lengthV) * (p.length)));
            int yChange = Convert.ToInt32(Math.Floor(((double)dy / (double)lengthV) * (p.length)));
            p.MovePointEnforce(v.position.X - xChange, v.position.Y - yChange);

        }

        public void MovePointEnforce(int x, int y)
        {
            this.position.X = x;
            this.position.Y = y;
        }

        public void MovePointDeltaEnforce(int dx, int dy)
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
