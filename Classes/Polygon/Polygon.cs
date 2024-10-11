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

        public void Draw(Bresenham bresenham)
        {
            foreach (Edge e in edges)
            {
                bresenham.Draw(e.start.position, e.end.position);
            }
        }

        public void AddFinalEdge()
        {
            edges.Add(new Edge(vertices[vertices.Count - 1], vertices[0]));
        }
    }
}
