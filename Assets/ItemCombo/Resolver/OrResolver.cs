using System.Collections.Generic;
using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// ORを計算する
    /// </summary>
    public class OrResolver : IResolver
    {
        private IResolver A;
        private IResolver B;

        public OrResolver(IResolver a, IResolver b)
        {
            A = a;
            B = b;
        }

        public bool Resolve(params string[] origin)
        {
            return A.Resolve(origin) || B.Resolve(origin);
        }

        public override string ToString()
        {
            return string.Format("({0}+{1})", A.ToString(), B.ToString());
        }
    }
}