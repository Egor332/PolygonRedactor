using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.ForSerialization
{
    public class VertexForSerialization
    {
        public Point position { get; set; }
        public BezierStates bezierState { get; set; }
    }
}
