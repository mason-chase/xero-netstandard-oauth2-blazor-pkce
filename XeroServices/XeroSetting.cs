using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeroServices
{
    public class XeroSetting
    {
        public const string LOGIN_URL = "https://login.xero.com";
        public const string MY_XERO_LOGIN_URL = "https://my.xero.com";
        public const string GO_XERO_LOGIN_URL = "https://login.xero.com/identity/user/login";
        public const string DEVELOPER_LOGIN_URL = "https://developer.xero.com/myapps/";
        public const string ORGANISATION_LOGIN_BY_SHORT_CODE_URL = "https://go.xero.com/OrganisationLogin/?shortCode={0}";
        public const string ORGANISATION_BY_SHORT_CODE_AND_USER_ID_URL = "https://my.xero.com/{0}/Action/OrganisationLogin/{1}";
        public const string BANK_ACCOUNTS_URL = "https://go.xero.com/Bank/BankAccounts.aspx";
        public const string BANK_ACCOUNT_STATEMENTS_BY_UUID_URL = "https://go.xero.com/Bank/Statements.aspx?accountID={0}";
        public const string BANK_ACCOUNT_STATEMENT_LINES_BY_UUID_URL = "https://go.xero.com/Bank/Statement.aspx?accountID={0}&statementID={1}";
        public const string BANK_ACCOUNT_TRANSACTIONS_BY_UUID_URL = "https://go.xero.com/Bank/BankTransactions.aspx?accountID={0}";
        public const string ORGANISATION_DETAIL_URL = "https://go.xero.com/Settings/Organisation";
        public const string UPLOAD_STATEMENT_BY_ACCOUNT_UUID_URL = "https://go.xero.com/Bank/Import.aspx?accountID={0}";
        public const string CHART_OF_ACCOUNTS_URL = "https://go.xero.com/GeneralLedger/ChartOfAccounts.aspx";
        public const string ACCOUNT_TRANSACTIONS_URL = "https://reporting.xero.com/{0}/summary";
        public const string LOGOUT_URL = "https://login.xero.com/signin/loggedout";
        public const string API_URL = "https://api.xero.com";
        public const string INVOICE_URL = "https://go.xero.com/AccountsReceivable/View.aspx?InvoiceID={0}";
    }
}
