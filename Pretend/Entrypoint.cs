namespace Pretend
{
    public class Entrypoint
    {
        public static void Start<TApp>(string title) where TApp : IApplication => Start<TApp, Settings>(title);

        public static void Start<TApp, TSettings>(string title)
            where TApp : IApplication
            where TSettings : Settings, new()
        {
            var factory = new Factory();
            factory.RegisterServices<TApp, TSettings>();
            factory.BuildContainer();

            var applicationRunner = factory.Create<IApplicationRunner>();

            applicationRunner.Run(title);
        }
    }
}
