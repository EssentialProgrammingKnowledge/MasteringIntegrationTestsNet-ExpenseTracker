namespace ExpenseTracker.API.Security
{
    public class JwtSettings
    {
        public string IssuerSigningKey { get; set; } = string.Empty;
        public string IssuerName { get; set; } = string.Empty;
        public string AudienceName { get; set; } = string.Empty;
        public long ExpireTimeSeconds { get; set; } = 300;
    }
}
