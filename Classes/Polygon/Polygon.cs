using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.Polygon
{
    internal class Polygon
    {
        public List<Vertex> vertices = new List<Vertex>();
        public List<Edge> edges = new List<Edge>();

        public void AddNewVertex(Point point)
        {
            vertices.Add(new Vertex(point));
        }

        public void AddNewEdge()
        {
            edges.Add(new Edge(vertices[vertices.Count - 1], vertices[vertices.Count - 2]));
        }

        private void DrawEdges(Bresenham bresenham)
        {
            foreach (Edge e in edges)
            {
                if (e.isSelected)
                {
                    bresenham.brush = new SolidBrush(Color.Red);
                    bresenham.Draw(e.start.position, e.end.position);
                    bresenham.brush = new SolidBrush(Color.Black);
                }
                else
                {
                    bresenham.Draw(e.start.position, e.end.position);
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
        }

        public void DrawPolygon(Bresenham bresenham, Graphics g) 
        {

            if (vertices.Count >= 3)
            {
                Point[] points = new Point[vertices.Count];
                int i = 0;
                foreach (Vertex v in vertices)
                {
                    points[i] = v.position;
                    i++;
                }
                g.FillPolygon(Brushes.LightGray, points);
            }
            DrawEdges(bresenham);
            DrawVertices(g);
        }
    }
}
