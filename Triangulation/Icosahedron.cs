using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Diagnostics.Contracts;

namespace Triangulation {
    public class Icosahedron {
        public List<Vector3D> Vertices { get; private set; }
        Dictionary<Vector3D, int> verticesIndex;
        Dictionary<string, int> ipIndex;
        Dictionary<int, string> ips;
        List<Tuple<int, int, int>> triangles;
        Dictionary<int, HashSet<int>> edges;
        public IEnumerable<int> Edges(int from) {
            HashSet<int> toSet;
            if (edges.TryGetValue(from, out toSet))
                return toSet.AsEnumerable();
            else
                return Enumerable.Empty<int>();
        }

        public Icosahedron(int subdivisions) {
            Contract.Requires<ArgumentException>(subdivisions >= 0 && subdivisions <= 5);
            // IPs are not unique because of rounding above 5

            Vertices = new List<Vector3D>();
            verticesIndex = new Dictionary<Vector3D, int>();
            ipIndex = new Dictionary<string, int>();
            ips = new Dictionary<int, string>();
            triangles = new List<Tuple<int, int, int>>();

            // create 12 vertices of a Icosahedron
            var t = (1 + Math.Sqrt(5)) / 2;

            v(-1, t, 0);
            v(1, t, 0);
            v(-1, -t, 0);
            v(1, -t, 0);

            v(0, -1, t);
            v(0, 1, t);
            v(0, -1, -t);
            v(0, 1, -t);

            v(t, 0, -1);
            v(t, 0, 1);
            v(-t, 0, -1);
            v(-t, 0, 1);

            // 5 faces around point 0
            f3(0, 11, 5, triangles);
            f3(0, 5, 1, triangles);
            f3(0, 1, 7, triangles);
            f3(0, 7, 10, triangles);
            f3(0, 10, 11, triangles);

            // 5 adjacent faces
            f3(1, 5, 9, triangles);
            f3(5, 11, 4, triangles);
            f3(11, 10, 2, triangles);
            f3(10, 7, 6, triangles);
            f3(7, 1, 8, triangles);

            // 5 faces around point 3
            f3(3, 9, 4, triangles);
            f3(3, 4, 2, triangles);
            f3(3, 2, 6, triangles);
            f3(3, 6, 8, triangles);
            f3(3, 8, 9, triangles);

            // 5 adjacent faces
            f3(4, 9, 5, triangles);
            f3(2, 4, 11, triangles);
            f3(6, 2, 10, triangles);
            f3(8, 6, 7, triangles);
            f3(9, 8, 1, triangles);

            // subdivide faces to refine the triangles
            for (var i = 0; i < subdivisions; i++) {
                var next = new List<Tuple<int, int, int>>();
                foreach (var tri in triangles) {
                    // replace each triangle by 4 triangles
                    var a = getMiddlePoint(tri.Item1, tri.Item2);
                    var b = getMiddlePoint(tri.Item2, tri.Item3);
                    var c = getMiddlePoint(tri.Item3, tri.Item1);

                    f3(tri.Item1, a, c, next);
                    f3(tri.Item2, b, a, next);
                    f3(tri.Item3, c, b, next);
                    f3(a, b, c, next);
                }
                triangles = next;
            }

            CreateGraph();
        }

        int v(double x, double y, double z) {
            var vec = new Vector3D(x, y, z);
            vec.Normalize();
            return AddVertex(vec);
        }

        int AddVertex(Vector3D vec) {
            int id = Vertices.Count;
            verticesIndex.Add(vec, id);
            Vertices.Add(vec);

            string ip = IP(vec);
            ips.Add(id, ip);
            ipIndex.Add(ip, id);

            return id;
        }

        void f3(int a, int b, int c, List<Tuple<int, int, int>> triangles) {
            triangles.Add(new Tuple<int, int, int>(a, b, c));
        }

        int getMiddlePoint(int p1, int p2) {
            var mid = (Vertices[p1] + Vertices[p2]) / 2;
            mid.Normalize();
            int v;
            if (verticesIndex.TryGetValue(mid, out v))
                return v;
            return AddVertex(mid);
        }


        void CreateGraph() {
            edges = new Dictionary<int, HashSet<int>>();
            foreach (var tri in triangles) {
                Bissoc(tri.Item1, tri.Item2);
                Assoc(tri.Item2, tri.Item3);
                Assoc(tri.Item3, tri.Item1);
            }
        }

        void Bissoc(int a, int b) {
            Assoc(a, b);
            Assoc(b, a);
        }

        void Assoc(int from, int to) {
            HashSet<int> toSet;
            if (!edges.TryGetValue(from, out toSet)) {
                toSet = new HashSet<int>();
                edges.Add(from, toSet);
            }
            toSet.Add(to);
        }


        void GenerateIPs() {
            for (int i = 0; i < Vertices.Count; i++) {

            }
        }

        static string IP(Vector3D v) {
            var r = v.Length;
            var lat = 90 - Math.Acos(v.Z / r) * 180 / Math.PI;
            var lon = Math.Atan2(v.Y, v.X) * 180 / Math.PI;
            return Math.Round(lat) + "," + Math.Round(lon);
        }

        public int? GetVertexFromIP(string IP) {
            int vertex;
            if (ipIndex.TryGetValue(IP, out vertex))
                return vertex;
            return null;
        }

        public string GetIP(int vertex) {
            string ip;
            if (ips.TryGetValue(vertex, out ip))
                return ip;
            return null;
        }

        [Pure]
        public bool UniqueVerticies() {
            return Vertices.Count == verticesIndex.Count;
        }

        [Pure]
        public bool UniqueIPs() {
            return Vertices.Count == ips.Count
                && Vertices.Count == ipIndex.Count;
        }

        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(UniqueVerticies());
            Contract.Invariant(UniqueIPs());
        }
    }
}
