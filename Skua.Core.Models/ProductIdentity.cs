namespace Skua.Core.Models;

/// <summary>
/// Central source for user-visible HOT DOG product identity and brand defaults.
/// Keep runtime compatibility identifiers (assemblies, executable names, paths,
/// IPC and mutex names) out of this class; those require dedicated migrations.
/// </summary>
public static class ProductIdentity
{
    public const string ProductName = "HOT DOG";
    public const string ManagerName = ProductName + " Manager";
    public const string ClientMenuName = ProductName + " Client";
    public const string AboutTitle = "About " + ProductName;
    public const string DefaultThemeName = ProductName;

    // Warm orange + mustard accents on the existing charcoal dark surface palette.
    // The near-black foreground keeps text legible on both accent fills.
    public const string PrimaryAccentHex = "#FFF97316";
    public const string SecondaryAccentHex = "#FFFBBF24";
    public const string AccentForegroundHex = "#FF111111";
}
