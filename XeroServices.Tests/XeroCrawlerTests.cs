using Azihub.Utilities.Base.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xunit;
using Xunit.Abstractions;

namespace XeroServices.Tests
{
    public class XeroCrawlerTests : TestBase
    {
        public XeroCrawlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void DeleteXeroInvoices()
        {
            Invoices invoices = Accounting
        }
    }
}
