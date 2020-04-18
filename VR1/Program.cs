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
            var application = new App(800, 600, "Масоны");
            application.Run(60);
        }
    }
}
