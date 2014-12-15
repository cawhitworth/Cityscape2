using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityScape2;
using NUnit.Framework;
using SharpDX;

namespace Tests
{
    public class PanelTests
    {
        private static readonly Vector2 Origin = new Vector2(0.0f);
        private static readonly Color Mod = Color.White;

        [Test]
        public void XY_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XY, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 0, -1);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,2,0)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(1,0,0)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(1,2,0)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }

        [Test]
        public void XY_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XY, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 0, 1);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,2,0)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(1,0,0)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(1,2,0)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }

        [Test]
        public void XZ_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XZ, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, 1, 0);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,0,2)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(1,0,0)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(1,0,2)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }

        [Test]
        public void XZ_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.XZ, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(0, -1, 0);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,0,2)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(1,0,0)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(1,0,2)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }

        [Test]
        public void YZ_Out_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.YZ, Panel.Facing.Out, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(1, 0, 0);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,2,0)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(0,0,1)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(0,2,1)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }

        [Test]
        public void YZ_In_Panel()
        {
            var p = new Panel(new Vector3(0, 0, 0), new Vector2(1, 2), Panel.Plane.YZ, Panel.Facing.In, Origin, Origin, Mod);

            var v = p.Vertices.ToArray();

            var norm = new Vector3(-1, 0, 0);

            Assert.That(v[0].Position, Is.EqualTo(new Vector3(0,0,0)));
            Assert.That(v[1].Position, Is.EqualTo(new Vector3(0,2,0)));
            Assert.That(v[2].Position, Is.EqualTo(new Vector3(0,0,1)));
            Assert.That(v[3].Position, Is.EqualTo(new Vector3(0,2,1)));

            Assert.That(v[0].Normal, Is.EqualTo(norm));
            Assert.That(v[1].Normal, Is.EqualTo(norm));
            Assert.That(v[2].Normal, Is.EqualTo(norm));
            Assert.That(v[3].Normal, Is.EqualTo(norm));
        }
    }
}
