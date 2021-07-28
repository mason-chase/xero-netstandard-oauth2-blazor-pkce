using Azihub.Utilities.Base.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.Tests.Settings;
using Xunit;
using Xunit.Abstractions;

namespace XeroServices.Tests
{
    public class AccountingTests : TestBase
    {
        private static string _accessToken = AppSettings.Global.TestAccessToken;
        private readonly string _tenantId;

        public AccountingTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _accessToken = AppSettings.Global.TestAccessToken;
            _tenantId = ApiClient.GetTenantId(_accessToken);
        }

        [Fact]
        public void GetConnectionTest()
        {

            string tenantId = ApiClient.GetTenantId(_accessToken);
            Assert.True(string.IsNullOrEmpty(tenantId));
        }

        [Fact]
        public void GetInvoicesTest()
        {
            AccountingApi accountingApi = new(Output.BuildLoggerFor<AccountingApi>());
            List<Invoice> response = accountingApi.GetInvoices(_accessToken, _tenantId);
            Assert.True(response.Count > 0);

            List<InvoiceSelect> selectCreditNote = response.Where(
                x => x.Type == Invoice.TypeEnum.ACCREC &&
                !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                x.AmountPaid == 0 &&
                x.AmountDue == 0 &&
                x.Status != Invoice.StatusEnum.VOIDED

                ).Select(x => new InvoiceSelect()
                {
                    InvoiceNumber = x.InvoiceNumber,
                    AmountDue = x.AmountDue.ToString(),
                    AmountPaid = x.AmountPaid.ToString(),
                    ContactName = x.Contact.Name
                }
                ).ToList();

            Output.WriteLine(string.Join("\n", selectCreditNote));
        }

        [Fact]
        public void DeleteSelectedSingleInvoiceTest()
        {
            AccountingApi accountingApi = new(Output.BuildLoggerFor<AccountingApi>());
            List<Invoice> response = accountingApi.GetInvoices(_accessToken, _tenantId);
            Assert.True(response.Count > 0);

            Invoice singleInvoice = response.Last(x => x.Type == Invoice.TypeEnum.ACCREC &&
                                                          !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                                                          x.AmountPaid == 0 &&
                                                          x.AmountDue == 0 &&
                                                          x.Status != Invoice.StatusEnum.VOIDED);

            Output.WriteLine(
                $"Invoice No:" + singleInvoice.InvoiceNumber + "\n" +
                $"AmountDue:" + singleInvoice.AmountDue + "\n" +
                $"AmountPaid:" + singleInvoice.AmountPaid + "\n" +
                $"Contact Name :" + singleInvoice.Contact.Name + "\n" +
                $"InvoiceID :" + singleInvoice.InvoiceID 
                );
            
            accountingApi.VoidInvoices(new Invoices() { _Invoices = new List<Invoice> { singleInvoice } }, _accessToken, _tenantId);
        }
        
        [Fact]
        public void DeleteSelectedInvoicesTest()
        {
            AccountingApi accountingApi = new(Output.BuildLoggerFor<AccountingApi>());
            List<Invoice> response = accountingApi.GetInvoices(_accessToken, _tenantId);
            Assert.True(response.Count > 0);

            List<Invoice> selectCreditNote = response.Where(
                x => x.Type == Invoice.TypeEnum.ACCREC &&
                     !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                     x.AmountPaid == 0 &&
                     x.AmountDue == 0 &&
                     x.Status != Invoice.StatusEnum.VOIDED

            ).ToList();

            Output.WriteLine(string.Join("\n", selectCreditNote.Select(x => new InvoiceSelect()
                {
                    InvoiceNumber = x.InvoiceNumber,
                    AmountDue = x.AmountDue.ToString(),
                    AmountPaid = x.AmountPaid.ToString(),
                    ContactName = x.Contact.Name
                }
            ).ToList()));

            accountingApi.VoidInvoices(new Invoices() { _Invoices = selectCreditNote }, _accessToken, _tenantId);
        }
    }

    /// <summary>
    /// Log format
    /// </summary>
    public class InvoiceSelect
    {
        public string InvoiceNumber { get; init; }
        public string AmountDue { get; init; }
        public string AmountPaid { get; init; }
        public string ContactName { get; init; }
        public override string ToString()
        {
            return $"InvoiceNo: {InvoiceNumber} , " +
                $"Name: {ContactName} , " +
                $"AmountDue: {AmountDue} , " +
                $"AmountPaid: {AmountPaid} , ";
        }
    }
}
