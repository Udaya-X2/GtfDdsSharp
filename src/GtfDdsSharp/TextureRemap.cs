namespace GtfDdsSharp;

/// <summary>
/// Defines constants used for component remapping when converting textures from DDS to GTF.
/// </summary>
public static class TextureRemap
{
    /// <summary>
    /// Use constant zero for the remapped component.
    /// </summary>
    public const uint Zero = 0;

    /// <summary>
    /// Use constant one for the remapped component.
    /// </summary>
    public const uint One = 1;

    /// <summary>
    /// Use the remapped source value for the component.
    /// </summary>
    public const uint Remap = 2;

    /// <summary>
    /// Source component: alpha.
    /// </summary>
    public const uint FromAlpha = 0;

    /// <summary>
    /// Source component: red.
    /// </summary>
    public const uint FromRed = 1;

    /// <summary>
    /// Source component: green.
    /// </summary>
    public const uint FromGreen = 2;

    /// <summary>
    /// Source component: blue.
    /// </summary>
    public const uint FromBlue = 3;

    /// <summary>
    /// Preset remap sequence for ARGB ordering.
    /// </summary>
    public const uint OrderARGB = MaskRRRR | FromAlpha | (FromRed << 2) | (FromGreen << 4) | (FromBlue << 6);

    /// <summary>
    /// Preset remap sequence for BGRA ordering.
    /// </summary>
    public const uint OrderBGRA = MaskRRRR | FromBlue | (FromGreen << 2) | (FromRed << 4) | (FromAlpha << 6);

    /// <summary>
    /// Preset remap sequence for ABGR ordering.
    /// </summary>
    public const uint OrderABGR = MaskRRRR | FromAlpha | (FromBlue << 2) | (FromGreen << 4) | (FromRed << 6);

    /// <summary>
    /// Preset remap sequence for AGRB ordering.
    /// </summary>
    public const uint OrderAGRB = MaskRRRR | FromAlpha | (FromGreen << 2) | (FromRed << 4) | (FromBlue << 6);

    /// <summary>
    /// Preset remap sequence for ARBG ordering.
    /// </summary>
    public const uint OrderARBG = MaskRRRR | FromAlpha | (FromRed << 2) | (FromBlue << 4) | (FromGreen << 6);

    /// <summary>
    /// Preset remap sequence for 1RGB ordering.
    /// </summary>
    public const uint Order1RGB = Mask1RRR | FromAlpha | (FromRed << 2) | (FromGreen << 4) | (FromBlue << 6);

    /// <summary>
    /// Preset remap sequence for 1BBB ordering.
    /// </summary>
    public const uint Order1BBB = Mask1RRR | FromBlue | (FromBlue << 2) | (FromBlue << 4) | (FromBlue << 6);

    /// <summary>
    /// Preset remap sequence for B000 ordering.
    /// </summary>
    public const uint OrderB000 = MaskR000 | FromBlue | (FromBlue << 2) | (FromBlue << 4) | (FromBlue << 6);

    /// <summary>
    /// Remap mask with all <see cref="Remap"/> flags.
    /// </summary>
    public const uint MaskRRRR = (Remap << 8) | (Remap << 10) | (Remap << 12) | (Remap << 14);

    /// <summary>
    /// Remap mask with <see cref="One"/> followed by all <see cref="Remap"/> flags.
    /// </summary>
    public const uint Mask1RRR = (One << 8) | (Remap << 10) | (Remap << 12) | (Remap << 14);

    /// <summary>
    /// Remap mask with <see cref="Remap"/> followed by all <see cref="Zero"/> flags.
    /// </summary>
    public const uint MaskR000 = (Remap << 8) | (Zero << 10) | (Zero << 12) | (Zero << 14);
}
