using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.Polygon
{
    public class Polygon
    {
        public List<Vertex> vertices = new List<Vertex>();
        public List<Edge> edges = new List<Edge>();

        public bool isSelected = false;
        public Point? pressPoint = null;

        public void AddNewVertex(Point point)
        {
            vertices.Add(new Vertex(point));
        }

        public void AddNewEdge()
        {
            edges.Add(new Edge(vertices[vertices.Count - 2], vertices[vertices.Count - 1]));
        }

        private void DrawEdges(Bresenham bresenham, bool isBresenham)
        {
            foreach (Edge e in edges)
            {
                DrawConstraint(e, bresenham.g);
                if (e.isSelected)
                {
                    bresenham.brush = new SolidBrush(Color.Red);
                    if (e.state != Enums.EdgeStates.Bezier)
                    {
                        if (isBresenham) bresenham.Draw(e.start.position, e.end.position);
                        else bresenham.g.DrawLine(new Pen(bresenham.brush), e.start.position, e.end.position);
                    }
                    else e.DrawBezier(bresenham.g, bresenham.brush);
                    bresenham.brush = new SolidBrush(Color.Black);
                }
                else
                {
                    if (e.state != Enums.EdgeStates.Bezier) 
                    {
                        if (isBresenham) bresenham.Draw(e.start.position, e.end.position);
                        else bresenham.g.DrawLine(new Pen(bresenham.brush), e.start.position, e.end.position);
                    }
                    else e.DrawBezier(bresenham.g, bresenham.brush);
                }
                
                
                
            }
        }

        private void DrawConstraint(Edge e, Graphics g)
        {            
            Point center = e.FindNewStatePosition();
            Pen pen = new Pen(Color.Green);
            switch (e.state)
            {
                case EdgeStates.Vertical:
                    g.DrawLine(pen, new Point(center.X, center.Y - 5), new Point(center.X, center.Y + 5));
                    break;
                case EdgeStates.Horizontal:
                    g.DrawLine(pen, new Point(center.X - 5, center.Y), new Point(center.X + 5, center.Y));
                    break;
                case EdgeStates.Fixed:
                    g.FillEllipse(Brushes.Green, center.X - 6, center.Y - 6, 12, 12);
                    break;
            }
            
        }

        private void DrawVertices(System.Drawing.Graphics g)
        {
            foreach (Vertex v in vertices)
            {

                if (v.isSelected)
                {
                    Brush brush = Brushes.Red;
                    g.FillEllipse(brush, v.position.X - v.radius, v.position.Y - v.radius, 2 * v.radius, 2 * v.radius);
                }
                else
                {
                    Brush brush = Brushes.Blue;
                    g.FillEllipse(brush, v.position.X - v.radius, v.position.Y - v.radius, 2 * v.radius, 2 * v.radius);
                }
            }
        }

        public void AddFinalEdge()
        {
            edges.Add(new Edge(vertices[vertices.Count - 1], vertices[0]));
            AssignEdgesToVertices();
        }

        private void AssignEdgesToVertices()
        {
            foreach (Edge e in edges)
            {
                e.end.leftEdge = e;
                e.start.rightEdge = e;
            }
        }

        public void DrawPolygon(Bresenham bresenham, Graphics g, bool isBresenham) 
        {

            //if (vertices.Count >= 3)
            //{
            //    Point[] points = new Point[vertices.Count];
            //    int i = 0;
            //    foreach (Vertex v in vertices)
            //    {
            //        points[i] = v.position;
            //        i++;
            //    }
            //    if (isSelected)
            //    {
            //        g.FillPolygon(Brushes.LightCoral, points);
            //    }
            //    else
            //    {
            //        g.FillPolygon(Brushes.LightGray, points);
            //    }
            //}
            DrawEdges(bresenham, isBresenham);
            DrawVertices(g);
            SetAllUnused();
        }

        public void AddNewVertex(Edge selectedEdge)
        {
            selectedEdge.state = Enums.EdgeStates.None;
            selectedEdge.start.rightConstraint = Enums.EdgeStates.None;
            selectedEdge.end.leftConstraint = Enums.EdgeStates.None;
            int edgeNumber = 0;            
            foreach (Edge e in edges)
            {
                if (e == selectedEdge)
                {
                    break;
                }
                edgeNumber++;
            }
            int x = (selectedEdge.start.position.X + selectedEdge.end.position.X) / 2;
            int y = (selectedEdge.start.position.Y + selectedEdge.end.position.Y) / 2;
            Vertex rightV = selectedEdge.end;
            Vertex newVertex = new Vertex(new Point(x, y));
            newVertex.leftEdge = selectedEdge;
            
            vertices.Insert(edgeNumber + 1, newVertex);

            Edge newEdge = new Edge(newVertex, selectedEdge.end);
            newVertex.rightEdge = newEdge;
            edges.Insert(edgeNumber + 1, newEdge);
            rightV.leftEdge = newEdge;
            selectedEdge.end = newVertex;
            
        }

        public void RemoveVertex(Vertex v)
        {
            if (vertices.Count == 3)
            {
                MessageBox.Show("Invalidate operation:\n only 3 vertices left", "Warning",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
                return;
            }
            if (v.leftEdge != null)
            { 
                v.leftEdge.start.rightConstraint = Enums.EdgeStates.None;
                v.leftEdge.state = Enums.EdgeStates.None;
            }
            if (v.rightEdge != null)
            {
                v.rightEdge.end.leftConstraint = Enums.EdgeStates.None;
                v.rightEdge.state = Enums.EdgeStates.None;
            }
            int i = 0;
            foreach (Vertex vert in vertices)
            {
                if (vert == v) break;
                i++;
            }
            int prevEdge;
            if (i == 0)
            {
                prevEdge = edges.Count - 1;
            }
            else
            {
                prevEdge = i - 1;
            }
            Edge e = edges[prevEdge];
            int nextV;
            if (i == vertices.Count - 1) nextV = 0;
            else nextV = i + 1;
            e.end = vertices[nextV];
            e.end.leftEdge = e;
            e.start.rightEdge = e;
            
            edges.RemoveAt(i);
            vertices.RemoveAt(i);
        }

        public bool CheckIsInside(Point point)
        {
            Point[] polygonPoints = new Point[vertices.Count];
            int i = 0;
            foreach (Vertex vertex in vertices)
            {
                polygonPoints[i] = vertex.position;
                i++;
            }
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(polygonPoints);
            if (path.IsVisible(point)) return true;
            return false;
        }

        public void MovePolygon(Point p, int width, int height)
        {
            foreach (Vertex v in vertices)
            {
                v.MovePointEnforce(p, pressPoint.Value);
            }
        }

        public void SetAllUnused()
        {
            foreach (Vertex v in vertices)
            {
                v.used = false;
            } 
        }

        public void ResetAllLengthes()
        {
            foreach (Edge e in edges)
            {
                if (e.state == EdgeStates.Fixed) continue;
                if (e.state == EdgeStates.Bezier)
                {
                    e.bezierControlPoints[0].length = Edge.Distance(e.start.position, e.bezierControlPoints[0].position);
                    e.bezierControlPoints[1].length = Edge.Distance(e.end.position, e.bezierControlPoints[1].position);
                    continue;
                }
                e.length = Edge.Distance(e.start.position, e.end.position);
                
            }
        }

        public void ResetBezier()
        {
            foreach (Vertex v in vertices)
            {
                if ((v.leftConstraint != EdgeStates.Bezier) && (v.leftEdge.start.leftConstraint == EdgeStates.Bezier))
                {
                    BezierControlPoint associatedPoint = v.leftEdge.start.leftEdge.bezierControlPoints[1];
                    Vertex midleVertex = v.leftEdge.start;
                    BezierStates state = v.leftEdge.start.bezierState;
                    if (state != BezierStates.G0) KeepContinuous(v, associatedPoint, midleVertex, state, v.leftEdge);
                }
                if ((v.rightConstraint != EdgeStates.Bezier) && (v.rightEdge.end.rightConstraint == EdgeStates.Bezier))
                {
                    BezierControlPoint associatedPoint = v.rightEdge.end.rightEdge.bezierControlPoints[0];
                    Vertex midleVertex = v.rightEdge.end;
                    BezierStates state = v.rightEdge.end.bezierState;
                    if (state != BezierStates.G0) KeepContinuous(v, associatedPoint, midleVertex, state, v.rightEdge);
                }
            }
        }

        private void KeepContinuous(Vertex v, BezierControlPoint p, Vertex m, BezierStates state, Edge e)
        {
            if (p.isSelected == true) return;
            if (state == BezierStates.G1)
            {
                KeepG1(v, p, m);
            }
            else
            {
                KeepC1(v, p, m, e);
            }
        }

        private void KeepG1(Vertex v, BezierControlPoint p, Vertex m)
        {
            int dx = m.position.X - v.position.X;
            int dy = m.position.Y - v.position.Y;
            int len = Edge.Distance(m.position, v.position);
            if (len == 0) { p.MovePointEnforce(m.position.X, m.position.Y); return; }
            int relatedX = Convert.ToInt32((((double)dx / (double)len) * (p.length)));
            int relatedY = Convert.ToInt32((((double)dy / (double)len) * (p.length)));
            p.MovePointEnforce(m.position.X + relatedX, m.position.Y + relatedY);
        }

        private void KeepC1(Vertex v, BezierControlPoint p, Vertex m, Edge e)
        {
            if (e.state == EdgeStates.Fixed)
            {
                int dx = m.position.X - v.position.X;
                int dy = m.position.Y - v.position.Y;
                int len = Edge.Distance(m.position, v.position);
                int relatedX = Convert.ToInt32((((double)dx / (double)len) * ((double)e.length / 3.0)));
                int relatedY = Convert.ToInt32((((double)dy / (double)len) * ((double)e.length / 3.0)));
                p.MovePointEnforce(m.position.X + relatedX, m.position.Y + relatedY);
            }
            else
            {
                int dx = m.position.X - v.position.X;
                int dy = m.position.Y - v.position.Y;
                int len = Edge.Distance(m.position, v.position);
                if (len == 0) { p.MovePointEnforce(m.position.X, m.position.Y); return; }
                int relatedX = Convert.ToInt32((((double)dx / (double)len) * ((double)len/3.0)));
                int relatedY = Convert.ToInt32((((double)dy / (double)len) * ((double)len/3.0)));
                p.MovePointEnforce(m.position.X + relatedX, m.position.Y + relatedY);
            }
        }
    }
}
