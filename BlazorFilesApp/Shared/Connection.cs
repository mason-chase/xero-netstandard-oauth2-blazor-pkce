using Newtonsoft.Json;
using System;

namespace BlazorFilesApp.Shared
{
    public class Connection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("authEventId")]
        public string AuthEventId { get; set; }
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }
        [JsonProperty("tenantType")]
        public string TenantType { get; set; }
        [JsonProperty("tenantName")]
        public string TenantName { get; set; }
        [JsonProperty("createdDateUtc")]
        public DateTime CreatedDateUtc { get; set; }
        [JsonProperty("updatedDateUtc")]
        public DateTime UpdatedDateUtc { get; set; }
    }
}