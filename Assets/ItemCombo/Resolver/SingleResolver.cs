using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// 単一の判定を行う
    /// </summary>
    public class SingleResolver : IResolver
    {
        private string _value;

        public string Value { get { return _value; } }

        public SingleResolver(string value)
        {
            _value = value;
        }

        public bool Resolve(params string[] origin)
        {
            return origin.Contains(_value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}