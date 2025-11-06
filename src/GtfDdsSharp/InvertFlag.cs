namespace GtfDdsSharp;

/// <summary>
/// Specifies how to invert memory when converting a texture.
/// </summary>
public enum InvertFlag : uint
{
    /// <summary>
    /// No endian inversion.
    /// </summary>
    None = 0,
    /// <summary>
    /// Invert 32-bit values.
    /// </summary>
    InvertEndian32 = 1,
    /// <summary>
    /// Invert 16-bit values.
    /// </summary>
    InvertEndian16 = 2,
    /// <summary>
    /// Invert 32-bit values on even addresses.
    /// </summary>
    InvertEndian32Even = 3
}
