namespace Residuum.Services
{
    public static class ServiceConfiguration
    {
        public static Authentication BlizzardAuthentication { get; set; }

        public static string RealmName { get; set; }

        public static string GuildName { get; set; }

        public static string PageUri { get; set; }

        public static bool OverrideRaidProgressionSummary { get; set; }
    }

    public class Authentication
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }
    }
}