using PolygonRedactor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.ForSerialization
{
    public class EdgeForSerialization
    {
        public EdgeStates state { get; set; }
        public int? length { get; set; }
        public BezierForSerialization?[] bezierControlPoints { get; set; }
    }
}
