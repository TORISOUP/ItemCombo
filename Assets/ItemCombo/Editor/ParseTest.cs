using System.Collections.Generic;
using System.Linq;
using Assets.ItemCombo.Resolver;
using NUnit.Framework;

namespace Assets.ItemCombo.Editor
{
    public class ParseTest
    {

        [Test]
        public void 数式で表現できる()
        {
            var parser = new Parser("!hoge^3");
            UnityEngine.Debug.Log(parser.Expr().ToString());
        }


    }
}
