using System.Collections.Generic;
using System.Linq;
using Assets.ItemCombo.Resolver;
using NUnit.Framework;

namespace Assets.ItemCombo.Editor
{
    public class ResolverParserTest
    {

        [Test]
        public void 数式で表現できる()
        {
            var str = "A + B * C";

            var array = str.Split(' ');
            var actual = ResolverParser.From(array, 0, array.Length);

            var expected =
                new AndResolver(
                    new OrResolver(
                        new SingleResolver("A"),
                        new SingleResolver("B")
                    ),
                    new SingleResolver("C"));

            UnityEngine.Debug.Log(actual.ToString());
            UnityEngine.Debug.Log(expected.ToString());

            PatternTest(actual, expected, "A", "B", "C", "D");
        }

        [Test]
        public void かっこを含んだ数式で表現できる()
        {
            var str = "A + ( B * C ) + D";

            var array = str.Split(' ');
            var actual = ResolverParser.From(array, 0, array.Length);

            var expected =
                new OrResolver(
                    new OrResolver(
                        new SingleResolver("A"),
                        new AndResolver(new SingleResolver("B"), new SingleResolver("C"))),
                    new SingleResolver("D"));


            UnityEngine.Debug.Log("input   : " + str);
            UnityEngine.Debug.Log("expected: " + expected.ToString());
            UnityEngine.Debug.Log("result  : " + actual.ToString());

            PatternTest(actual, expected, "A", "B", "C", "D");
        }

        private void PatternTest(IResolver actual, IResolver expected, params string[] values)
        {
            foreach (var c in Enumerable.Range(0, values.Length + 1).SelectMany(n => Comb(values, n)))
            {
                var a = c.ToArray();
                Assert.AreEqual(expected.Resolve(a), actual.Resolve(a));
            }
        }


        private IEnumerable<IEnumerable<T>> Comb<T>(IEnumerable<T> items, int r)
        {
            if (r == 0)
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                var i = 1;
                foreach (var x in items)
                {
                    var xs = items.Skip(i);
                    foreach (var c in Comb(xs, r - 1))
                        yield return Before(c, x);

                    i++;
                }
            }

        }

        private static IEnumerable<T> Before<T>(IEnumerable<T> items, T first)
        {
            yield return first;
            foreach (var i in items)
                yield return i;
        }
    }
}
