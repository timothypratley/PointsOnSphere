using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Triangulation;


namespace Demo {
    class Program {
        static void Main(string[] args) {
            /** this algorithm works for any number of nodes, but is not as regular
            int N = 100;
            double dlong = Math.PI * (3 - Math.Sqrt(5));
            double dz = 2.0 / N;
            double longitude = 0;
            double z = 1 - dz / 2;
            var nodes = new List<Vector3D>();
            for (var k = 0; k < N; k++) {
                var r = Math.Sqrt(1 - z * z);
                nodes.Add(new Vector3D(Math.Cos(longitude) * r, Math.Sin(longitude) * r, z));
                z = z - dz;
                longitude = longitude + dlong;
            }
            foreach (var node in nodes) {
                Console.WriteLine(node);
            }
            Console.ReadKey();
            */

            var sector = new Icosahedron(4);
            for (int i = 0; i < sector.Vertices.Count; i++) {
                Console.WriteLine(
                    "[" + i + "] \t" + VertStr(sector.Vertices[i])
                    + "\tIP(" + sector.GetIP(i)
                    + ")\tex" + SetStr(sector.Edges(i)));
            }
            Console.ReadKey();
        }

        static string VertStr(Vector3D v) {
            return string.Format("{0:F},{1:F},{2:F}", v.X, v.Y, v.Z);
        }

        static string SetStr(IEnumerable<int> s) {
            return "{" + s.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b) + "}";
        }
    }
}
