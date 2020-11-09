namespace Pretend
{
    public class Entrypoint
    {
        public static void Start<TApp>(string title)
            where TApp : IApplication
        {
            var factory = new Factory();
            factory.RegisterServices<TApp>();
            factory.BuildContainer();

            var applicationRunner = factory.Create<IApplicationRunner>();

            applicationRunner.Run(title);
        }
    }
}
