namespace GtfDdsSharp;

/// <summary>
/// Specifies options for DDS to GTF conversion.
/// </summary>
[Flags]
public enum ConvertOptions
{
    /// <summary>
    /// Convert to linear format.
    /// </summary>
    Linearize = 1 << 0,
    /// <summary>
    /// Convert to unnormalized format.
    /// </summary>
    Unnormalize = 1 << 1
}
