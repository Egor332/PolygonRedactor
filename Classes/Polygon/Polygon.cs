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

            Vertex newVertex = new Vertex(new Point(x, y));

            vertices.Insert(edgeNumber + 1, newVertex);

            Edge newEdge = new Edge(newVertex, selectedEdge.end);
            
            edges.Insert(edgeNumber + 1, newEdge);
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

            }
        }
    }
}
