using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.ErrorResponse;
using XeroServices.Exceptions;
using XeroServices.Login;
using XeroServices.WebDriver;
using static XeroServices.XeroSetting;
using XeroOrganisation = Xero.NetStandard.OAuth2.Model.Accounting.Organisation;

namespace XeroServices
{
    public partial class XeroServices
    {
        public ILogger<XeroServices> Logger { get; }

        public AccountingApi AccountingApi { get; }
        public XeroServices(ILogger<XeroServices> logger)
        {
            Logger = logger;
            AccountingApi = new();
        }

        public Organisations GetOrganisations()
        {
            Organisations result;
            try
            {
                result = AccountingApi.GetOrganisationsAsync(AccessToken, TenantId).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                throw;
            }
            return result;
        }

        public List<Invoice> GetInvoices(XeroOrganisation organisation)
        {
            Organisation = organisation;
            Invoices result;
            try
            {
                Xero.NetStandard.OAuth2.Api.AccountingApi accountingApi = new();
                result = accountingApi.GetInvoicesAsync(AccessToken, TenantId).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                throw;
            }

            return result._Invoices;
        }

        public void VoidInvoices(Invoices invoices)
        {
            Xero.NetStandard.OAuth2.Api.AccountingApi accountingApi = new();
            foreach (var invoice in invoices._Invoices)
            {
                invoice.Status = Invoice.StatusEnum.VOIDED;
            }

            // create chunk of 1000 pages
            List<Invoice> invoiceChunk;
            int page = 1;
            do
            {
                invoiceChunk = invoices._Invoices.Page(page, 100).ToList();

                bool success = false;
                do
                {

                    try
                    {

                        var result =
                            accountingApi.UpdateOrCreateInvoicesAsync(AccessToken, TenantId, new Invoices { _Invoices = invoiceChunk }, true)
                                                    .GetAwaiter().GetResult();

                        Logger.LogDebug(result._Invoices.ToString());
                        success = true;
                    }
                    catch (ApiException e)
                    {

                        const string FAILED_TO_DELETE_ERROR_MESSAGE = "The contact with the specified contact details has been archived. " +
                            "The contact must be un-archived before creating new invoices or credit notes.";
                        const string BLOCKED_BY_PAYMENTS = "This document cannot be edited as it has a payment or credit note allocated to it.";

                        ErrorResponseInvoice invoiceValidationError = JsonConvert.DeserializeObject<ErrorResponseInvoice>(e.ErrorContent);

                        List<Invoice> invoicesVoidByWeb = new();
                        foreach (var invoice in invoiceValidationError.Elements)
                        {
                            if (invoice.ValidationErrors[0].Message == FAILED_TO_DELETE_ERROR_MESSAGE ||
                                invoice.ValidationErrors[0].Message == BLOCKED_BY_PAYMENTS)
                            {
                                invoicesVoidByWeb.Add(invoice);
                                //contacts.Add(
                                //                                    new()
                                //                                    {
                                //                                        ContactID = invoice.Contact.ContactID,
                                //                                        ContactStatus = Contact.ContactStatusEnum.ACTIVE
                                //                                    }
                                //                                );

                                
                                //accountingApi.UpdateContactAsync(
                                //    accessToken,
                                //    tenantId,
                                //    (Guid)invoice.Contact.ContactID,
                                //     new Contacts
                                //     {
                                //         _Contacts = new()
                                //         {
                                //             new Contact
                                //             {
                                //                 ContactID = invoice.Contact.ContactID,
                                //                 ContactStatus = Contact.ContactStatusEnum.ACTIVE
                                //             }
                                //         }
                                //     }
                                //).GetAwaiter().GetResult();

                            }
                            else
                                Logger.LogError("We have unhandled error message:" + invoice.ValidationErrors[0].Message);
                        }

                        if (invoicesVoidByWeb.Count > 0)
                        {
                            DeleteOrVoidInvoicesByWebDriver(new() { _Invoices = invoicesVoidByWeb });
                        }

                        success = true;
                        // accountingApi.UpdateOrCreateContactsAsync(accessToken, tenantId, new Contacts { _Contacts = contacts }).GetAwaiter().GetResult();

                        //Logger.LogError(e.Message, e);
                    }
                } while (success == false);

                page++;

            } while (invoiceChunk.Count > 0);


        }

        
    }


    public static class PagingExtensions
    {
        //used by LINQ to SQL
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        //used by LINQ
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

    }

}
