using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.ErrorResponse;

namespace XeroServices.Tests.DataSamples
{
    public static class GetSampleData
    {
        public static readonly char Ds = Path.DirectorySeparatorChar;
        public static readonly string OrganisationPath = $"DataSamples{Ds}Organisation.json";
        public static readonly string InvoiceUpdateErrorsPath = $"DataSamples{Ds}InvoiceUpdateErrors.json";

        public static Organisation GetSampleOrganisation()
        {
            string jsonFile = File.ReadAllText(OrganisationPath);
            return JsonConvert.DeserializeObject<Organisation>(jsonFile);
        }

        public static ErrorResponseInvoice GetSampleErrors()
        {
            string jsonFile = File.ReadAllText(InvoiceUpdateErrorsPath);
            return JsonConvert.DeserializeObject<ErrorResponseInvoice>(jsonFile);

        }
    }
}
