using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR1
{
    public struct Vertex
    {
        public Vertex(Vector3 position, Vector3 color)
        {
            this.position = position;
            this.color = color;
        }

        public Vector3 position;
        public Vector3 color;
    }
}
