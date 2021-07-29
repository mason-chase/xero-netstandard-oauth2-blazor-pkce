using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeroServices.Exceptions
{
    /// <summary>
    ///     Submitted xero credential has error by request xero login
    /// </summary>
    public class LoginErrorException : Exception
    {
        public byte[] Screenshot { get; }
        public string XeroUsername { get; }
        public string XeroLoginErrorText { get; }

        public LoginErrorException(string xeroUsername, string xeroLoginErrorText, byte[] screenshot = null)
        {
            Screenshot = screenshot;
            XeroLoginErrorText = xeroLoginErrorText;
            XeroLoginErrorText = xeroUsername;
        }
    }
}
