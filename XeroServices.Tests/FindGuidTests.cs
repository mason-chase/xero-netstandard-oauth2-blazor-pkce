using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeroServices.Utilities;
using Xunit;

namespace XeroServices.Tests
{
    public class FindGuidTests
    {
        [Fact]
        public void FindGuidSuccessTest()
        {
            // Setup
            string testUrl = "https://go.xero.com/Bank/ViewTransaction.aspx?bankTransactionID=3f9491a1-74e2-4386-909b-0cdc24f14bd0";
            Guid expected = Guid.Parse("3f9491a1-74e2-4386-909b-0cdc24f14bd0");

            // Act
            var testGuid = testUrl.FindGuid();

            Assert.Equal(expected, testGuid);
        }

        [Fact]
        public void FindGuidFailTest()
        {
            // Setup
            string testUrl = "https://go.xero.com/Bank/ViewTransaction.aspx?bankTransactionID=3f9491a1-74e2-4386-909b-0cdc24f14b";

            // Act
            Guid? testGuid = testUrl.FindGuid();

            Assert.Null(testGuid);
        }
    }
}
