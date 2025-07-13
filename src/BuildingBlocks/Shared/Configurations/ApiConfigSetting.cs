namespace Shared.Configurations;

public class ApiConfigSetting
{
    public string ApiTitle { get; set; }
    public string ApiName { get; set; }
    public string ApiVersion { get; set; }
    public string ApiBaseUrl { get; set; }
    public string IdentityServerBaseUrl { get; set; }
    public string IssuerUri { get; set; }
    public string ClientId { get; set; }
    public bool CorsAllowAnyOrigin { get; set; }
    public string[] CorsAllowOrigin { get; set; }
}