using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// 前段の情報をN回満たすか
    /// </summary>
    public class CountResolver : IResolver
    {
        private IResolver _value;
        private int _count;

        public CountResolver(int count, IResolver value)
        {
            _count = count;
            _value = value;
        }

        public CountResolver Register(IResolver value)
        {
            _value = value;
            return this;
        }


        public bool Resolve(params string[] origin)
        {
            return origin.Count(x => _value.Resolve(x)) >= _count;
        }

        public override string ToString()
        {
            return string.Format("({0}^{1})", _value, _count);
        }
    }
}