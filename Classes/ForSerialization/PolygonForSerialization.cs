using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonRedactor.Classes.ForSerialization
{
    public class PolygonForSerialization
    {
        public List<VertexForSerialization> vertices { get ; set; }
        public List<EdgeForSerialization> edges { get; set;}
    }
}
