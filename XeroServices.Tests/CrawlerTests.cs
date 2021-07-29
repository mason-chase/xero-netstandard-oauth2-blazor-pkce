
using Divergic.Logging.Xunit;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.ErrorResponse;
using XeroServices.Login;
using XeroServices.Tests.DataSamples;
using XeroServices.Tests.Settings;
using Xunit;
using Xunit.Abstractions;
using static XeroServices.Tests.Settings.AppSettings;

namespace XeroServices.Tests
{
    public class CrawlerTests : ApiTests
    {
        public CrawlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AppLogger.LoggerFactory = LogFactory.Create(outputHelper);
        }

        [Fact]
        public void DeleteXeroInvoices()
        {
            // Setup
            ErrorResponseInvoice response = GetSampleData.GetSampleErrors();
            Invoices invoices = new() { _Invoices = response.Elements };
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());

            // Act
            accountingApi.VoidInvoices(invoices);

            // Assert)
        }

        [Fact]
        public void DeleteOrVoidXeroInvoicesByWebDriver()
        {
            // Setup
            Organisation organisation = GetSampleData.GetSampleOrganisation();
            Invoices invoices = new() { _Invoices = GetSampleData.GetSampleErrors().Elements };
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());
            XeroLogin xeroLogin = AppSettings.XeroLogin;
            
            // Act
            accountingApi.DeleteOrVoidInvoicesByWebDriver(xeroLogin, organisation, invoices);

            // Assert
        }
    }
}
