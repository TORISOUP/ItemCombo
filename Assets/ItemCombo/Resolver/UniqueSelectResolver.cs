using System.Collections.Generic;
using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// 特定文字列のうち、ユニークな指定文字列をN個以上含んでいるか
    /// </summary>
    public class UniqueSelectResolver : IResolver
    {
        private IEnumerable<SingleResolver> _values;
        private int _count;

        public UniqueSelectResolver(int count, params string[] values)
        {
            _count = count;
            _values = values.Select(x => new SingleResolver(x));
        }

        public UniqueSelectResolver(int count, params SingleResolver[] values)
        {
            _count = count;
            _values = values;
        }

        public UniqueSelectResolver Register(params SingleResolver[] values)
        {
            _values = _values.Concat(values);
            return this;
        }

        public bool Resolve(params string[] origin)
        {
            return origin.Intersect(_values.Select(x => x.Value)).Count() >= _count;
        }
    }
}