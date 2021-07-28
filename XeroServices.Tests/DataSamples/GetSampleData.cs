using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeroServices.ErrorResponse;

namespace XeroServices.Tests.DataSamples
{
    public static class GetSampleData
    {
        public static readonly char Ds = Path.DirectorySeparatorChar;
        public static readonly string DataPath = $"DataSamples{Ds}InvoiceUpdateErrors.json";


        public static ErrorResponseInvoice GetSampleErrors()
        {
            string jsonFile = File.ReadAllText(DataPath);
            return JsonConvert.DeserializeObject<ErrorResponseInvoice>(jsonFile);

        }
    }
}
