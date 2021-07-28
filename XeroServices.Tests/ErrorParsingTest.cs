using Azihub.Utilities.Base.Tests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeroServices.ErrorResponse;
using XeroServices.Tests.DataSamples;
using Xunit;
using Xunit.Abstractions;

namespace XeroServices.Tests
{
    public class ErrorParsingTest : TestBase
    {
        public ErrorParsingTest(ITestOutputHelper outputHelper) : base(outputHelper) { }
        [Fact]
        public void TestErrorParse()
        {
            ErrorResponseInvoice response = GetSampleData.GetSampleErrors();
            Assert.Equal(209, response.Elements.Count);
        }

    }
}
