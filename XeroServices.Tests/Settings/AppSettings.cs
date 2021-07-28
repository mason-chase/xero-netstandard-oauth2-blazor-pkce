using Azihub.Utilities.Base.Tools;

namespace XeroServices.Tests.Settings
{
    public static class AppSettings
    {
        static AppSettings()
        {
            DotEnv.Load();
            Global = DotEnv.Load<GlobalSettings>();
        }

        public static GlobalSettings Global { get; }
    }
}
