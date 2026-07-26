namespace TryOnReady.Api.Authentication;

internal sealed class DemoAccessOptions
{
    public const string SectionName = "DemoAccess";

    public DemoAccountOptions Retailer { get; init; } = new();

    public DemoAccountOptions Administrator { get; init; } = new();
}

internal sealed class DemoAccountOptions
{
    public string Username { get; init; } = "";

    public string Password { get; init; } = "";

    public string DisplayName { get; init; } = "";

    public string BoutiqueId { get; init; } = "";
}

internal sealed record DemoLoginRequest(string? Username, string? Password);

internal static class DemoAccessPolicies
{
    public const string Retailer = "Retailer";
    public const string Administrator = "Administrator";
    public const string RetailerOrAdministrator = "RetailerOrAdministrator";
}
