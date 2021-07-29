using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroServices.ErrorResponse;
using XeroServices.Exceptions;
using XeroServices.Login;
using XeroServices.Utilities;
using XeroServices.WebDriver;
using static XeroServices.XeroSetting;
using XeroOrganisation = Xero.NetStandard.OAuth2.Model.Accounting.Organisation;

namespace XeroServices
{
    public partial class XeroServices
    {
        private bool LoggedIn { get; set; }
        protected XeroLogin XeroLogin { get; set; }
        protected Organisation Organisation { get; set; }
        protected string AccessToken { get; set; }
        protected string TenantId { get; set; }
        public void SetAuth(string accessToken, string tenantId = null, XeroLogin xeroLogin = null)
        {
            AccessToken = accessToken;
            XeroLogin = xeroLogin;
            TenantId = tenantId;
        }

        public void SetOrganisation(XeroOrganisation organisation)
        {
            Organisation = organisation;
        }

        public void SetOrganisation(string shortCode)
        {
            GetOrganisations()._Organisations.First(x => x.ShortCode == shortCode);
        }
        private void DeleteOrVoidInvoicesByWebDriver(Invoices invoices)
        {
            if (!LoggedIn)
                Login();

            foreach (Invoice invoice in invoices._Invoices)
            {
                DeleteOrVoidInvoiceByWebDriver(invoice);
            }
        }

        /// <summary>
        /// Direct call to private function for testing
        /// </summary>
        /// <param name="organisation"></param>
        /// <param name="invoices"></param>
        /// <param name="xeroLogin"></param>
        public void DeleteOrVoidInvoicesByWebDriver(XeroLogin xeroLogin, Organisation organisation, Invoices invoices)
        {
            XeroLogin = xeroLogin;
            Organisation = organisation;
        }

        public void DeleteOrVoidInvoiceByWebDriver(Invoice invoice)
        {
            WebDriver.GoToUrlWait(string.Format(INVOICE_URL, invoice.InvoiceID));

            try
            {
                //IWebElement status = WebDriver.FindElementWait(By.XPath($"//strong[contains(text(),'Voided')]"), 1000, 1);
                IWebElement status = WebDriver.FindElementWait(By.XPath($"//div[contains(@class,'status')]/div/strong"), 1000, 1);
                Logger.LogInformation($"InvoiceId {invoice.InvoiceID} already voided: {status.Text.Trim()}");
                if (status.Text.Trim().ToUpper() == "VOIDED")
                    return;
            }
            catch (NotFoundException)
            {
                // Invoice is not voided
            }

            List<IWebElement> payments = new();
            try
            {
                payments = WebDriver.FindElementsWait(By.XPath("//A[text()='Payment']"), 2000, 1).ToList();

                if (payments.Count > 0)
                {
                    List<string> xeroPayment = payments.Select(x => x.GetAttribute("href")).ToList();
                    foreach (IWebElement payment in payments)
                    {
                        Guid paymentId = (Guid)payment.GetAttribute("href").FindGuid();
                        AccountingApi.DeletePaymentAsync(AccessToken, TenantId, paymentId, new() { Status = "DELETE" })
                            .ConfigureAwait(true).GetAwaiter().GetResult();
                    }
                }
            }
            catch (NotFoundException)
            {
                // No Payments are attached

            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
#if !DEBUG
                throw;
#else
                Debugger.Break();
#endif
            }

            List<IWebElement> creditNotes = new();
            try
            {
                creditNotes = WebDriver.FindElementsWait(By.XPath("//A[text()='Credit Note']"), 2000, 3).ToList();

                if (creditNotes.Count > 0)
                {
                    foreach (IWebElement creditNote in creditNotes)
                    {
                        Guid creditNoteId = (Guid)creditNote.GetAttribute("href").FindGuid();
                        var xeroCreditNote = AccountingApi.GetCreditNoteAsync(AccessToken, TenantId, creditNoteId)
                            .ConfigureAwait(true).GetAwaiter().GetResult();

                        ((IJavaScriptExecutor)WebDriver).ExecuteScript("creditNote = window.open();");
                        WebDriver.SwitchTo().Window(WebDriver.WindowHandles.Last());
                        WebDriver.GoToUrlWait(string.Format(CREDIT_NOTE_URL, creditNoteId));

                        List <IWebElement> allocations = WebDriver.FindElementsWait(By.XPath("//div[@class='payment']//a[contains(text(),'Invoice')]"), 2000, 3).ToList();
                        List<IWebElement> allocationsDelete = WebDriver.FindElementsWait(By.XPath("//a[contains(@class,'delete-small')]"), 2000, 3).ToList();
                        
                        for(int i = 0; i < allocations.Count(); i++)
                        {
                            Guid allocationId = (Guid)allocations[i].GetAttribute("href").FindGuid();
                            if (allocationId == invoice.InvoiceID)
                            {
                                allocationsDelete[i].Click();
                                // //button[text()='OK']
                                IWebElement clickOk = WebDriver.FindElementWait(By.XPath("//a[contains(@class,'delete-small')]"), 2000, 3);
                                clickOk.Click();
                            }

                        }

                        try
                        {
                            IWebElement options = WebDriver.FindElementWait(By.XPath("//div[contains(text(),'Credit Note Options')]"), 2000, 3);
                            options.Click();

                            IWebElement voidCreditNote = WebDriver.FindElementWait(By.XPath("//a[contains(text(),'Void')]"), 2000, 3);
                            voidCreditNote.Click();
                        }
                        catch (NotFoundException)
                        {

                        }
                        
                        ((IJavaScriptExecutor)WebDriver).ExecuteScript("creditNote.close();");
                    }
                }
            }
            catch (NotFoundException)
            {
                // No credit Note
            }

            IWebElement invoiceOption = WebDriver.FindElementWait(By.XPath($"//div[contains(text(),'Invoice Options')]"), 2000, 3);
            invoiceOption.Click();

            IWebElement voidButton = WebDriver.FindElementWait(By.XPath($"//a[contains(text(),'Void')]"), 2000, 3);
            voidButton.Click();

            IWebElement okButton = WebDriver.FindElementWait(By.XPath($"//button[contains(text(),'OK')]"), 2000, 3);
            okButton.Click();

            try
            {
                /*
                 * Invoice voided
                    <div class="x-msg green" id="ext-gen1037">
                        <p>Invoice voided</p>
                        <p class="small">
                            <a href="/AccountsReceivable/View.aspx?invoiceID=ff38eaa1-f037-4f5c-907d-b04a2245be90">View Invoice</a>
                        </p>
                        <a class="close" id="ext-gen1038">&nbsp;</a>
                    </div>
                 */
                WebDriver.FindElementWait(By.XPath($"//p[contains(text(),'Invoice voided')]"), 2000, 3);
            }
            catch (NotFoundException)
            {
                WebDriver.FindElementWait(By.XPath($"//button[contains(text(),'OK')]"), 2000, 3);
            }
        }

        private IWebDriver WebDriver { get; set; }
        private void Login() 
        {
            WebDriver = WebDriverFactory.Create();
            WebDriver.GoToUrlWait(GO_XERO_LOGIN_URL);


            IWebElement email = WebDriver.FindElementWait(By.XPath("//input[@name='Username']"), 1000, 2);
            email.Clear();
            // Xero login was change and detect robot behavior, for this reason we must send each character by its own keys separately
            foreach (Char item in XeroLogin.Email)
            {
                email.SendKeys(item.ToString());
            }

            IWebElement password = WebDriver.FindElement(By.XPath("//input[@name='Password']"));
            password.Clear();
            // Xero login was change and detect robot behavior, for this reason we must send each character by its own keys separately
            foreach (Char item in XeroLogin.Password)
            {
                password.SendKeys(item.ToString());
            }

            // Thread.Sleep(1000);
            IWebElement loginBtn = WebDriver.FindElement(By.XPath("//button[@name='button']"));

            loginBtn.Click();

            // Wait for page load after submit
            WebDriver.WaitForReadyState();

            SwitchOrganisation();
            LoggedIn = true;
        }

        private void SwitchOrganisation()
        {
            WebDriver.GoToUrlWait(string.Format(ORGANISATION_LOGIN_BY_SHORT_CODE_URL, Organisation.ShortCode));

            string orgShortCode;
            try
            {
                // Get organisation short code
                string useBootstrapApiData = WebDriver.ExecuteScript("return JSON.parse(document.getElementById('dashboard-data').innerHTML).useBootstrapApiData;").ToString().ToLower();
                if (useBootstrapApiData == "true")
                    orgShortCode = WebDriver.ExecuteScript("return JSON.parse(document.getElementById('header-data').innerHTML).substitutions.organisationShortCodeToken;").ToString();
                else
                    orgShortCode = WebDriver.ExecuteScript("return JSON.parse(document.getElementById('dashboard-data').innerHTML).organisationShortCodeToken;").ToString();

            }
            catch (Exception ex)
            {
                // Todo: #397
                throw new FailedToExtractShortCodeException(Organisation.Name, Organisation.ShortCode, WebDriver.GetScreenshotBytes(), ex);
            }

            // Compare with Company short code in dashboard
            if (orgShortCode != Organisation.ShortCode)
            {
                // Todo: #397
                throw new SwitchOrganisationFailedException(Organisation.Name, Organisation.ShortCode, orgShortCode, WebDriver.GetScreenshotBytes());
            }
        }

        private void CheckForLoginError(XeroLogin login)
        {
            try
            {
                WebDriver.WaitForReadyState(3000);
                IWebElement error = WebDriver.FindElementWait(By.XPath("//form//div[contains(@class, 'validation')]//li"), 1000, 3);
                throw new LoginErrorException(login.Email, error.Text, WebDriver.GetScreenshotBytes());
            }
            catch (NotFoundException)
            {
                // Successful login
            }
        }
    }
}
