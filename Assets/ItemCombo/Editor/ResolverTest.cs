using System;
using System.Collections;
using System.Linq;
using Assets.ItemCombo.Resolver;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Assets.ItemCombo.Editor
{
    public class ResolverTest
    {
        #region SingleResolver
        [Test]
        public void SingleResolver()
        {
            var strs = new[] { "abc", "def", "ghi" };

            var s1 = new SingleResolver("abc");
            var s2 = new SingleResolver("xxx");
            Assert.True(s1.Resolve(strs));
            Assert.False(s2.Resolve(strs));
        }
        #endregion

        #region AndResolver
        [Test]
        public void AndResolver()
        {
            var strs = new[] { "abc", "def", "ghi" };

            var s1 = new SingleResolver("abc");
            var s2 = new SingleResolver("def");
            var s4 = new SingleResolver("xxx");

            var andSuccess = new AndResolver(s1, s2);
            var andFailuer = new AndResolver(s1, s4);

            Assert.True(andSuccess.Resolve(strs));
            Assert.False(andFailuer.Resolve(strs));
        }
        #endregion

        #region OrResolver
        [Test]
        public void OrResolver()
        {
            var strs = new[] { "abc", "def", "ghi" };

            var s1 = new SingleResolver("abc");
            var s2 = new SingleResolver("def");
            var s3 = new SingleResolver("yyy");
            var s4 = new SingleResolver("xxx");

            var orS1 = new OrResolver(s1, s2);
            var orF1 = new OrResolver(s3, s4);

            Assert.True(orS1.Resolve(strs));
            Assert.False(orF1.Resolve(strs));
        }
        #endregion

        #region NotResolver
        [Test]
        public void NotResolver()
        {
            var strs = new[] { "abc", "def", "ghi" };

            var s1 = new SingleResolver("abc");
            var s2 = new SingleResolver("xxx");

            var n1 = new NotResolver(s1);
            var n2 = new NotResolver(s2);

            Assert.True(n2.Resolve(strs));
            Assert.False(n1.Resolve(strs));
        }
        #endregion


        #region CountResolver

        [Test]
        public void CountResolver()
        {
            // A を 2個以上
            var u = new CountResolver(2, new SingleResolver("A"));

            Assert.False(u.Resolve("A"));
            Assert.False(u.Resolve("A", "B"));
            Assert.False(u.Resolve("A", "C"));
            Assert.False(u.Resolve("B", "C"));

            Assert.True(u.Resolve("A", "A"));
            Assert.True(u.Resolve("A", "A", "A"));
            Assert.True(u.Resolve("A", "A", "B"));
        }
        #endregion

        #region Combination

        [Test]
        public void Combination()
        {
            var A = new SingleResolver("A");
            var B = new SingleResolver("B");
            var C = new SingleResolver("C");
            var D = new SingleResolver("D");

            // (A and B) or C 
            {
                var r1 = new OrResolver(new AndResolver(A, B), C);
                Assert.True(r1.Resolve("C"));
                Assert.True(r1.Resolve("A", "B"));
                Assert.True(r1.Resolve("A", "C"));
                Assert.True(r1.Resolve("B", "C"));
                Assert.True(r1.Resolve("A", "B", "C"));
                Assert.False(r1.Resolve("A"));
                Assert.False(r1.Resolve("B"));
                Assert.False(r1.Resolve());
            }

            // A and ( B or C )
            {
                var r2 = new AndResolver(A, new OrResolver(B, C));
                Assert.True(r2.Resolve(new[] { "A", "B" }));
                Assert.True(r2.Resolve(new[] { "A", "C" }));
                Assert.True(r2.Resolve(new[] { "A", "B", "C" }));
                Assert.False(r2.Resolve(new[] { "B", "C" }));
                Assert.False(r2.Resolve(new[] { "C" }));
                Assert.False(r2.Resolve(new[] { "A" }));
                Assert.False(r2.Resolve(new[] { "B" }));
                Assert.False(r2.Resolve(new string[0]));
            }

            // A and !(B and C)
            {
                var r3 = new AndResolver(A, new NotResolver(new AndResolver(B, C)));
                Assert.True(r3.Resolve(new[] { "A" }));
                Assert.True(r3.Resolve(new[] { "A", "B" }));
                Assert.True(r3.Resolve(new[] { "A", "C" }));
                Assert.False(r3.Resolve(new[] { "A", "B", "C" }));
                Assert.False(r3.Resolve(new[] { "B", "C" }));
                Assert.False(r3.Resolve(new[] { "C" }));
                Assert.False(r3.Resolve(new[] { "B" }));
                Assert.False(r3.Resolve(new string[0]));
            }
        }

        #endregion
    }
}
