using Azihub.Utilities.Base.Tests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.Tests.DataSamples;
using XeroServices.Tests.Settings;
using Xunit;
using Xunit.Abstractions;
using static XeroServices.Tests.Settings.AppSettings;

namespace XeroServices.Tests
{
    public class ApiTests : TestBase
    {

        public ApiTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void GetConnectionTest()
        {

            string tenantId = ApiClient.GetTenantId(AccessToken);
            Assert.True(string.IsNullOrEmpty(tenantId));
        }

        [Fact]
        public void GetOrganisationTest()
        {
            // Setup
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());
            accountingApi.SetAuth(AccessToken, TenantId);
            accountingApi.SetOrganisation(GetSampleData.GetSampleOrganisation());

            // Act
            Organisations response = accountingApi.GetOrganisations();
            
            // Assert
            string json = JsonConvert.SerializeObject(response._Organisations.First(), Formatting.Indented);
            Assert.True(response._Organisations.Count > 0);

        }
        [Fact]
        public void GetInvoicesTest()
        {
            // Setup
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());
            accountingApi.SetAuth(AccessToken, TenantId);
            accountingApi.SetOrganisation(GetSampleData.GetSampleOrganisation());
            
            // Act
            List<Invoice> response = accountingApi.GetInvoices(GetSampleData.GetSampleOrganisation());
            
            // Assert
            Assert.True(response.Count > 0);

            List<InvoiceSelect> selectCreditNote = response.Where(
                x => x.Type == Invoice.TypeEnum.ACCREC &&
                !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                x.AmountPaid == 0 &&
                x.AmountDue > 0 &&
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
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());
            accountingApi.SetAuth(AccessToken, TenantId);
            List<Invoice> response = accountingApi.GetInvoices(GetSampleData.GetSampleOrganisation());
            Assert.True(response.Count > 0);

            Invoice singleInvoice = response.Last(x => x.Type == Invoice.TypeEnum.ACCREC &&
                                                          !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                                                          //x.AmountPaid == 0 &&
                                                          x.AmountDue >0 &&
                                                          x.Status != Invoice.StatusEnum.VOIDED);

            Output.WriteLine(
                $"Invoice No:" + singleInvoice.InvoiceNumber + "\n" +
                $"AmountDue:" + singleInvoice.AmountDue + "\n" +
                $"AmountPaid:" + singleInvoice.AmountPaid + "\n" +
                $"Contact Name :" + singleInvoice.Contact.Name + "\n" +
                $"InvoiceID :" + singleInvoice.InvoiceID 
                );
            
            accountingApi.VoidInvoices(new Invoices() { _Invoices = new List<Invoice> { singleInvoice } });
        }
        
        [Fact]
        public void DeleteSelectedInvoicesTest()
        {
            XeroServices accountingApi = new(Output.BuildLoggerFor<XeroServices>());
            accountingApi.SetAuth(AccessToken, TenantId, XeroLogin);

            List<Invoice> response = accountingApi.GetInvoices(GetSampleData.GetSampleOrganisation());
            Assert.True(response.Count > 0);

            List<Invoice> selectCreditNote = response.Where(
                x => x.Type == Invoice.TypeEnum.ACCREC &&
                     !x.InvoiceNumber.StartsWith("INV-LIC01") &&
                     //x.AmountPaid == 0 &&
                     x.AmountDue == 0 &&
                     x.Status != Invoice.StatusEnum.VOIDED

            ).ToList();

            Output.WriteLine(
                string.Join("\n", selectCreditNote.Select(x => new InvoiceSelect()
            {
                InvoiceNumber = x.InvoiceNumber,
                AmountDue = x.AmountDue.ToString(),
                AmountPaid = x.AmountPaid.ToString(),
                ContactName = x.Contact.Name
            }
            ).ToList()));

            accountingApi.VoidInvoices(new Invoices() { _Invoices = selectCreditNote });
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
