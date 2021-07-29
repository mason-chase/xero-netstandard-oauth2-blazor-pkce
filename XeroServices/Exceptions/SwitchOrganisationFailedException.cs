using System;
using System.Runtime.Serialization;

namespace XeroServices
{
    [Serializable]
    internal class SwitchOrganisationFailedException : Exception
    {
        private string name;
        private string shortCode;
        private string orgShortCode;
        private byte[] vs;

        public SwitchOrganisationFailedException()
        {
        }

        public SwitchOrganisationFailedException(string message) : base(message)
        {
        }

        public SwitchOrganisationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SwitchOrganisationFailedException(string name, string shortCode, string orgShortCode, byte[] vs)
        {
            this.name = name;
            this.shortCode = shortCode;
            this.orgShortCode = orgShortCode;
            this.vs = vs;
        }

        protected SwitchOrganisationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}