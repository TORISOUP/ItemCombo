using System.Linq;

namespace Assets.ItemCombo.Resolver
{
    /// <summary>
    /// Notを計算する
    /// </summary>
    public class NotResolver : IResolver
    {
        private IResolver _resolver = EmptyResolver.Default;

        public NotResolver()
        {
        }

        public NotResolver(IResolver resolver)
        {
            _resolver = resolver;
        }

        public NotResolver Register(IResolver resolver)
        {
            _resolver = resolver;
            return this;
        }

        public bool Resolve(params string[] origin)
        {
            return !_resolver.Resolve(origin);
        }
    }
}