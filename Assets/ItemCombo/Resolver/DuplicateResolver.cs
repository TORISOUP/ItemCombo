using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// 特定文字列のうち、指定文字列をN個以上含んでいるか
    /// </summary>
    public class DuplicateResolver : IResolver
    {
        private SingleResolver _value;
        private int _count;

        public DuplicateResolver(int count, SingleResolver value)
        {
            _count = count;
            _value = value;
        }

        public DuplicateResolver Register(SingleResolver value)
        {
            _value = value;
            return this;
        }


        public bool Resolve(params string[] origin)
        {
            return origin.Count(x => _value.Resolve(x)) >= _count;
        }
    }
}