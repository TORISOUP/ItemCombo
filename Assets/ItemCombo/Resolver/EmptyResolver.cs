namespace Assets.ItemCombo.Resolver
{
    public struct EmptyResolver : IResolver
    {
        public bool Resolve(params string[] origin)
        {
            return true;
        }

        public static EmptyResolver Default = new EmptyResolver();
    }
}