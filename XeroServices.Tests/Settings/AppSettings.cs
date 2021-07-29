using Azihub.Utilities.Base.Tools;
using XeroServices.Login;

namespace XeroServices.Tests.Settings
{
    public static class AppSettings
    {
        static AppSettings()
        {
            DotEnv.Load();
            Local = DotEnv.Load<LocalSettings>();
        }

        public static LocalSettings Local { get; }

        public static string AccessToken => Local.TestAccessToken;
        public static string TenantId => ApiClient.GetTenantId(AccessToken);
        /// <summary>
        /// Xero account to run tests
        /// </summary>
        public static XeroLogin XeroLogin => new(Local.XeroUser, Local.XeroPass);
    }
}
