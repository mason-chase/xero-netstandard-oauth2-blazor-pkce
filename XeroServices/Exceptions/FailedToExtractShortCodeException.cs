using System;
using System.Runtime.Serialization;

namespace XeroServices
{
    [Serializable]
    internal class FailedToExtractShortCodeException : Exception
    {
        private string OrganisationName;
        private string ShortCode;
        private byte[] ScreenShot;
        public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organisationName"></param>
        /// <param name="shortCode"></param>
        /// <param name="screenShot"></param>
        /// <param name="exception"></param>
        public FailedToExtractShortCodeException(string organisationName, string shortCode, byte[] screenShot, Exception exception)
        {
            OrganisationName = organisationName;
            ShortCode = shortCode;
            ScreenShot = screenShot;
            Exception = exception;
        }

        protected FailedToExtractShortCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}