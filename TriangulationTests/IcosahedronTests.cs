using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triangulation;

namespace TriangulationTests {
    [TestClass]
    public class IcosahedronTests {
        [TestMethod]
        public void CreateIcosahedronTest() {
            var sphere = new Icosahedron(2);
        }

        [TestMethod]
        public void LargeIcosahedronTest() {
            for (int i = 0; i <= 5; i++)
                new Icosahedron(i);
        }
    }
}
