﻿namespace XeroServices.Tests.Settings
{
    public record LocalSettings
    {
        public string XeroUser { get; init; }
        public string XeroPass{ get; init; }
        public string TestAccessToken { get; init; }
    }
}