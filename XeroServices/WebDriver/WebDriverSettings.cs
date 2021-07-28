namespace XeroServices.WebDriver
{
    public class WebDriverSettings
    {
        public Browser Browser { get; set; } = Browser.Firefox;
        public bool ShowImages { get; set; } = true;
        public bool HeadlessBrowser { get; set; } = false;
        public int CrawlerLifetimeMinutes { get; set; } = 1200;

        public int FindElementTimeoutMilliseconds { get; set; } = 0;
        public int PageLoadTimeoutMilliseconds { get; set; } = 0;
    }
}
