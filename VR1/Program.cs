using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR1
{
    class Program
    {
        static void Main(string[] args)
        {
            var application = new Win(800, 800, "Test OpenGL");
            application.Run(60);
        }
    }
}
