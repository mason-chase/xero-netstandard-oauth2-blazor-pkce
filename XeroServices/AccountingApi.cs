using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.ErrorResponse;

namespace XeroServices
{
    public class AccountingApi
    {
        public ILogger<AccountingApi> Logger { get; }
        public AccountingApi(ILogger<AccountingApi> logger)
        {
            Logger = logger;
        }
        public List<Invoice> GetInvoices(string accessToken, string tenantId)
        {
            Invoices result;
            try
            {
                Xero.NetStandard.OAuth2.Api.AccountingApi accountingApi = new();
                result = accountingApi.GetInvoicesAsync(accessToken, tenantId).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                throw;
            }

            return result._Invoices;
        }

        public void VoidInvoices(Invoices invoices, string accessToken, string tenantId)
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
                            accountingApi.UpdateOrCreateInvoicesAsync(accessToken, tenantId, new Invoices { _Invoices = invoiceChunk }, true)
                                                    .GetAwaiter().GetResult();

                        Logger.LogDebug(result._Invoices.ToString());
                        success = true;
                    }
                    catch (ApiException e)
                    {

                        const string ERROR_MESSAGE = "The contact with the specified contact details has been archived. " +
                            "The contact must be un-archived before creating new invoices or credit notes.";

                        ErrorResponseInvoice invoiceValidationError = JsonConvert.DeserializeObject<ErrorResponseInvoice>(e.ErrorContent);

                        List<Contact> contacts = new();
                        foreach (var invoice in invoiceValidationError.Elements)
                        {
                            if (invoice.ValidationErrors[0].Message == ERROR_MESSAGE)
                            {
                                contacts.Add(
                                                                    new()
                                                                    {
                                                                        ContactID = invoice.Contact.ContactID,
                                                                        ContactStatus = Contact.ContactStatusEnum.ACTIVE
                                                                    }
                                                                );

                                accountingApi.UpdateContactAsync(
                                    accessToken,
                                    tenantId,
                                    (Guid)invoice.Contact.ContactID,
                                     new Contacts
                                     {
                                         _Contacts = new()
                                         {
                                             new Contact
                                             {
                                                 ContactID = invoice.Contact.ContactID,
                                                 ContactStatus = Contact.ContactStatusEnum.ACTIVE
                                             }
                                         }
                                     }
                                ).GetAwaiter().GetResult();

                                Thread.Sleep(1000);
                            }
                            else
                                Logger.LogError("We have unhandled error message:" + invoice.ValidationErrors[0].Message);
                        }

                        // accountingApi.UpdateOrCreateContactsAsync(accessToken, tenantId, new Contacts { _Contacts = contacts }).GetAwaiter().GetResult();

                        Logger.LogError(e.Message, e);
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
