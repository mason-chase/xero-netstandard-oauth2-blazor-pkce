namespace XeroServices.Utilities
{
    public class BankTools
    {
        /// <summary>
        /// Converts Bank CSV statement and export into a list of <see cref="XeroStatement"/> values and saves it to the specified exportPath.
        /// <param name="csv">the serialized values of IXeroStatement instances</param>
        /// <param name="bankType">the real type of IXeroStatement instance, for example, 'saman'</param>
        /// </summary>
        //public static string ConvertBankCsvToXeroCsv(BankType bankType, string csv)
        //    {
        //        // get instance of classMap converter based on BankName
        //        IList<XeroStatement> xeroList = StatementFactory
        //            .GetInstance(bankType, csv)
        //            .Select(item => new XeroStatement(item))
        //            .ToList();

        //        const string separatorChar = ",";

        //        using (StringWriter sw = new StringWriter())
        //        {
        //            CsvWriter writer = new CsvWriter(sw);
        //            writer.Configuration.RegisterClassMap(XeroStatement.CsvClassMap);
        //            writer.Configuration.Delimiter = separatorChar;
        //            writer.Configuration.CultureInfo = CultureInfo.InvariantCulture;
        //            writer.Configuration.HasHeaderRecord = false;
        //            writer.Configuration.Quote = '\"';

        //            // Create a sample of Xero Statement and add header
        //            XeroStatement xeroStatement = new XeroStatement();
        //            foreach (PropertyInfo column in xeroStatement.GetType().GetProperties())
        //            {
        //                writer.WriteField(column.Name);
        //            }

        //            writer.NextRecord();

        //            // Write data
        //            for (int i = 0; i < xeroList.Count; ++i)
        //            {
        //                writer.WriteRecord(xeroList[i]);
        //                if (i < xeroList.Count - 1)
        //                {
        //                    writer.NextRecord();
        //                }
        //                else
        //                {
        //                    // Do not write newline after last rec
        //                    writer.Flush();
        //                }
        //            }
                        
        //            return sw.ToString();
        //        }

        //    }

    }
}