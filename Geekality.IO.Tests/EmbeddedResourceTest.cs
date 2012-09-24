using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Geekality.IO
{
    [TestFixture]
    class EmbeddedResourceTest
    {
        public static readonly string CONTENTS = "Foobar æøå";

        [Test]
        public void Get_FromCallingAssembly()
        {
            using (var s = new StreamReader(EmbeddedResource.Get("Geekality.IO.EmbeddedResourceTest.txt")))
                Assert.AreEqual(CONTENTS, s.ReadToEnd());
        }
        [Test]
        public void Get_FromTypeAssembly()
        {
            using (var s = new StreamReader(EmbeddedResource.Get<EmbeddedResourceTest>("Geekality.IO.EmbeddedResourceTest.txt", false)))
                Assert.AreEqual(CONTENTS, s.ReadToEnd());
        }
        [Test]
        public void Get_FromTypeAssemblyUsingNamespace()
        {
            using (var s = new StreamReader(EmbeddedResource.Get<EmbeddedResourceTest>("EmbeddedResourceTest.txt", true)))
                Assert.AreEqual(CONTENTS, s.ReadToEnd());
        }
    }
}
