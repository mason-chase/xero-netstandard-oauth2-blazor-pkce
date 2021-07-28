using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace XeroServices.ErrorResponse
{
    public class ErrorResponseInvoice
    {
        public int ErrorNumber { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public List<Invoice> Elements { get; set; }
    }

}
