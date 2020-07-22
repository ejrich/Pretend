namespace Pretend
{
    public class Entrypoint
    {
        public static void Start<TApp, TWA>()
            where TApp : IApplication
            where TWA : IWindowAttributesProvider
        {
            var factory = new Factory();
            factory.RegisterServices<TApp, TWA>();
            factory.BuildContainer();

            var applicationRunner = factory.Create<IApplicationRunner>();

            applicationRunner.Run();
        }
    }
}
