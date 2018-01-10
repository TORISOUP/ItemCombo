using System.Collections.Generic;
using System.Linq;
using Assets.ItemCombo.Resolver;
using NUnit.Framework;

namespace Assets.ItemCombo.Editor
{
    public class ResolverParserTest
    {


        [Test]
        public void パースできる1()
        {
            var resolver = ResolverParser.Parse("ebi ika tako +2");
            var expected = new UniqueSelectResolver(2, "ebi", "ika", "tako");
            PatternTest(resolver, expected, "ebi", "ika", "tako", "unko");
        }

        [Test]
        public void パースできる2()
        {
            var resolver = ResolverParser.Parse("ebi ika tako +2 geta &");

            var expected = new AndResolver(
                new SingleResolver("geta"),
                new UniqueSelectResolver(2, "ebi", "ika", "tako"));

            PatternTest(resolver, expected, "ebi", "ika", "tako", "unko", "geta");
        }

        [Test]
        public void パースできる3()
        {
            var resolver = ResolverParser.Parse("ebi ika tako +2 geta & doku ! &");

            var expected = new AndResolver(
                new AndResolver(
                    new SingleResolver("geta"),
                    new UniqueSelectResolver(2, "ebi", "ika", "tako")
                ),
                new NotResolver(new SingleResolver("doku")));

            PatternTest(resolver, expected, "ebi", "ika", "tako", "unko", "geta", "doku");
        }

        [Test]
        public void 逆ポーランド記法からResolverを生成できる()
        {
            // ebi ika tako からどれか2個以上を含む　かつ getaを含む かつ dokuは含まない かつ nikuかsakeのどちらかを含む
            var resolver = ResolverParser.Parse("ebi ika tako +2 geta & doku ! & niku sake | &");

            var expected =
                new AndResolver(
                    new OrResolver(new SingleResolver("sake"), new SingleResolver("niku")),
                    new AndResolver(
                        new AndResolver(
                            new SingleResolver("geta"),
                            new UniqueSelectResolver(2, "ebi", "ika", "tako")
                        ),
                        new NotResolver(new SingleResolver("doku"))
                    )
                );

            //全組み合わせテスト
            PatternTest(resolver, expected, "ebi", "ika", "tako", "unko", "geta", "doku", "niku", "sake");
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
